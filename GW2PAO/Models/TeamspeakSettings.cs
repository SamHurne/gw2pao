using System;
using System.Collections.Generic;
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
    /// User settings for the Teamspeak Overlay
    /// </summary>
    [Serializable]
    public class TeamspeakSettings : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default settings filename
        /// </summary>
        public static string Filename { get { return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location) + ".TeamspeakSettings.xml"; } }

        private bool showChatEntryBox;
        private bool showChannelName;

        /// <summary>
        /// True to show the chat entry box, else false
        /// </summary>
        public bool ShowChatEntryBox
        {
            get { return this.showChatEntryBox; }
            set { this.SetField(ref this.showChatEntryBox, value); }
        }

        /// <summary>
        /// True to show the channel name box, else false
        /// </summary>
        public bool ShowChannelName
        {
            get { return this.showChannelName; }
            set { this.SetField(ref this.showChannelName, value); }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TeamspeakSettings()
        {
            this.ShowChatEntryBox = true;
        }

        /// <summary>
        /// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
        /// </summary>
        public void EnableAutoSave()
        {
            logger.Info("Enabling auto save");
            this.PropertyChanged += (o, e) => TeamspeakSettings.SaveSettings(this);
        }

        /// <summary>
        /// Loads the user settings
        /// </summary>
        /// <returns>The loaded EventTrackerSettings, or null if the load fails</returns>
        public static TeamspeakSettings LoadSettings()
        {
            logger.Debug("Loading user settings");

            XmlSerializer deserializer = new XmlSerializer(typeof(EventSettings));
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
                return loadedSettings as TeamspeakSettings;
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
        public static void SaveSettings(TeamspeakSettings settings)
        {
            logger.Debug("Saving user settings");
            XmlSerializer serializer = new XmlSerializer(typeof(TeamspeakSettings));
            using (TextWriter writer = new StreamWriter(Filename))
            {
                serializer.Serialize(writer, settings);
            }
        }
    }
}
