using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Enums;
using GW2PAO.PresentationCore;

namespace GW2PAO.ViewModels
{
    public class WvWObjectiveViewModel : NotifyPropertyChangedBase
    {
        private IEnumerable<WvWTeamViewModel> wvwTeams;
        private ICollection<WvWObjectiveViewModel> displayedNotificationsCollection;
        private WorldColor prevWorldOwner;
        private DateTime flipTime;
        private TimeSpan timerValue;
        private bool isRIActive;
        private bool isNotificationVisible;
        private bool isNotificationShown;
        private bool isRemovingNotification;

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

                    // Also refresh the world owner's name
                    this.RaisePropertyChanged("WorldOwnerName");
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
        /// Countdown timer used for Indignation Buff tracking
        /// </summary>
        public TimeSpan TimerValue
        {
            get { return this.timerValue; }
            set { SetField(ref this.timerValue, value); }
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
        /// Visiblity of the notification for this event
        /// This is configured by the user by showing/hiding specific event notifications
        /// TODO: Implement UI for user to configure/set this
        /// </summary>
        public bool IsNotificationVisible
        {
            get { return this.isNotificationVisible; }
            set { SetField(ref this.isNotificationVisible, value); }
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
        /// Closes the displayed notification
        /// </summary>
        public DelegateCommand CloseNotificationCommand { get { return new DelegateCommand(this.CloseNotification); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="objective">The objective details</param>
        /// <param name="wvwTeams">Collection containing all of the WvW Teams</param>
        public WvWObjectiveViewModel(WvWObjective objective, IEnumerable<WvWTeamViewModel> wvwTeams, ICollection<WvWObjectiveViewModel> displayedNotificationsCollection)
        {
            this.ModelData = objective;
            this.wvwTeams = wvwTeams;
            this.displayedNotificationsCollection = displayedNotificationsCollection;
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
