using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Enums;
using GW2PAO.Models;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.ViewModels
{
    public class WvWObjectiveViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IEnumerable<WvWTeamViewModel> wvwTeams;
        private ICollection<WvWObjectiveViewModel> displayedNotificationsCollection;
        private WorldColor prevWorldOwner;
        private DateTime flipTime;
        private TimeSpan timerValue;
        private double distanceFromPlayer;
        private bool isVisible;
        private bool isRIActive;
        private bool isNotificationShown;
        private bool isRemovingNotification;
        private WvWSettings userSettings;

        /// <summary>
        /// The primary backing model data for the viewmodel
        /// </summary>
        public WvWObjective ModelData { get; private set; }

        /// <summary>
        /// ID of the objective
        /// </summary>
        public int ID { get { return this.ModelData.ID; } }

        /// <summary>
        /// The type of the objective
        /// </summary>
        public ObjectiveType Type { get { return this.ModelData.Type; } }

        /// <summary>
        /// Current owner of the objective
        /// </summary>
        public WorldColor WorldOwner
        {
            get { return this.ModelData.WorldOwner; }
            set
            {
                if (this.ModelData.WorldOwner != value)
                {
                    this.ModelData.WorldOwner = value;
                    this.RaisePropertyChanged();

                    // Also refresh the world owner's name, and visiblity
                    this.RaisePropertyChanged("WorldOwnerName");
                    this.RefreshVisibility();
                }
            }
        }

        /// <summary>
        /// Current world owner's name
        /// </summary>
        public string WorldOwnerName
        {
            get
            {
                if (this.WorldOwner != WorldColor.None)
                {
                    // Find the team
                    var team = this.wvwTeams.First(wrld => wrld.MatchId == this.ModelData.MatchId
                                                         && wrld.Color == this.WorldOwner);
                    // Return the world name
                    return team.WorldName;
                }
                else
                {
                    return "N/A";
                }
            }
        }

        /// <summary>
        /// Current owner of the objective
        /// </summary>
        public WorldColor PrevWorldOwner
        {
            get { return this.prevWorldOwner; }
            set
            {
                if (this.SetField(ref this.prevWorldOwner, value))
                {
                    // Also refresh the world owner's name
                    this.RaisePropertyChanged("PrevWorldOwnerName");
                }
            }
        }

        /// <summary>
        /// Current world owner's name
        /// </summary>
        public string PrevWorldOwnerName
        {
            get
            {
                if (this.PrevWorldOwner != WorldColor.None)
                {
                    // Find the team
                    var team = this.wvwTeams.First(wrld => wrld.MatchId == this.ModelData.MatchId
                                                         && wrld.Color == this.PrevWorldOwner);
                    // Return the world name
                    return team.WorldName;
                }
                else
                {
                    return "N/A";
                }
            }
        }

        /// <summary>
        /// Name of the objective
        /// </summary>
        public string Name { get { return this.ModelData.FullName; } }

        /// <summary>
        /// Short Name (or type) of the objective
        /// </summary>
        public string ShortName { get { return this.ModelData.Name; } }

        /// <summary>
        /// Textual location of the objective
        /// </summary>
        public string Location { get { return this.ModelData.Location; } }

        /// <summary>
        /// Map location of the objective
        /// </summary>
        public WvWMap Map { get { return this.ModelData.Map; } }

        /// <summary>
        /// The value of this objective in points
        /// </summary>
        public int Points { get { return this.ModelData.Points; } }

        /// <summary>
        /// Most-recent flip time for the event, in UTC
        /// </summary>
        public DateTime FlipTime
        {
            get { return this.flipTime; }
            set { SetField(ref this.flipTime, value); }
        }

        /// <summary>
        /// Distance of the objective from the player
        /// Units depend on user selection
        /// </summary>
        public double DistanceFromPlayer
        {
            get { return this.distanceFromPlayer; }
            set { SetField(ref this.distanceFromPlayer, value); }
        }

        /// <summary>
        /// Countdown timer used for Indignation Buff tracking
        /// </summary>
        public TimeSpan TimerValue
        {
            get { return this.timerValue; }
            set { SetField(ref this.timerValue, value); }
        }

        /// <summary>
        /// The WvW user settings
        /// </summary>
        public WvWSettings UserSettings { get { return this.userSettings; } }

        /// <summary>
        /// Visibility of the objective
        /// Visibility is based on multiple properties, including:
        ///     - Type and the user configuration for what objective types are shown
        ///     - Owner and the user configuration for what owner colors are shown
        ///     - Whether or not the event is user-configured as hidden
        /// </summary>
        public bool IsVisible
        {
            get { return this.isVisible; }
            set { SetField(ref this.isVisible, value); }
        }

        /// <summary>
        /// True if RI is active, else faluse
        /// </summary>
        public bool IsRIActive
        {
            get { return this.isRIActive; }
            set { SetField(ref this.isRIActive, value); }
        }

        /// <summary>
        /// True if the notification for this event has already been shown, else false
        /// </summary>
        public bool IsNotificationShown
        {
            get { return this.isNotificationShown; }
            set { SetField(ref this.isNotificationShown, value); }
        }

        /// <summary>
        /// True if the notification for this event is about to be removed, else false
        /// TODO: I hate having this here, but due to a limitation in WPF, there's no reasonable way around this at this time
        /// </summary>
        public bool IsRemovingNotification
        {
            get { return this.isRemovingNotification; }
            set { SetField(ref this.isRemovingNotification, value); }
        }

        /// <summary>
        /// Command to hide the objective
        /// </summary>
        public DelegateCommand HideCommand { get { return new DelegateCommand(this.AddToHiddenObjectives); } }

        /// <summary>
        /// Closes the displayed notification
        /// </summary>
        public DelegateCommand CloseNotificationCommand { get { return new DelegateCommand(this.CloseNotification); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="objective">The objective details</param>
        /// <param name="wvwTeams">Collection containing all of the WvW Teams</param>
        public WvWObjectiveViewModel(WvWObjective objective, WvWSettings userSettings, IEnumerable<WvWTeamViewModel> wvwTeams, ICollection<WvWObjectiveViewModel> displayedNotificationsCollection)
        {
            this.ModelData = objective;
            this.userSettings = userSettings;
            this.wvwTeams = wvwTeams;
            this.displayedNotificationsCollection = displayedNotificationsCollection;

            this.PrevWorldOwner = WorldColor.None;
            this.FlipTime = DateTime.UtcNow;
            this.TimerValue = TimeSpan.Zero;
            this.DistanceFromPlayer = 0.0;
            this.IsRIActive = false;
            this.IsNotificationShown = false;
            this.IsRemovingNotification = false;

            this.userSettings.PropertyChanged += (o, e) => this.RefreshVisibility();
            this.userSettings.HiddenObjectives.CollectionChanged += (o, e) => this.RefreshVisibility();
            this.RefreshVisibility();
        }

        /// <summary>
        /// Adds the event to the list of hidden events
        /// </summary>
        private void AddToHiddenObjectives()
        {
            logger.Debug("Adding \"{0}\" to hidden objectives", this.Name);
            this.userSettings.HiddenObjectives.Add(this.ID);
        }

        /// <summary>
        /// Refreshes the visibility of the event
        /// </summary>
        private void RefreshVisibility()
        {
            logger.Trace("Refreshing visibility of \"{0}\"", this.Name);
            if (this.userSettings.HiddenObjectives.Any(id => id == this.ID))
            {
                this.IsVisible = false;
            }
            else if (!this.userSettings.AreRedObjectivesShown && this.WorldOwner == WorldColor.Red)
            {
                this.IsVisible = false;
            }
            else if (!this.userSettings.AreGreenObjectivesShown && this.WorldOwner == WorldColor.Green)
            {
                this.IsVisible = false;
            }
            else if (!this.userSettings.AreBlueObjectivesShown && this.WorldOwner == WorldColor.Blue)
            {
                this.IsVisible = false;
            }
            else if (!this.userSettings.AreNeutralObjectivesShown && this.WorldOwner == WorldColor.None)
            {
                this.IsVisible = false;
            }
            else if (!this.userSettings.AreCastlesShown && this.Type == ObjectiveType.Castle)
            {
                this.IsVisible = false;
            }
            else if (!this.userSettings.AreKeepsShown && this.Type == ObjectiveType.Keep)
            {
                this.IsVisible = false;
            }
            else if (!this.userSettings.AreTowersShown && this.Type == ObjectiveType.Tower)
            {
                this.IsVisible = false;
            }
            else if (!this.userSettings.AreCampsShown && this.Type == ObjectiveType.Camp)
            {
                this.IsVisible = false;
            }
            else if (!this.userSettings.AreBloodlustObjectivesShown &&
                        (this.Type == ObjectiveType.TempleofLostPrayers
                         || this.Type == ObjectiveType.BattlesHollow
                         || this.Type == ObjectiveType.BauersEstate
                         || this.Type == ObjectiveType.OrchardOverlook
                         || this.Type == ObjectiveType.CarversAscent) )
            {
                this.IsVisible = false;
            }
            else
            {
                this.IsVisible = true;
            }
            logger.Trace("IsVisible = {0}", this.IsVisible);
        }

        /// <summary>
        /// Removes this objective from the collection of displayed notifications
        /// </summary>
        private void CloseNotification()
        {
            this.displayedNotificationsCollection.Remove(this);
        }
    }
}
