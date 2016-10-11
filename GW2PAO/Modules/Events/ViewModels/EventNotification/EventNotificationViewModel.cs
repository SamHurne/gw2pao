using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Modules.Events.Interfaces;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Events.ViewModels.EventNotification
{
    public abstract class EventNotificationViewModel : BindableBase, IEventNotification
    {
        private bool wasNotificationShown;
        private bool isRemovingNotification;
        private ICollection<IEventNotification> visibleNotifications;

        /// <summary>
        /// GUID for the event
        /// </summary>
        public Guid EventId
        {
            get;
            protected set;
        }

        /// <summary>
        /// Name of the event
        /// </summary>
        public string EventName
        {
            get;
            protected set;
        }

        /// <summary>
        /// True if the notification for this event has already been shown, else false
        /// </summary>
        public bool WasNotificationShown
        {
            get { return this.wasNotificationShown; }
            set { SetProperty(ref this.wasNotificationShown, value); }
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
        /// Closes the displayed notification
        /// </summary>
        public DelegateCommand CloseNotificationCommand { get { return new DelegateCommand(this.CleanupNotification); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        protected EventNotificationViewModel(Guid eventId, string eventName, ICollection<IEventNotification> visibleNotifications)
        {
            this.EventId = eventId;
            this.EventName = eventName;
            this.visibleNotifications = visibleNotifications;
        }

        /// <summary>
        /// Cleans up the notification
        /// </summary>
        public void Cleanup()
        {
            this.CleanupNotification();
        }

        /// <summary>
        /// Performs cleanup for this notification
        /// </summary>
        protected virtual void CleanupNotification()
        {
            this.visibleNotifications.Remove(this);
        }
    }
}
