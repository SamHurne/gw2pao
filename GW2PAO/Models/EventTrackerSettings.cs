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

namespace GW2PAO.Models
{
    /// <summary>
    /// User settings for the Event Tracker
    /// </summary>
    [Serializable]
    public class EventTrackerSettings : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default settings filename
        /// </summary>
        public static string Filename { get { return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location) + ".EventTrackerSettings.xml"; } }

        private bool areInactiveEventsVisible;
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
        public EventTrackerSettings()
        {
            this.AreInactiveEventsVisible = true;
            this.LastResetDateTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
        /// </summary>
        public void EnableAutoSave()
        {
            logger.Info("Enabling auto save");
            this.PropertyChanged += (o, e) => EventTrackerSettings.SaveSettings(this);
            this.HiddenEvents.CollectionChanged += (o, e) => EventTrackerSettings.SaveSettings(this);
            this.EventsWithTreasureObtained.CollectionChanged += (o, e) => EventTrackerSettings.SaveSettings(this);
        }

        /// <summary>
        /// Loads the user settings
        /// </summary>
        /// <returns>The loaded EventTrackerSettings, or null if the load fails</returns>
        public static EventTrackerSettings LoadSettings()
        {
            logger.Debug("Loading user settings");

            XmlSerializer deserializer = new XmlSerializer(typeof(EventTrackerSettings));
            object loadedSettings = null;

            if (File.Exists(Filename))
            {
                try
                {
                    using (TextReader reader = new StreamReader(Filename))
                    {
                        loadedSettings = deserializer.Deserialize(reader);
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Unable to load user settings! Exception: ", ex);
                }
            }

            if (loadedSettings != null)
            {
                logger.Info("Settings successfully loaded");
                return loadedSettings as EventTrackerSettings;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Saves the user settings
        /// </summary>
        /// <param name="settings">The user settings to save</param>
        public static void SaveSettings(EventTrackerSettings settings)
        {
            logger.Debug("Saving user settings");
            XmlSerializer serializer = new XmlSerializer(typeof(EventTrackerSettings));
            using (TextWriter writer = new StreamWriter(Filename))
            {
                serializer.Serialize(writer, settings);
            }
        }
    }
}
