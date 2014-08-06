using System;
using System.Collections.Generic;
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
    /// User settings for WvW
    /// </summary>
    [Serializable]
    public class WvWSettings : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default settings filename
        /// </summary>
        public static string Filename { get { return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location) + ".WvWSettings.xml"; } }

        private World worldSelection;
        private bool areBlueBorderlandsNotificationsEnabled;
        private bool areGreenBorderlandsNotificationsEnabled;
        private bool areRedBorderlandsNotificationsEnabled;
        private bool areEternalBattlegroundsNotificationsEnabled;

        /// <summary>
        /// The user's world selection for WvW
        /// </summary>
        public World WorldSelection
        {
            get { return this.worldSelection; }
            set { this.SetField(ref this.worldSelection, value); }
        }

        /// <summary>
        /// True if notifications for the Blue Borderlands are enabled, else false
        /// </summary>
        public bool AreBlueBorderlandsNotificationsEnabled
        {
            get { return this.areBlueBorderlandsNotificationsEnabled; }
            set { SetField(ref this.areBlueBorderlandsNotificationsEnabled, value); }
        }

        /// <summary>
        /// True if notifications for the Green Borderlands are enabled, else false
        /// </summary>
        public bool AreGreenBorderlandsNotificationsEnabled
        {
            get { return this.areGreenBorderlandsNotificationsEnabled; }
            set { SetField(ref this.areGreenBorderlandsNotificationsEnabled, value); }
        }

        /// <summary>
        /// True if notifications for the Red Borderlands are enabled, else false
        /// </summary>
        public bool AreRedBorderlandsNotificationsEnabled
        {
            get { return this.areRedBorderlandsNotificationsEnabled; }
            set { SetField(ref this.areRedBorderlandsNotificationsEnabled, value); }
        }

        /// <summary>
        /// True if notifications for the Eternal Battlegrounds are enabled, else false
        /// </summary>
        public bool AreEternalBattlegroundsNotificationsEnabled
        {
            get { return this.areEternalBattlegroundsNotificationsEnabled; }
            set { SetField(ref this.areEternalBattlegroundsNotificationsEnabled, value); }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public WvWSettings()
        {
            this.WorldSelection = new World() { ID = 1019, Name = "Blackgate" };
            this.AreBlueBorderlandsNotificationsEnabled = true;
            this.AreGreenBorderlandsNotificationsEnabled = true;
            this.AreRedBorderlandsNotificationsEnabled = true;
            this.AreEternalBattlegroundsNotificationsEnabled = true;
        }

        /// <summary>
        /// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
        /// </summary>
        public void EnableAutoSave()
        {
            logger.Info("Enabling auto save");
            this.PropertyChanged += (o, e) => WvWSettings.SaveSettings(this);
        }

        /// <summary>
        /// Loads the user settings
        /// </summary>
        /// <returns>The loaded DungeonSettings, or null if the load fails</returns>
        public static WvWSettings LoadSettings()
        {
            logger.Debug("Loading user settings");

            XmlSerializer deserializer = new XmlSerializer(typeof(WvWSettings));
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
                return loadedSettings as WvWSettings;
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
        public static void SaveSettings(WvWSettings settings)
        {
            logger.Debug("Saving user settings");
            XmlSerializer serializer = new XmlSerializer(typeof(WvWSettings));
            using (TextWriter writer = new StreamWriter(Filename))
            {
                serializer.Serialize(writer, settings);
            }
        }
    }
}
