using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Properties;

namespace GW2PAO.Modules.WvW.ViewModels
{
    public class WvWObjectiveViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private ICollection<WvWTeamViewModel> wvwTeams;
        private ICollection<WvWObjectiveViewModel> displayedNotificationsCollection;
        private WorldColor prevWorldOwner;
        private DateTime flipTime;
        private TimeSpan timerValue;
        private double distanceFromPlayer;
        private bool isVisible;
        private bool isRIActive;
        private bool isNotificationShown;
        private bool isRemovingNotification;
        private WvWUserData userData;

        /// <summary>
        /// The primary backing model data for the viewmodel
        /// </summary>
        public WvWObjective ModelData { get; private set; }

        /// <summary>
        /// ID of the objective
        /// </summary>
        public WvWObjectiveId ID { get { return this.ModelData.ID; } }

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
                    this.OnPropertyChanged(() => this.WorldOwner);

                    // Also refresh the world owner's name, and visiblity
                    this.OnPropertyChanged(() => this.WorldOwnerName);
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
                    var team = this.wvwTeams.FirstOrDefault(wrld => wrld.MatchId == this.ModelData.MatchId
                                                                 && wrld.Color == this.WorldOwner);
                    if (team != null)
                    {
                        // Return the world name
                        return team.WorldName;
                    }
                }

                // else
                return "N/A";
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
                if (this.SetProperty(ref this.prevWorldOwner, value))
                {
                    // Also refresh the world owner's name
                    this.OnPropertyChanged(() => this.PrevWorldOwnerName);
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
                    var team = this.wvwTeams.FirstOrDefault(wrld => wrld.MatchId == this.ModelData.MatchId
                                                                 && wrld.Color == this.PrevWorldOwner);
                    if (team != null)
                    {
                        // Return the world name
                        return team.WorldName;
                    }
                }

                // else
                return "N/A";
            }
        }

        /// <summary>
        /// The Guild that has claimed this objective
        /// </summary>
        public GuildViewModel GuildClaimer
        {
            get;
            set;
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
            set { SetProperty(ref this.flipTime, value); }
        }

        /// <summary>
        /// Distance of the objective from the player
        /// Units depend on user selection
        /// </summary>
        public double DistanceFromPlayer
        {
            get { return this.distanceFromPlayer; }
            set { SetProperty(ref this.distanceFromPlayer, value); }
        }

        /// <summary>
        /// Countdown timer used for Indignation Buff tracking
        /// </summary>
        public TimeSpan TimerValue
        {
            get { return this.timerValue; }
            set { SetProperty(ref this.timerValue, value); }
        }

        /// <summary>
        /// The WvW user settings
        /// </summary>
        public WvWUserData UserData { get { return this.userData; } }

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
            set { SetProperty(ref this.isVisible, value); }
        }

        /// <summary>
        /// True if RI is active, else faluse
        /// </summary>
        public bool IsRIActive
        {
            get { return this.isRIActive; }
            set { SetProperty(ref this.isRIActive, value); }
        }

        /// <summary>
        /// True if the notification for this event has already been shown, else false
        /// </summary>
        public bool IsNotificationShown
        {
            get { return this.isNotificationShown; }
            set { SetProperty(ref this.isNotificationShown, value); }
        }

        /// <summary>
        /// True if the notification for this event is about to be removed, else false
        /// TODO: I hate having this here, but due to a limitation in WPF, there's no reasonable way around this at this time
        /// </summary>
        public bool IsRemovingNotification
        {
            get { return this.isRemovingNotification; }
            set { SetProperty(ref this.isRemovingNotification, value); }
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
        /// Command to copy the chat code for this objective to the clipboard
        /// </summary>
        public DelegateCommand CopyChatCodeCommand { get { return new DelegateCommand(this.CopyChatCode, this.CanCopyChatCode); } }

        /// <summary>
        /// Command to copy the "'x' is under attack!" text to the clipboard
        /// </summary>
        public DelegateCommand CopyUnderAttackTextCommand { get { return new DelegateCommand(this.CopyUnderAttackText); } }

        /// <summary>
        /// Command to copy the "Enemy headed to 'x'" text to the clipboard
        /// </summary>
        public DelegateCommand CopyEnemyHeadedToTextCommand { get { return new DelegateCommand(this.CopyEnemyHeadedToText); } }

        /// <summary>
        /// Command to copy the "I'm headed to 'x'" text to the clipboard
        /// </summary>
        public DelegateCommand CopyPlayerHeadedToTextCommand { get { return new DelegateCommand(this.CopyPlayerHeadedToText); } }

        /// <summary>
        /// Command to copy the "x: RI 'xx:xx:xx'" or "x: RI Not Active" text to the clipboard
        /// </summary>
        public DelegateCommand CopyRITextCommand { get { return new DelegateCommand(this.CopyRIText); } }

        /// <summary>
        /// Command to copy the information about the event to the clipboard
        /// </summary>
        public DelegateCommand CopyGeneralDataCommand { get { return new DelegateCommand(this.CopyGeneralData); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="objective">The objective details</param>
        /// <param name="wvwTeams">Collection containing all of the WvW Teams</param>
        public WvWObjectiveViewModel(WvWObjective objective, WvWUserData userData, ICollection<WvWTeamViewModel> wvwTeams, ICollection<WvWObjectiveViewModel> displayedNotificationsCollection)
        {
            this.ModelData = objective;
            this.userData = userData;
            this.wvwTeams = wvwTeams;
            this.displayedNotificationsCollection = displayedNotificationsCollection;

            this.PrevWorldOwner = WorldColor.None;
            this.FlipTime = DateTime.UtcNow;
            this.TimerValue = TimeSpan.Zero;
            this.DistanceFromPlayer = 0.0;
            this.IsRIActive = false;
            this.IsNotificationShown = false;
            this.IsRemovingNotification = false;
            this.GuildClaimer = new GuildViewModel();

            this.userData.PropertyChanged += (o, e) => this.RefreshVisibility();
            this.userData.HiddenObjectives.CollectionChanged += (o, e) => this.RefreshVisibility();
            this.RefreshVisibility();
        }

        /// <summary>
        /// Refreshes the objective information for a new WvW matchup
        /// </summary>
        /// <param name="wvwTeams">The new collection of WvW teams</param>
        public void RefreshForMatchReset(ICollection<WvWTeamViewModel> wvwTeams)
        {
            this.wvwTeams = wvwTeams;

            // Just raise property changed that all properties have changed
            this.OnPropertyChanged(null);
        }

        /// <summary>
        /// Adds the event to the list of hidden events
        /// </summary>
        private void AddToHiddenObjectives()
        {
            logger.Debug("Adding \"{0}\" to hidden objectives", this.Name);
            this.userData.HiddenObjectives.Add(this.ID);
        }

        /// <summary>
        /// Refreshes the visibility of the event
        /// </summary>
        private void RefreshVisibility()
        {
            logger.Trace("Refreshing visibility of \"{0}\"", this.Name);
            if (this.userData.HiddenObjectives.Any(id => id == this.ID))
            {
                this.IsVisible = false;
            }
            else if (!this.userData.AreRedObjectivesShown && this.WorldOwner == WorldColor.Red)
            {
                this.IsVisible = false;
            }
            else if (!this.userData.AreGreenObjectivesShown && this.WorldOwner == WorldColor.Green)
            {
                this.IsVisible = false;
            }
            else if (!this.userData.AreBlueObjectivesShown && this.WorldOwner == WorldColor.Blue)
            {
                this.IsVisible = false;
            }
            else if (!this.userData.AreNeutralObjectivesShown && this.WorldOwner == WorldColor.None)
            {
                this.IsVisible = false;
            }
            else if (!this.userData.AreCastlesShown && this.Type == ObjectiveType.Castle)
            {
                this.IsVisible = false;
            }
            else if (!this.userData.AreKeepsShown && this.Type == ObjectiveType.Keep)
            {
                this.IsVisible = false;
            }
            else if (!this.userData.AreTowersShown && this.Type == ObjectiveType.Tower)
            {
                this.IsVisible = false;
            }
            else if (!this.userData.AreCampsShown && this.Type == ObjectiveType.Camp)
            {
                this.IsVisible = false;
            }
            else if (!this.userData.AreBloodlustObjectivesShown &&
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

        /// <summary>
        /// Returns true if a chat code is available for this objective, else false
        /// </summary>
        private bool CanCopyChatCode()
        {
            return !string.IsNullOrWhiteSpace(this.ModelData.ChatCode);
        }

        /// <summary>
        /// Copies the objectives's chat code, if it has one
        /// </summary>
        private void CopyChatCode()
        {
            logger.Debug("Copying chat code of \"{0}\" as \"{1}\"", this.Name, this.ModelData.ChatCode);
            System.Windows.Clipboard.SetDataObject(this.ModelData.ChatCode);
        }

        /// <summary>
        /// Copies "'x' is under attack!" text to the clipboard for pasting into the in-game chat
        /// </summary>
        private void CopyUnderAttackText()
        {
            logger.Debug("Copying under attack text of \"{0}\"", this.Name);
            string name = this.CanCopyChatCode() ? this.ModelData.ChatCode : this.Name;
            System.Windows.Clipboard.SetDataObject(string.Format(Resources.WvWUnderAttackClipboardText, name, this.Location, this.Type));
        }

        /// <summary>
        /// Copies "Enemy headed to 'x'" text to the clipboard for pasting into the in-game chat
        /// </summary>
        private void CopyEnemyHeadedToText()
        {
            logger.Debug("Copying enemy-headed-to text of \"{0}\"", this.Name);
            string name = this.CanCopyChatCode() ? this.ModelData.ChatCode : this.Name;
            if (this.Type != ObjectiveType.BattlesHollow
                && this.Type != ObjectiveType.BauersEstate
                && this.Type != ObjectiveType.CarversAscent
                && this.Type != ObjectiveType.OrchardOverlook
                && this.Type != ObjectiveType.TempleofLostPrayers)
            {
                System.Windows.Clipboard.SetDataObject(string.Format(Resources.WvWEnemyHeadedClipboardText, name, this.Location, this.Type));
            }
            else
            {
                System.Windows.Clipboard.SetDataObject(string.Format(Resources.WvWEnemyHeadedAltClipboardText, name));
            }
        }

        /// <summary>
        /// Copies "I'm headed to 'x'" text to the clipboard for pasting into the in-game chat
        /// </summary>
        private void CopyPlayerHeadedToText()
        {
            string distance = string.Empty;
            string distanceUnits = string.Empty;
            switch (this.UserData.DistanceUnits)
            {
                case Units.Feet:
                    distance = this.DistanceFromPlayer.ToString();
                    distanceUnits = "ft";
                    break;
                case Units.Meters:
                    distance = this.DistanceFromPlayer.ToString();
                    distanceUnits = "m";
                    break;
                case Units.TimeDistance:
                    distance = TimeSpan.FromSeconds(this.DistanceFromPlayer).ToString("mm\\:ss");
                    distanceUnits = string.Empty;
                    break;
                default:
                    break;
            }

            logger.Debug("Copying player-headed-to text of \"{0}\"", this.Name);

            string name = this.CanCopyChatCode() ? this.ModelData.ChatCode : this.Name;

            if (this.Type != ObjectiveType.BattlesHollow
                && this.Type != ObjectiveType.BauersEstate
                && this.Type != ObjectiveType.CarversAscent
                && this.Type != ObjectiveType.OrchardOverlook
                && this.Type != ObjectiveType.TempleofLostPrayers)
            {
                System.Windows.Clipboard.SetDataObject(string.Format(Resources.WvWPlayerHeadedClipboardText, name, this.Location, this.Type, distance, distanceUnits));
            }
            else
            {
                System.Windows.Clipboard.SetDataObject(string.Format(Resources.WvWPlayerHeadedAltClipboardText, name, distance, distanceUnits));
            }
        }

        /// <summary>
        /// Copies "x: RI 'xx:xx:xx'" or "x: RI Not Active" text to the clipboard for pasting into the in-game chat
        /// </summary>
        private void CopyRIText()
        {
            logger.Debug("Copying RI text of \"{0}\"", this.Name);

            string name = this.CanCopyChatCode() ? this.ModelData.ChatCode : this.Name;

            if (this.IsRIActive)
            {
                System.Windows.Clipboard.SetDataObject(string.Format(Resources.WvWRIClipboardText, name, this.Location, this.Type, this.TimerValue.ToString("mm\\:ss")));
            }
            else
            {
                System.Windows.Clipboard.SetDataObject(string.Format(Resources.WvWNoRIClipboardText, name, this.Location, this.Type));
            }
        }

        /// <summary>
        /// Copies general data about an objective to the clipboard for pasting into the in-game chat
        /// </summary>
        private void CopyGeneralData()
        {
            logger.Debug("Copying RI text of \"{0}\"", this.Name);

            string name = this.CanCopyChatCode() ? this.ModelData.ChatCode : this.Name;

            if (this.IsRIActive)
            {
                System.Windows.Clipboard.SetDataObject(string.Format(Resources.WvWGeneralClipboardText,
                    name,
                    this.Location, this.Type,
                    this.WorldOwnerName,
                    this.TimerValue.ToString("mm\\:ss")));
            }
            else
            {
                System.Windows.Clipboard.SetDataObject(string.Format(Resources.WvWGeneralNoRIClipboardText,
                    name, 
                    this.Location, this.Type,
                    this.WorldOwnerName));
            }

            logger.Debug("Copying RI text of \"{0}\"", this.Name);
        }
    }
}
