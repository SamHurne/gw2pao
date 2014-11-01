using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.API.Data;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.Data.UserData
{
    /// <summary>
    /// User settings for the Events Tracker and Event Notifications
    /// </summary>
    [Serializable]
    public class EventsUserData : UserData<EventsUserData>
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default settings filename
        /// </summary>
        public const string Filename = "EventsUserData.xml";

        private bool areInactiveEventsVisible;
        private bool areEventNotificationsEnabled;
        private bool useAdjustedTimeTable;
        private DateTime lastResetDateTime;
        private ObservableCollection<Guid> hiddenEvents = new ObservableCollection<Guid>();
        private ObservableCollection<Guid> eventsWithTreasureObtained = new ObservableCollection<Guid>();

        /// <summary>
        /// True if inactive events are visible, else false
        /// </summary>
        public bool AreInactiveEventsVisible
        {
            get { return this.areInactiveEventsVisible; }
            set { SetField(ref this.areInactiveEventsVisible, value); }
        }

        /// <summary>
        /// True if event notifications are enabled, else false
        /// </summary>
        public bool AreEventNotificationsEnabled
        {
            get { return this.areEventNotificationsEnabled; }
            set { SetField(ref this.areEventNotificationsEnabled, value); }
        }

        /// <summary>
        /// True if the adjusted time table should be used, else false
        /// </summary>
        public bool UseAdjustedTimeTable
        {
            get { return this.useAdjustedTimeTable; }
            set { SetField(ref this.useAdjustedTimeTable, value); }
        }

        /// <summary>
        /// The last recorded server-reset date/time
        /// </summary>
        public DateTime LastResetDateTime
        {
            get { return this.lastResetDateTime; }
            set { SetField(ref this.lastResetDateTime, value); }
        }

        /// <summary>
        /// Collection of user-configured Hidden Events
        /// </summary>
        public ObservableCollection<Guid> HiddenEvents { get { return this.hiddenEvents; } }

        /// <summary>
        /// Collection of user-configured events with treasures already obtained
        /// </summary>
        public ObservableCollection<Guid> EventsWithTreasureObtained { get { return this.eventsWithTreasureObtained; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public EventsUserData()
        {
            this.AreInactiveEventsVisible = true;
            this.AreEventNotificationsEnabled = true;
            this.UseAdjustedTimeTable = true;
            this.LastResetDateTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
        /// </summary>
        public override void EnableAutoSave()
        {
            logger.Info("Enabling auto save");
            this.PropertyChanged += (o, e) => EventsUserData.SaveData(this, EventsUserData.Filename);
            this.HiddenEvents.CollectionChanged += (o, e) => EventsUserData.SaveData(this, EventsUserData.Filename);
            this.EventsWithTreasureObtained.CollectionChanged += (o, e) => EventsUserData.SaveData(this, EventsUserData.Filename);
        }
    }
}
