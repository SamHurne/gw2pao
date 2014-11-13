using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;
using GW2PAO.Data.UserData;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.Modules.ZoneCompletion
{
    /// <summary>
    /// User settings for the Zone Completion Assistant
    /// </summary>
    [Serializable]
    public class ZoneCompletionUserData : UserData<ZoneCompletionUserData>
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default settings filename
        /// </summary>
        public const string Filename = "ZoneCompletionUserData.xml";

        private bool areHeartsVisible;
        private bool arePoisVisible;
        private bool areSkillPointsVisible;
        private bool areVistasVisible;
        private bool areWaypointsVisible;
        private bool autoUnlockWaypoints;
        private bool autoUnlockPois;
        private bool autoUnlockVistas;
        private bool autoUnlockHeartQuests;
        private bool autoUnlockSkillChallenges;
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
        /// True if Heart Quests are configured to automatically be marked as unlocked, else false
        /// </summary>
        public bool AutoUnlockHeartQuests
        {
            get { return this.autoUnlockHeartQuests; }
            set { SetField(ref this.autoUnlockHeartQuests, value); }
        }

        /// <summary>
        /// True if Skill Point Challenges are configured to automatically be marked as unlocked, else false
        /// </summary>
        public bool AutoUnlockSkillChallenges
        {
            get { return this.autoUnlockSkillChallenges; }
            set { SetField(ref this.autoUnlockSkillChallenges, value); }
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
        public ZoneCompletionUserData()
        {
            this.AreHeartsVisible = true;
            this.ArePoisVisible = true;
            this.AreSkillChallengesVisible = true;
            this.AreVistasVisible = true;
            this.AreWaypointsVisible = true;
            this.AutoUnlockWaypoints = true;
            this.AutoUnlockPois = true;
            this.AutoUnlockVistas = true;
            this.AutoUnlockHeartQuests = true;
            this.AutoUnlockSkillChallenges = true;
            this.ShowUnlockedPoints = true;
            this.DistanceUnits = Units.Feet;
        }

        /// <summary>
        /// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
        /// </summary>
        public override void EnableAutoSave()
        {
            logger.Info("Enabling auto save");
            this.PropertyChanged += (o, e) => ZoneCompletionUserData.SaveData(this, ZoneCompletionUserData.Filename);
            this.HiddenZoneItems.CollectionChanged += (o, e) => ZoneCompletionUserData.SaveData(this, ZoneCompletionUserData.Filename);
            this.UnlockedZoneItems.CollectionChanged += (o, e) =>
                {
                    ZoneCompletionUserData.SaveData(this, ZoneCompletionUserData.Filename);
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (CharacterZoneItems itemAdded in e.NewItems)
                        {
                            itemAdded.ZoneItems.CollectionChanged += (a, b) => ZoneCompletionUserData.SaveData(this, ZoneCompletionUserData.Filename);
                        }
                    }
                };
        }
    }

    public class CharacterZoneItems
    {
        private ObservableCollection<ZoneItem> zoneItems = new ObservableCollection<ZoneItem>();

        public string Character { get; set; }
        public ObservableCollection<ZoneItem> ZoneItems { get { return this.zoneItems; } }

    }
}
