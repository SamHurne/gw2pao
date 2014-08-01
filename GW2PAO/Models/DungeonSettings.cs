using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.Models
{
    /// <summary>
    /// User settings for the Dungeons Tracker
    /// </summary>
    [Serializable]
    public class DungeonSettings : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default settings filename
        /// </summary>
        public static string Filename { get { return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location) + ".DungeonSettings.xml"; } }

        private DateTime lastResetDateTime;
        private ObservableCollection<Guid> hiddenDungeons = new ObservableCollection<Guid>();
        private ObservableCollection<Guid> completedPaths = new ObservableCollection<Guid>();

        /// <summary>
        /// The last recorded server-reset date/time
        /// </summary>
        public DateTime LastResetDateTime
        {
            get { return this.lastResetDateTime; }
            set { SetField(ref this.lastResetDateTime, value); }
        }

        /// <summary>
        /// Collection of user-configured Hidden Dungeons
        /// </summary>
        public ObservableCollection<Guid> HiddenDungeons { get { return this.hiddenDungeons; } }

        /// <summary>
        /// Collection of user-configured completed dungeon paths
        /// </summary>
        public ObservableCollection<Guid> CompletedPaths { get { return this.completedPaths; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DungeonSettings()
        {
            this.LastResetDateTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
        /// </summary>
        public void EnableAutoSave()
        {
            logger.Info("Enabling auto save");
            this.PropertyChanged += (o, e) => DungeonSettings.SaveSettings(this);
            this.HiddenDungeons.CollectionChanged += (o, e) => DungeonSettings.SaveSettings(this);
            this.CompletedPaths.CollectionChanged += (o, e) => DungeonSettings.SaveSettings(this);
        }

        /// <summary>
        /// Loads the user settings
        /// </summary>
        /// <returns>The loaded DungeonSettings, or null if the load fails</returns>
        public static DungeonSettings LoadSettings()
        {
            logger.Debug("Loading user settings");

            XmlSerializer deserializer = new XmlSerializer(typeof(DungeonSettings));
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
                return loadedSettings as DungeonSettings;
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
        public static void SaveSettings(DungeonSettings settings)
        {
            logger.Debug("Saving user settings");
            XmlSerializer serializer = new XmlSerializer(typeof(DungeonSettings));
            using (TextWriter writer = new StreamWriter(Filename))
            {
                serializer.Serialize(writer, settings);
            }
        }
    }
}
