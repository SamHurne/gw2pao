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
using GW2PAO.API.Data.Enums;
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
        private bool isTrackerHorizontal;
        private bool areTimeDistancesShown;
        private bool areBlueBorderlandsNotificationsEnabled;
        private bool areGreenBorderlandsNotificationsEnabled;
        private bool areRedBorderlandsNotificationsEnabled;
        private bool areEternalBattlegroundsNotificationsEnabled;
        private bool areCastlesShown;
        private bool areKeepsShown;
        private bool areTowersShown;
        private bool areCampsShown;
        private bool areBloodlustObjectivesShown;
        private bool areRedObjectivesShown;
        private bool areGreenObjectivesShown;
        private bool areBlueObjectivesShown;
        private bool areNeutralObjectivesShown;
        private ObservableCollection<int> hiddenObjectives = new ObservableCollection<int>();

        /// <summary>
        /// The user's world selection for WvW
        /// </summary>
        public World WorldSelection
        {
            get { return this.worldSelection; }
            set { this.SetField(ref this.worldSelection, value); }
        }

        /// <summary>
        /// True if the WvW tracker window is horizontal, else false if vertical
        /// </summary>
        public bool IsTrackerHorizontal
        {
            get { return this.isTrackerHorizontal; }
            set { SetField(ref this.isTrackerHorizontal, value); }
        }

        /// <summary>
        /// True if the WvW tracker should show time-distances, else false
        /// </summary>
        public bool AreTimeDistancesShown
        {
            get { return this.areTimeDistancesShown; }
            set { SetField(ref this.areTimeDistancesShown, value); }
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
        /// True if Castle objectives are shown in the tracker, else false
        /// </summary>
        public bool AreCastlesShown
        {
            get { return this.areCastlesShown; }
            set { SetField(ref this.areCastlesShown, value); }
        }

        /// <summary>
        /// True if Keep objectives are shown in the tracker, else false
        /// </summary>
        public bool AreKeepsShown
        {
            get { return this.areKeepsShown; }
            set { SetField(ref this.areKeepsShown, value); }
        }

        /// <summary>
        /// True if Tower objectives are shown in the tracker, else false
        /// </summary>
        public bool AreTowersShown
        {
            get { return this.areTowersShown; }
            set { SetField(ref this.areTowersShown, value); }
        }

        /// <summary>
        /// True if Camp objectives are shown in the tracker, else false
        /// </summary>
        public bool AreCampsShown
        {
            get { return this.areCampsShown; }
            set { SetField(ref this.areCampsShown, value); }
        }

        /// <summary>
        /// True if Bloodlust objectives (Orchard Overlook, Bauer's Estate, etc) are shown in the tracker, else false
        /// </summary>
        public bool AreBloodlustObjectivesShown
        {
            get { return this.areBloodlustObjectivesShown; }
            set { SetField(ref this.areBloodlustObjectivesShown, value); }
        }

        /// <summary>
        /// True if Red-owned objectives are shown, else false
        /// </summary>
        public bool AreRedObjectivesShown
        {
            get { return this.areRedObjectivesShown; }
            set { SetField(ref this.areRedObjectivesShown, value); }
        }

        /// <summary>
        /// True if Green-owned objectives are shown, else false
        /// </summary>
        public bool AreGreenObjectivesShown
        {
            get { return this.areGreenObjectivesShown; }
            set { SetField(ref this.areGreenObjectivesShown, value); }
        }

        /// <summary>
        /// True if Blue-owned objectives are shown, else false
        /// </summary>
        public bool AreBlueObjectivesShown
        {
            get { return this.areBlueObjectivesShown; }
            set { SetField(ref this.areBlueObjectivesShown, value); }
        }

        /// <summary>
        /// True if Neutral objectives are shown, else false
        /// </summary>
        public bool AreNeutralObjectivesShown
        {
            get { return this.areNeutralObjectivesShown; }
            set { SetField(ref this.areNeutralObjectivesShown, value); }
        }

        /// <summary>
        /// Collection of user-configured Hidden Objectives
        /// </summary>
        public ObservableCollection<int> HiddenObjectives { get { return this.hiddenObjectives; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public WvWSettings()
        {
            this.WorldSelection = new World() { ID = 1019, Name = "Blackgate" };
            this.IsTrackerHorizontal = false;
            this.AreTimeDistancesShown = true;
            this.AreBlueBorderlandsNotificationsEnabled = false;
            this.AreGreenBorderlandsNotificationsEnabled = false;
            this.AreRedBorderlandsNotificationsEnabled = false;
            this.AreEternalBattlegroundsNotificationsEnabled = false;
            this.AreCastlesShown = true;
            this.AreKeepsShown = true;
            this.AreTowersShown = true;
            this.AreCampsShown = true;
            this.AreRedObjectivesShown = true;
            this.AreGreenObjectivesShown = true;
            this.AreBlueObjectivesShown = true;
            this.AreNeutralObjectivesShown = true;
            this.AreBloodlustObjectivesShown = true;
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
