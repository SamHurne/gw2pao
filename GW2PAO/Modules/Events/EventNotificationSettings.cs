using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Events
{
    /// <summary>
    /// Class containing the configuration for an event's notification settings
    /// </summary>
    [Serializable]
    public class EventNotificationSettings : BindableBase
    {
        private string eventName;
        private bool isNotificationEnabled;

        /// <summary>
        /// ID of the event
        /// </summary>
        public Guid EventID
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the Event
        /// I'd prefer not to duplicate this, but I have no alternative at the moment
        /// </summary>
        public string EventName
        {
            get { return this.eventName; }
            set { this.SetProperty(ref this.eventName, value); }
        }

        /// <summary>
        /// True if the notification is enabled, else false
        /// </summary>
        public bool IsNotificationEnabled
        {
            get { return this.isNotificationEnabled; }
            set { this.SetProperty(ref this.isNotificationEnabled, value); }
        }

        /// <summary>
        /// The amount of time before an event that a notification should be shown
        /// </summary>
        public SerializableTimespan TimeToNotify
        {
            get;
            set;
        }

        [XmlIgnore]
        public TimeSpan MaxNotificationInterval { get { return TimeSpan.FromHours(1); } }

        [XmlIgnore]
        public TimeSpan MinNotificationInterval { get { return TimeSpan.Zero; } }

        [XmlIgnore]
        public TimeSpan NotificationInterval
        {
            get { return this.TimeToNotify.Time; }
            set { this.SetProperty(ref this.TimeToNotify.Time, value); }
        }

        /// <summary>
        /// Parameter-less constructor for serialization purposes
        /// </summary>
        public EventNotificationSettings()
        {
            this.TimeToNotify = new SerializableTimespan();
        }

        /// <summary>
        /// Default constructor, initializes an EventNotificationSettings object with defaults
        /// </summary>
        /// <param name="eventId"></param>
        public EventNotificationSettings(Guid eventId)
        {
            this.EventID = eventId;
            this.TimeToNotify = new SerializableTimespan();
            this.NotificationInterval = TimeSpan.FromMinutes(1);
            this.IsNotificationEnabled = true;
        }
    }
}
