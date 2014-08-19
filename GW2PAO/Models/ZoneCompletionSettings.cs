using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    /// User settings for the Zone Completion Assistant
    /// </summary>
    [Serializable]
    public class ZoneCompletionSettings : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default settings filename
        /// </summary>
        public static string Filename { get { return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location) + ".ZoneCompletionSettings.xml"; } }

        private bool areHeartsVisible;
        private bool arePoisVisible;
        private bool areSkillPointsVisible;
        private bool areVistasVisible;
        private bool areWaypointsVisible;
        private bool autoUnlockWaypoints;
        private bool autoUnlockPois;
        private bool autoUnlockVistas;
        private bool showUnlockedPoints;
        private Units distanceUnits;
        private ObservableCollection<ZoneItem> hiddenZoneItems = new ObservableCollection<ZoneItem>();
        private ObservableCollection<CharacterZoneItems> unlockedZoneItems = new ObservableCollection<CharacterZoneItems>();

        /// <summary>
        /// True if Heart Quests are shown in the list, else false
        /// </summary>
        public bool AreHeartsVisible
        {
            get { return this.areHeartsVisible; }
            set { SetField(ref this.areHeartsVisible, value); }
        }

        /// <summary>
        /// True if Points of Interest are shown in the list, else false
        /// </summary>
        public bool ArePoisVisible
        {
            get { return this.arePoisVisible; }
            set { SetField(ref this.arePoisVisible, value); }
        }

        /// <summary>
        /// True if Skill Challenges are show in the list, else false
        /// </summary>
        public bool AreSkillChallengesVisible
        {
            get { return this.areSkillPointsVisible; }
            set { SetField(ref this.areSkillPointsVisible, value); }
        }

        /// <summary>
        /// True if Vistas are show in the list, else false
        /// </summary>
        public bool AreVistasVisible
        {
            get { return this.areVistasVisible; }
            set { SetField(ref this.areVistasVisible, value); }
        }

        /// <summary>
        /// True if Waypoints are shown in the list, else false
        /// </summary>
        public bool AreWaypointsVisible
        {
            get { return this.areWaypointsVisible; }
            set { SetField(ref this.areWaypointsVisible, value); }
        }

        /// <summary>
        /// True if Waypoints are configured to automatically be marked as unlocked, else false
        /// </summary>
        public bool AutoUnlockWaypoints
        {
            get { return this.autoUnlockWaypoints; }
            set { SetField(ref this.autoUnlockWaypoints, value); }
        }

        /// <summary>
        /// True if Points of Interest are configured to automatically be marked as unlocked, else false
        /// </summary>
        public bool AutoUnlockPois
        {
            get { return this.autoUnlockPois; }
            set { SetField(ref this.autoUnlockPois, value); }
        }

        /// <summary>
        /// True if Vistas are configured to automatically be marked as unlocked, else false
        /// </summary>
        public bool AutoUnlockVistas
        {
            get { return this.autoUnlockVistas; }
            set { SetField(ref this.autoUnlockVistas, value); }
        }

        /// <summary>
        /// True if unlocked points are shown in the list, else false
        /// </summary>
        public bool ShowUnlockedPoints
        {
            get { return this.showUnlockedPoints; }
            set { SetField(ref this.showUnlockedPoints, value); }
        }

        /// <summary>
        /// The units used for calculated distances
        /// </summary>
        public Units DistanceUnits
        {
            get { return this.distanceUnits; }
            set { SetField(ref this.distanceUnits, value); }
        }

        /// <summary>
        /// Collection of hidden zone items 
        /// Since IDs are not unique across zones, this is a collection of the zone item objects themselves
        /// </summary>
        public ObservableCollection<ZoneItem> HiddenZoneItems { get { return this.hiddenZoneItems; } }

        /// <summary>
        /// Collection of unlocked zone items 
        /// Since IDs are not unique across zones, this is a collection of the zone item objects themselves
        /// Key: Character Name
        /// Value: Unlocked zone items for that character
        /// </summary>
        public ObservableCollection<CharacterZoneItems> UnlockedZoneItems { get { return this.unlockedZoneItems; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ZoneCompletionSettings()
        {
            this.AreHeartsVisible = true;
            this.ArePoisVisible = true;
            this.AreSkillChallengesVisible = true;
            this.AreVistasVisible = true;
            this.AreWaypointsVisible = true;
            this.AutoUnlockWaypoints = true;
            this.AutoUnlockPois = true;
            this.AutoUnlockVistas = true;
            this.ShowUnlockedPoints = true;
            this.DistanceUnits = Units.Feet;
        }

        /// <summary>
        /// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
        /// </summary>
        public void EnableAutoSave()
        {
            logger.Info("Enabling auto save");
            this.PropertyChanged += (o, e) => ZoneCompletionSettings.SaveSettings(this);
            this.HiddenZoneItems.CollectionChanged += (o, e) => ZoneCompletionSettings.SaveSettings(this);
            this.UnlockedZoneItems.CollectionChanged += (o, e) =>
                {
                    ZoneCompletionSettings.SaveSettings(this);
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (CharacterZoneItems itemAdded in e.NewItems)
                        {
                            itemAdded.ZoneItems.CollectionChanged += (a, b) => ZoneCompletionSettings.SaveSettings(this);
                        }
                    }
                };
        }

        /// <summary>
        /// Loads the user settings
        /// </summary>
        /// <returns>The loaded ZoneCompletionSettings, or null if the load fails</returns>
        public static ZoneCompletionSettings LoadSettings()
        {
            logger.Debug("Loading user settings");

            XmlSerializer deserializer = new XmlSerializer(typeof(ZoneCompletionSettings));
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
                    logger.Warn("Unable to load user settings!", ex);
                }
            }

            if (loadedSettings != null)
            {
                logger.Info("Settings successfully loaded");
                return loadedSettings as ZoneCompletionSettings;
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
        public static void SaveSettings(ZoneCompletionSettings settings)
        {
            logger.Debug("Saving user settings");
            XmlSerializer serializer = new XmlSerializer(typeof(ZoneCompletionSettings));
            using (TextWriter writer = new StreamWriter(Filename))
            {
                serializer.Serialize(writer, settings);
            }
        }
    }

    public class CharacterZoneItems
    {
        private ObservableCollection<ZoneItem> zoneItems = new ObservableCollection<ZoneItem>();

        public string Character { get; set; }
        public ObservableCollection<ZoneItem> ZoneItems { get { return this.zoneItems; } }

    }
}
