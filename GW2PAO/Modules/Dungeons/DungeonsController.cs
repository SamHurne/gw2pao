using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.API.Util;
using GW2PAO.Data.UserData;
using GW2PAO.Modules.Dungeons.Interfaces;
using GW2PAO.Modules.Dungeons.ViewModels;
using GW2PAO.Modules.Dungeons.ViewModels.DungeonTimer;
using GW2PAO.Modules.WebBrowser.Interfaces;
using GW2PAO.Utility;
using NLog;

namespace GW2PAO.Modules.Dungeons
{
    [Export(typeof(IDungeonsController))]
    public class DungeonsController : IDungeonsController
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The dungeons service object
        /// </summary>
        private IDungeonsService dungeonsService;

        /// <summary>
        /// The zone service object
        /// </summary>
        private IZoneService zoneService;

        /// <summary>
        /// Service responsible for providing player information via the mumble interface
        /// </summary>
        private IPlayerService playerService;

        /// <summary>
        /// Browser controller. Currently just passed through to DungeonViewModels
        /// </summary>
        private IWebBrowserController browserController;

        /// <summary>
        /// Keeps track of how many times Start() has been called in order
        /// to support reuse of a single object
        /// </summary>
        private int startCallCount;

        /// <summary>
        /// True if the controller's timers are no longer running, else false
        /// </summary>
        private bool isStopped;

        /// <summary>
        /// The primary reset timer object
        /// </summary>
        private Timer dungeonsRefreshTimer;

        /// <summary>
        /// Locking object for operations performed with the reset timer
        /// </summary>
        private readonly object refreshTimerLock = new object();

        /// <summary>
        /// User settings for dungeons
        /// </summary>
        private DungeonsUserData userData;

        /// <summary>
        /// The current map ID for the player
        /// </summary>
        private int currentMapId;

        /// <summary>
        /// The interval by which to refresh the dungeon reset state (in ms)
        /// </summary>
        public int RefreshInterval { get; set; }

        /// <summary>
        /// The dungeon user settings
        /// </summary>
        public DungeonsUserData UserData { get { return this.userData; } }

        /// <summary>
        /// Backing store of the Dungeons collection
        /// </summary>
        private ObservableCollection<DungeonViewModel> dungeons = new ObservableCollection<DungeonViewModel>();

        /// <summary>
        /// The collection of Dungeons
        /// </summary>
        public ObservableCollection<DungeonViewModel> Dungeons { get { return this.dungeons; } }

        /// <summary>
        /// View model containing timer information
        /// </summary>
        [Export]
        public DungeonTimerViewModel DungeonTimerData
        {
            get;
            private set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dungeonsService">The dungeons service object</param>
        /// <param name="userData">The dungeons user data object</param>
        [ImportingConstructor]
        public DungeonsController(IDungeonsService dungeonsService, IZoneService zoneService, IPlayerService playerService, IWebBrowserController browserController, DungeonsUserData userData)
        {
            logger.Debug("Initializing Dungeons Controller");
            this.dungeonsService = dungeonsService;
            this.zoneService = zoneService;
            this.playerService = playerService;
            this.browserController = browserController;
            this.userData = userData;
            this.isStopped = false;

            // Initialize the dungeon timer view model
            this.DungeonTimerData = new DungeonTimerViewModel();

            // Initialize the refresh timer
            this.dungeonsRefreshTimer = new Timer(this.Refresh);
            this.RefreshInterval = 250;

            // Initialize the start call count to 0
            this.startCallCount = 0;

            // Initialize the dungeons
            this.InitializeDungeons();

            logger.Info("Dungeons Controller initialized");
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
                    this.isStopped = false;
                    logger.Debug("Starting refresh timers");
                    this.Refresh();
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
                lock (this.refreshTimerLock)
                {
                    this.isStopped = true;
                    this.dungeonsRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Forces a shutdown of the controller, including all running timers/threads
        /// </summary>
        public void Shutdown()
        {
            logger.Debug("Shutdown called");
            logger.Debug("Stopping refresh timers");
            lock (this.refreshTimerLock)
            {
                this.isStopped = true;
                this.dungeonsRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Initializes the collection of dungeons
        /// </summary>
        private void InitializeDungeons()
        {
            logger.Debug("Initializing dungeons");
            this.dungeonsService.LoadTable();
            foreach (var dungeon in this.dungeonsService.DungeonsTable.Dungeons)
            {
                logger.Debug("Initializing localized strings for {0}", dungeon.ID);
                dungeon.Name = this.dungeonsService.GetLocalizedName(dungeon.ID);
                dungeon.MapName = this.zoneService.GetZoneName(dungeon.WorldMapID);

                foreach (var path in dungeon.Paths)
                {
                    path.Nickname = this.dungeonsService.GetLocalizedName(path.ID);
                }

                Threading.InvokeOnUI(() =>
                {
                    logger.Debug("Initializing view model for {0}", dungeon.Name);
                    this.Dungeons.Add(new DungeonViewModel(dungeon, this.browserController, this.userData));
                });

                logger.Debug("Initializing path times for {0}", dungeon.Name);
                foreach (var dung in this.Dungeons)
                {
                    foreach (var path in dung.Paths)
                    {
                        var existingPathTimeData = this.UserData.BestPathTimes.FirstOrDefault(pt => pt.PathID == path.PathId);
                        if (existingPathTimeData == null)
                        {
                            this.UserData.BestPathTimes.Add(new Data.PathTime(path));
                        }
                        else
                        {
                            existingPathTimeData.PathData = path;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes all dungeons within the dungeons collection
        /// and also updates player information
        /// This is the primary function of the DungeonsController
        /// </summary>
        private void Refresh(object state = null)
        {
            lock (this.refreshTimerLock)
            {
                if (this.isStopped)
                    return; // Immediately return if we are supposed to be stopped

                // Refresh state of path completions
                if (DateTime.UtcNow.Date.CompareTo(this.userData.LastResetDateTime.Date) != 0)
                {
                    logger.Info("Resetting path completions state");
                    this.userData.LastResetDateTime = DateTime.UtcNow;
                    Threading.BeginInvokeOnUI(() =>
                    {
                        foreach (var dungeon in this.Dungeons)
                        {
                            foreach (var path in dungeon.Paths)
                            {
                                path.IsCompleted = false;
                            }
                        }
                    });
                }

                // Check if the player is in a dungeon map
                if (this.playerService.HasValidMapId)
                {
                    bool mapChanged = this.currentMapId != this.playerService.MapId;
                    if (mapChanged)
                    {
                        logger.Info("Map change detected - Previous: {0} - New: {1}", this.currentMapId, this.playerService.MapId);
                        this.currentMapId = this.playerService.MapId;

                        // Reset current dungeon/path information
                        this.DungeonTimerData.CurrentDungeon = null;
                        this.DungeonTimerData.CurrentPath = null;

                        if (this.DungeonTimerData.IsTimerRunning
                            && this.UserData.AutoStopDungeonTimer)
                        {
                            // If enabled, stop the timer when the user changes map
                            logger.Info("Pausing dungeon timer - Timer: {0}", this.DungeonTimerData.TimerValue);
                            this.DungeonTimerData.PauseTimer();

                            // Save the dungeon time if the player completed the dungeon
                            if (this.DungeonTimerData.CurrentPath != null
                                && this.DungeonTimerData.CurrentPath.CompletionPrerequisitePoints.Values.All(preReqMet => preReqMet == true)
                                && this.DungeonTimerData.CurrentPath.IsCompleted)
                            {
                                var bestPathTime = this.UserData.BestPathTimes.FirstOrDefault(pt => pt.PathID == this.DungeonTimerData.CurrentPath.PathId);
                                if (this.DungeonTimerData.TimerValue.CompareTo(bestPathTime.Time) < 0)
                                {
                                    logger.Info("New best time for {0} ({1}) detected: {2}", bestPathTime.PathID, bestPathTime.PathData.DisplayName, this.DungeonTimerData.TimerValue);
                                    bestPathTime.Time = this.DungeonTimerData.TimerValue;
                                    bestPathTime.Timestamp = DateTime.Now;
                                }
                            }
                        }
                    }

                    // Determine if the current map is a dungeon map
                    var dungeonVm = this.GetDungeonViewModel(this.currentMapId);
                    Threading.InvokeOnUI(() => this.DungeonTimerData.CurrentDungeon = dungeonVm);
                    if (this.DungeonTimerData.CurrentDungeon != null)
                    {
                        // Yes, this is a dungeon map
                        logger.Trace("Dungeon map detected: {0}", dungeonVm.DungeonName);

                        // If map just changed, we just entered the dungeon, so start the timer if auto-start is turned on
                        if (mapChanged && this.UserData.AutoStartDungeonTimer)
                        {
                            Threading.InvokeOnUI(() => 
                                {
                                    logger.Info("Starting dungeon timer");
                                    this.DungeonTimerData.StopTimer();
                                    this.DungeonTimerData.StartTimer();
                                });
                        }

                        // Loop through the dungeon paths and see if we know what path we are in
                        if (this.DungeonTimerData.CurrentPath == null)
                        {
                            foreach (var path in dungeonVm.Paths)
                            {
                                if (this.IsPlayerInPath(path))
                                {
                                    logger.Trace("Dungeon path detected: {0}", path.DisplayName);
                                    Threading.InvokeOnUI(() => this.DungeonTimerData.CurrentPath = path);
                                    break;
                                }
                            }
                        }

                        if (this.DungeonTimerData.CurrentPath != null)
                        {
                            // The current path is known, so monitor the player position to mark any pre-requisite points as met
                            foreach (var preReq in this.DungeonTimerData.CurrentPath.CompletionPrerequisitePoints.Keys)
                            {
                                if (this.IsPlayerNear(preReq, this.DungeonTimerData.CurrentPath.PointDetectionRadius))
                                {
                                    this.DungeonTimerData.CurrentPath.CompletionPrerequisitePoints[preReq] = true;
                                }
                            }

                            if (!this.DungeonTimerData.CurrentPath.IsCompleted && this.UserData.AutoCompleteDungeons)
                            {
                                // If the player has reached the end of the path and met all pre-requisite points,
                                // mark that path as completed, if enabled
                                if (this.IsPlayerNear(this.DungeonTimerData.CurrentPath.EndPoint, this.DungeonTimerData.CurrentPath.PointDetectionRadius))
                                {
                                    if (this.DungeonTimerData.CurrentPath.CompletionPrerequisitePoints.Values.All(preReqMet => preReqMet == true))
                                    {
                                        logger.Trace("End of path reached, marking as completed");
                                        this.DungeonTimerData.CurrentPath.IsCompleted = true;
                                    }
                                }
                            }
                        }
                    }
                }

                //logger.Debug("Player Location = {0}", this.playerService.PlayerPosition);
                this.dungeonsRefreshTimer.Change(this.RefreshInterval, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Retrieves the current path view model for the give map ID, if the map ID matches a dungeon path
        /// </summary>
        /// <param name="mapId">The map ID of the dungeon to retrieve</param>
        /// <returns>The dungeon matching the given map ID, else null if there is no match</returns>
        private DungeonViewModel GetDungeonViewModel(int mapId)
        {
            foreach (var dungeon in this.Dungeons)
            {
                foreach (var path in dungeon.DungeonModel.Paths)
                {
                    if (mapId == path.InstanceMapID)
                        return dungeon;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines if the player is in the given path
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>True if the player is in the given path, else false</returns>
        private bool IsPlayerInPath(PathViewModel path)
        {
            if (path.PathModel.IdentifyingPoints.Count > 0)
            {
                bool isInRange = false;
                foreach (var point in path.PathModel.IdentifyingPoints)
                {
                    isInRange |= CalcUtil.IsInRadius(point, this.playerService.PlayerPosition, path.PathModel.PointDetectionRadius);
                }
                return isInRange;
            }
            else
            {
                // No identifying points, so use the mapID only
                return this.playerService.MapId == path.PathModel.InstanceMapID;
            }
        }

        /// <summary>
        /// Determines if the player is positioned near a given point
        /// </summary>
        /// <param name="point">The point to test</param>
        /// <param name="detectionRadius">Radius to use during detection</param>
        /// <returns>True if the player has reached the end of the given path, else false</returns>
        private bool IsPlayerNear(Point point, double detectionRadius)
        {
            if (point != null)
            {
                return CalcUtil.IsInRadius(point, this.playerService.PlayerPosition, detectionRadius);
            }
            else
            {
                return false;
            }
        }
    }
}
