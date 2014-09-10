using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Models;
using NLog;
using GW2PAO.Controllers.Interfaces;
using System.Collections.ObjectModel;
using GW2PAO.ViewModels;
using GW2PAO.Utility;
using GW2PAO.API.Data.Enums;
using GW2PAO.TrayIcon;
using GW2PAO.API.Data;
using GW2PAO.ViewModels.Interfaces;
using GW2PAO.API.Util;

namespace GW2PAO.Controllers
{
    public class WvWController : IWvWController
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private const int EternalBattlegroundsMapID = 38;
        private const int RedBorderlandsMapID = 94;
        private const int GreenBorderlandsMapID = 95;
        private const int BlueBorderlandsMapID = 96;
        private const int EdgeOfMistsMapID = 968;

        /// <summary>
        /// Service responsible for WvW information
        /// </summary>
        private IWvWService wvwService;

        /// <summary>
        /// Service responsible for Player information
        /// </summary>
        private IPlayerService playerService;

        /// <summary>
        /// Keeps track of how many times Start() has been called in order
        /// to support reuse of a single object
        /// </summary>
        private int startCallCount;

        /// <summary>
        /// The current match id monitored by the controller
        /// </summary>
        private string matchID;

        /// <summary>
        /// The objectives refresh timer object
        /// </summary>
        private Timer objectivesRefreshTimer;

        /// <summary>
        /// Locking object for operations performed with the objectivesRefreshTimerLock
        /// </summary>
        private readonly object objectivesRefreshTimerLock = new object();

        /// <summary>
        /// Timer counter used for reducing the amount of requests performed
        /// See RefreshObjectives() for more details
        /// </summary>
        private int timerCount;

        /// <summary>
        /// User settings for dungeons
        /// </summary>
        private WvWSettings userSettings;

        /// <summary>
        /// The object containing the WvWMap shown to the user
        /// </summary>
        private IHasWvWMap mapObj;

        /// <summary>
        /// Previous WvW map
        /// </summary>
        private WvWMap prevMap;

        /// <summary>
        /// The player's current WvWMap
        /// </summary>
        private WvWMap PlayerMap
        {
            get
            {
                var currentMapId = this.playerService.MapId;
                switch (currentMapId)
                {
                    case EternalBattlegroundsMapID:
                        return WvWMap.EternalBattlegrounds;
                    case RedBorderlandsMapID:
                        return WvWMap.RedBorderlands;
                    case GreenBorderlandsMapID:
                        return WvWMap.GreenBorderlands;
                    case BlueBorderlandsMapID:
                        return WvWMap.BlueBorderlands;
                    default:
                        return WvWMap.Unknown;
                }
            }
        }

        /// <summary>
        /// Map with which to override the player map.
        /// To disable the override, set this to Unknown
        /// </summary>
        public WvWMap MapOverride
        {
            get { return this.UserSettings.MapOverride; }
            set { this.UserSettings.MapOverride = value; }
        }

        /// <summary>
        /// The interval by which to refresh the objectives state
        /// </summary>
        public int ObjectivesRefreshInterval { get; set; }

        /// <summary>
        /// The WvW user settings
        /// </summary>
        public WvWSettings UserSettings { get { return this.userSettings; } }

        /// <summary>
        /// Backing store of the teams collection
        /// </summary>
        private ObservableCollection<WvWTeamViewModel> teams = new ObservableCollection<WvWTeamViewModel>();

        /// <summary>
        /// The collection of WvW Teams
        /// </summary>
        public ObservableCollection<WvWTeamViewModel> Teams { get { return this.teams; } }

        /// <summary>
        /// Backing store of the All WvW Objectives collection
        /// </summary>
        private ObservableCollection<WvWObjectiveViewModel> allobjectives = new ObservableCollection<WvWObjectiveViewModel>();

        /// <summary>
        /// The collection of All WvW Objectives
        /// </summary>
        public ObservableCollection<WvWObjectiveViewModel> AllObjectives { get { return this.allobjectives; } }

        /// <summary>
        /// Backing store of the current WvW Objectives collection
        /// </summary>
        private ObservableCollection<WvWObjectiveViewModel> currentObjectives = new ObservableCollection<WvWObjectiveViewModel>();

        /// <summary>
        /// The collection of current WvW Objectives
        /// </summary>
        public ObservableCollection<WvWObjectiveViewModel> CurrentObjectives { get { return this.currentObjectives; } }

        /// <summary>
        /// Backing store of the WvW Notifications collection
        /// </summary>
        private ObservableCollection<WvWObjectiveViewModel> wvwNotifications = new ObservableCollection<WvWObjectiveViewModel>();

        /// <summary>
        /// The collection of WvW Objective Notifications
        /// </summary>
        public ObservableCollection<WvWObjectiveViewModel> WvWNotifications { get { return this.wvwNotifications; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dungeonsService">The dungeons service object</param>
        /// <param name="userSettings">The dungeons user settings object</param>
        public WvWController(IWvWService wvwService, IPlayerService playerService, IHasWvWMap mapObj, WvWSettings userSettings)
        {
            logger.Debug("Initializing WvW Controller");
            this.wvwService = wvwService;
            this.playerService = playerService;
            this.mapObj = mapObj;
            this.userSettings = userSettings;
            this.timerCount = 0;

            // Initialize the refresh timer
            this.objectivesRefreshTimer = new Timer(this.RefreshObjectives);
            this.ObjectivesRefreshInterval = 500;

            // Initialize the start call count to 0
            this.startCallCount = 0;

            // Initialize the collections
            this.wvwService.LoadTable();
            this.InitializeTeams();
            this.InitializeAllObjectivesCollection();

            logger.Info("WvW Controller initialized");
        }

        /// <summary>
        /// Starts the automatic refresh
        /// </summary>
        public void Start()
        {
            logger.Debug("Start called");
            Task.Factory.StartNew(() =>
            {
                // Start the timer if this is the first time that Start() has been called
                if (this.startCallCount == 0)
                {
                    logger.Debug("Starting refresh timers");
                    this.RefreshObjectives();
                }

                this.startCallCount++;
                logger.Debug("startCallCount = {0}", this.startCallCount);

            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Stops the automatic refresh
        /// </summary>
        public void Stop()
        {
            this.startCallCount--;
            logger.Debug("Stop called - startCallCount = {0}", this.startCallCount);

            // Stop the refresh timer if all calls to Start() have had a matching call to Stop()
            if (this.startCallCount == 0)
            {
                logger.Debug("Stopping refresh timers");
                lock (this.objectivesRefreshTimerLock)
                {
                    this.objectivesRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Initializes the WvW teams collection
        /// </summary>
        private void InitializeTeams()
        {
            foreach (var world in this.wvwService.Worlds.Worlds)
            {
                var team = new WvWTeamViewModel(world);
                team.MatchId = this.wvwService.GetMatchId(team.WorldId);
                team.Color = this.wvwService.GetTeamColor(team.WorldId);
                //team.Score = this.wvwService.GetWorldScore(team.WorldId); // This really slows things down... disabled for now
                Threading.InvokeOnUI(() => this.Teams.Add(team));
            }
        }

        /// <summary>
        /// Initializes the All Objectives collection
        /// </summary>
        private void InitializeAllObjectivesCollection()
        {
            logger.Debug("Initializing objectives");

            // Determine the current match. If this changes, we don't need to re-initialize since the actual objectives don't change - just the owners change
            var matchID = this.wvwService.GetMatchId(this.UserSettings.WorldSelection.ID);
            var objectives = this.wvwService.GetAllObjectives(matchID);

            Threading.InvokeOnUI(() =>
            {
                foreach (var obj in objectives)
                {
                    logger.Debug("Initializing view model for {0} - {1}", obj.Name, obj.Map);
                    var vm = new WvWObjectiveViewModel(obj, this.UserSettings, this.Teams, this.WvWNotifications);
                    this.AllObjectives.Add(vm);
                }
            });
        }

        /// <summary>
        /// Rebuilds the current objectives collections
        /// </summary>
        private void RebuildCurrentObjectivesCollection(WvWMap map)
        {
            logger.Debug("Building objectives collection");

            Threading.InvokeOnUI(() =>
            {
                this.CurrentObjectives.Clear();
                foreach (var objective in this.AllObjectives.Where(obj => obj.Map == map))
                {
                    this.CurrentObjectives.Add(objective);
                }
            });
        }

        /// <summary>
        /// Refreshes all objectives within the objectives collection
        /// This is the primary function of the WvWController
        /// </summary>
        private void RefreshObjectives(object state = null)
        {
            lock (this.objectivesRefreshTimerLock)
            {
                var matchID = this.wvwService.GetMatchId(this.UserSettings.WorldSelection.ID);
                if (matchID == null)
                {
                    // Unable to retrieve the current match ID, which means a reset is probably occuring
                    // When this happens, clear out the state of everything
                    Threading.InvokeOnUI(() =>
                        {
                            foreach (var objective in this.AllObjectives)
                            {
                                objective.PrevWorldOwner = WorldColor.None;
                                objective.WorldOwner = WorldColor.None;
                                objective.FlipTime = DateTime.UtcNow;
                                objective.DistanceFromPlayer = 0;
                                objective.TimerValue = TimeSpan.Zero;
                                objective.IsRIActive = false;
                            }
                        });
                }
                else
                {

                    if (this.matchID != matchID)
                    {
                        logger.Info("Match change detected: new matchID = {0}", matchID);
                        this.matchID = matchID;

                        // Refresh state of all objectives
                        var latestObjectivesData = this.wvwService.GetAllObjectives(matchID);
                        foreach (var objective in this.AllObjectives)
                        {
                            var latestData = latestObjectivesData.First(obj => obj.ID == objective.ID);
                            Threading.InvokeOnUI(() =>
                            {
                                objective.ModelData.MatchId = this.matchID;
                                objective.PrevWorldOwner = latestData.WorldOwner;
                                objective.WorldOwner = latestData.WorldOwner;
                                objective.FlipTime = DateTime.UtcNow;
                                objective.DistanceFromPlayer = 0;
                                objective.TimerValue = TimeSpan.Zero;
                                objective.IsRIActive = false;
                            });
                        }
                    }
                    else
                    {
                        // Check for new WvW Map
                        if (this.MapOverride != WvWMap.Unknown)
                        {
                            if (this.MapOverride != this.prevMap)
                            {
                                // Map changed, rebuild the objectives
                                this.prevMap = this.MapOverride;
                                this.mapObj.Map = this.MapOverride;
                                this.RebuildCurrentObjectivesCollection(this.MapOverride);
                            }
                        }
                        else
                        {
                            if (this.PlayerMap != this.prevMap)
                            {
                                // Map changed, rebuild the objectives
                                this.prevMap = this.PlayerMap;
                                this.mapObj.Map = this.PlayerMap;
                                this.RebuildCurrentObjectivesCollection(this.PlayerMap);
                            }
                        }

                        // Refresh state of all objectives
                        // Do this only once every 2 seconds
                        this.timerCount++;
                        if (this.timerCount >= 4) // 500ms * 4 = 2seconds
                        {
                            this.timerCount = 0;
                            var latestObjectivesData = this.wvwService.GetAllObjectives(matchID);
                            if (latestObjectivesData.Count() > 0)
                            {
                                foreach (var objective in this.AllObjectives)
                                {
                                    var latestData = latestObjectivesData.First(obj => obj.ID == objective.ID);

                                    if (objective.WorldOwner != latestData.WorldOwner)
                                    {
                                        // New owner
                                        Threading.InvokeOnUI(() =>
                                            {
                                                objective.PrevWorldOwner = objective.WorldOwner;
                                                objective.WorldOwner = latestData.WorldOwner;

                                                // Bloodlust objectives don't get RI, so don't bother with a flip time or RI flag
                                                if (objective.Type != ObjectiveType.TempleofLostPrayers
                                                    && objective.Type != ObjectiveType.BattlesHollow
                                                    && objective.Type != ObjectiveType.BauersEstate
                                                    && objective.Type != ObjectiveType.OrchardOverlook
                                                    && objective.Type != ObjectiveType.CarversAscent)
                                                {
                                                    objective.FlipTime = DateTime.UtcNow;
                                                    objective.IsRIActive = true;
                                                }
                                            });

                                        if (objective.WorldOwner != WorldColor.None) // Don't show a notification if the new owner is "none"
                                        {
                                            // Owner just changed, raise a notification!
                                            this.DisplayNotification(objective);
                                        }
                                    }
                                }
                            }
                        }

                        // Refresh timers
                        foreach (var objective in this.AllObjectives)
                        {
                            var timeSinceFlip = DateTime.UtcNow - objective.FlipTime;
                            if (timeSinceFlip <= TimeSpan.FromMinutes(5))
                            {
                                var countdownTime = TimeSpan.FromMinutes(5) - timeSinceFlip;
                                Threading.InvokeOnUI(() => objective.TimerValue = countdownTime);
                            }
                            else
                            {
                                Threading.InvokeOnUI(() => objective.IsRIActive = false);
                            }
                        }

                        if (this.UserSettings.AreTimeDistancesShown) // Don't bother if we aren't showing these
                        {
                            // Calculate time distances for all objectives, based on the player's position, if the player is in the same map as the objective
                            // Note: these are approximations at best
                            var playerPosition = CalcUtil.ConvertToMapPosition(this.playerService.PlayerPosition);
                            foreach (var objective in this.CurrentObjectives)
                            {
                                if (this.PlayerMap == objective.Map)
                                {
                                    if (playerPosition != null && objective.ModelData.MapLocation != null)
                                    {
                                        objective.DistanceFromPlayer = Math.Round(CalcUtil.CalculateDistance(playerPosition, objective.ModelData.MapLocation, this.UserSettings.DistanceUnits));
                                    }
                                }
                                else
                                {
                                    objective.DistanceFromPlayer = 0;
                                }
                            }
                        }
                    }
                }

                this.objectivesRefreshTimer.Change(this.ObjectivesRefreshInterval, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Adds an objective to the notifications collection, and then removes the objective 10 seconds later
        /// </summary>
        private void DisplayNotification(WvWObjectiveViewModel objectiveData)
        {
            if (this.CanShowNotification(objectiveData))
            {
                Task.Factory.StartNew(() =>
                {
                    logger.Debug("Adding notification for \"{0}\" in {1}", objectiveData.Name, objectiveData.Map);
                    Threading.InvokeOnUI(() => this.WvWNotifications.Add(objectiveData));

                    // For 5 seconds, loop and sleep, with checks to see if notifications have been disabled
                    for (int i = 0; i < 20; i++)
                    {
                        System.Threading.Thread.Sleep(250);
                        if (!this.CanShowNotification(objectiveData))
                        {
                            logger.Debug("Removing notification for \"{0}\" in {1}", objectiveData.Name, objectiveData.Map);
                            Threading.InvokeOnUI(() => this.WvWNotifications.Remove(objectiveData));
                        }
                    }

                    logger.Debug("Removing notification for \"{0}\" in {1}", objectiveData.Name, objectiveData.Map);

                    // TODO: I hate having this here, but due to a limitation in WPF, there's no reasonable way around this at this time
                    // This makes it so that the notifications can fade out before they are removed from the notification window
                    Threading.InvokeOnUI(() => objectiveData.IsRemovingNotification = true);
                    System.Threading.Thread.Sleep(250);
                    Threading.InvokeOnUI(() =>
                    {
                        this.WvWNotifications.Remove(objectiveData);
                        objectiveData.IsRemovingNotification = false;
                    });
                }, TaskCreationOptions.LongRunning);
            }
        }

        /// <summary>
        /// Determines if we can show a notification for the given objective, based on user settings
        /// </summary>
        /// <param name="objectiveData">The objective's data</param>
        /// <returns>True if the notification can be shown, else false</returns>
        private bool CanShowNotification(WvWObjectiveViewModel objectiveData)
        {
            switch (objectiveData.Map)
            {
                case WvWMap.BlueBorderlands:
                    return this.UserSettings.AreBlueBorderlandsNotificationsEnabled;
                case WvWMap.GreenBorderlands:
                    return this.UserSettings.AreGreenBorderlandsNotificationsEnabled;
                case WvWMap.RedBorderlands:
                    return this.UserSettings.AreRedBorderlandsNotificationsEnabled;
                case WvWMap.EternalBattlegrounds:
                    return this.UserSettings.AreEternalBattlegroundsNotificationsEnabled;
                default:
                    return false;
            }
        }
    }
}
