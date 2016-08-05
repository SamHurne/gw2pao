using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Entities;
using GW2PAO.Modules.Events.Interfaces;
using GW2PAO.Modules.Events.ViewModels.MetaEventTimers;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Events.ViewModels.EventNotification
{
    /// <summary>
    /// View model for meta event notifications
    /// </summary>
    public class MetaEventNotificationViewModel : EventNotificationViewModel
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private MetaEventViewModel metaEvent;

        /// <summary>
        /// Next stage of the meta event
        /// </summary>
        public MetaEventStage CurrentStage
        {
            get { return this.metaEvent.CurrentStage; }
        }

        /// <summary>
        /// Next stage of the meta event
        /// </summary>
        public MetaEventStage NextStage
        {
            get { return this.metaEvent.NextStage; }
        }

        /// <summary>
        /// Time until the next meta event stage begins
        /// </summary>
        public TimeSpan TimeUntilNextStage
        {
            get { return this.metaEvent.TimeUntilNextStage; }
        }

        /// <summary>
        /// Time since the current meta event stage started
        /// </summary>
        public TimeSpan TimeSinceStageStarted
        {
            get { return this.metaEvent.TimeSinceStageStarted; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MetaEventNotificationViewModel(MetaEventViewModel metaEvent, ICollection<IEventNotification> visibleNotificationsCollection)
            : base(metaEvent.EventId, metaEvent.MapName, visibleNotificationsCollection)
        {
            this.metaEvent = metaEvent;
            this.metaEvent.PropertyChanged += MetaEvent_PropertyChanged;
        }

        /// <summary>
        /// Performs cleanup for this notification
        /// </summary>
        protected override void CleanupNotification()
        {
            this.metaEvent.PropertyChanged -= MetaEvent_PropertyChanged;
            base.CleanupNotification();
        }

        /// <summary>
        /// Handles the property change event of the backing Meta Event View Model
        /// </summary>
        private void MetaEvent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Forward the property changed event
            this.OnPropertyChanged(e.PropertyName);
        }
    }
}
