using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;
using GW2PAO.Modules.Events.Interfaces;
using GW2PAO.Modules.Events.ViewModels.WorldBossTimers;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.Modules.Events.ViewModels.EventNotification
{
    /// <summary>
    /// View model for world boss event notifications
    /// </summary>
    public class WorldBossEventNotificationViewModel : EventNotificationViewModel
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private WorldBossEventViewModel eventViewModel;

        /// <summary>
        /// Name of the zone in which the event occurs
        /// </summary>
        public string ZoneName
        {
            get { return this.eventViewModel.ZoneName; }
        }

        /// <summary>
        /// Current state of the event
        /// </summary>
        public EventState State
        {
            get { return this.eventViewModel.State; }
        }

        /// <summary>
        /// Depending on the state of the event, contains the
        /// 'Time Until Active' or the 'Time Since Active'
        /// </summary>
        public TimeSpan TimerValue
        {
            get { return this.eventViewModel.TimerValue; }
        }

        /// <summary>
        /// Time since the event was last active
        /// </summary>
        public TimeSpan TimeSinceActive
        {
            get { return this.eventViewModel.TimeSinceActive; }
        }

        /// <summary>
        /// Daily treasure obtained state
        /// </summary>
        public bool IsTreasureObtained
        {
            get { return this.eventViewModel.IsTreasureObtained; }
        }

        /// <summary>
        /// Command to copy the nearest waypoint's chat code to the clipboard
        /// </summary>
        public DelegateCommand CopyWaypointCommand { get { return this.eventViewModel.CopyWaypointCommand; } }

        /// <summary>
        /// Command to copy the information about the event to the clipboard
        /// </summary>
        public DelegateCommand CopyDataCommand { get { return this.eventViewModel.CopyDataCommand; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public WorldBossEventNotificationViewModel(WorldBossEventViewModel eventViewModel, ICollection<IEventNotification> visibleNotifications)
            : base(eventViewModel.EventId, eventViewModel.EventName, visibleNotifications)
        {
            this.eventViewModel = eventViewModel;
            this.eventViewModel.PropertyChanged += EventViewModel_PropertyChanged;
        }

        /// <summary>
        /// Performs cleanup for this notification
        /// </summary>
        protected override void CleanupNotification()
        {
            this.eventViewModel.PropertyChanged -= EventViewModel_PropertyChanged;
            base.CleanupNotification();
        }

        /// <summary>
        /// Handles the event view model's property changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Forward the property changed event
            this.OnPropertyChanged(e.PropertyName);
        }
    }
}
