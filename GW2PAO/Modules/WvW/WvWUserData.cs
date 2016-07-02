using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace GW2PAO.Modules.WvW
{
    /// <summary>
    /// User settings for WvW
    /// </summary>
    [Serializable]
    public class WvWUserData : UserData<WvWUserData>
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default settings filename
        /// </summary>
        public const string Filename = "WvWUserData.xml";

        private World worldSelection;
        private bool isTrackerHorizontal;
        private bool areTimeDistancesShown;
        private bool areNotificationsEnabled;
        private bool areBlueBorderlandsNotificationsEnabled;
        private bool areGreenBorderlandsNotificationsEnabled;
        private bool areRedBorderlandsNotificationsEnabled;
        private bool areEternalBattlegroundsNotificationsEnabled;

        private bool notifyWhenHomeTakesObjective;
        private bool notifyWhenHomeLosesObjective;
        private bool notifyWhenOtherTakesOtherObjective;
        private bool areCastleNotificationsEnabled;
        private bool areKeepNotificationsEnabled;
        private bool areTowerNotificationsEnabled;
        private bool areCampNotificationsEnabled;
        private bool areBloodlustNotificationsEnabled;

        private string objectivesSortProperty;

        private bool areCastlesShown;
        private bool areKeepsShown;
        private bool areTowersShown;
        private bool areCampsShown;
        private bool areBloodlustObjectivesShown;
        private bool areRedObjectivesShown;
        private bool areGreenObjectivesShown;
        private bool areBlueObjectivesShown;
        private bool areNeutralObjectivesShown;
        private bool areShortNamesShown;
        private WvWMap mapOverride;
        private Units distanceUnits;
        private bool autoOpenCloseTracker;
        private uint notificationDuration;
        private ObservableCollection<WvWObjectiveId> hiddenObjectives = new ObservableCollection<WvWObjectiveId>();

        /// <summary>
        /// The user's world selection for WvW
        /// </summary>
        public World WorldSelection
        {
            get { return this.worldSelection; }
            set { this.SetProperty(ref this.worldSelection, value); }
        }

        /// <summary>
        /// True if the WvW tracker window is horizontal, else false if vertical
        /// </summary>
        public bool IsTrackerHorizontal
        {
            get { return this.isTrackerHorizontal; }
            set { this.SetProperty(ref this.isTrackerHorizontal, value); }
        }

        /// <summary>
        /// True if the WvW tracker should show time-distances, else false
        /// </summary>
        public bool AreTimeDistancesShown
        {
            get { return this.areTimeDistancesShown; }
            set { this.SetProperty(ref this.areTimeDistancesShown, value); }
        }

        /// <summary>
        /// True if notifications are enabled (in general), else false
        /// </summary>
        public bool AreNotificationsEnabled
        {
            get { return this.areNotificationsEnabled; }
            set { SetProperty(ref this.areNotificationsEnabled, value); }
        }

        /// <summary>
        /// True if notifications for the Blue Borderlands are enabled, else false
        /// </summary>
        public bool AreBlueBorderlandsNotificationsEnabled
        {
            get { return this.areBlueBorderlandsNotificationsEnabled; }
            set { this.SetProperty(ref this.areBlueBorderlandsNotificationsEnabled, value); }
        }

        /// <summary>
        /// True if notifications for the Green Borderlands are enabled, else false
        /// </summary>
        public bool AreGreenBorderlandsNotificationsEnabled
        {
            get { return this.areGreenBorderlandsNotificationsEnabled; }
            set { this.SetProperty(ref this.areGreenBorderlandsNotificationsEnabled, value); }
        }

        /// <summary>
        /// True if notifications for the Red Borderlands are enabled, else false
        /// </summary>
        public bool AreRedBorderlandsNotificationsEnabled
        {
            get { return this.areRedBorderlandsNotificationsEnabled; }
            set { this.SetProperty(ref this.areRedBorderlandsNotificationsEnabled, value); }
        }

        /// <summary>
        /// True if notifications for the Eternal Battlegrounds are enabled, else false
        /// </summary>
        public bool AreEternalBattlegroundsNotificationsEnabled
        {
            get { return this.areEternalBattlegroundsNotificationsEnabled; }
            set { this.SetProperty(ref this.areEternalBattlegroundsNotificationsEnabled, value); }
        }

        /// <summary>
        /// True to enable notifications shown when the player's home world takes an objective
        /// </summary>
        public bool NotifyWhenHomeTakesObjective
        {
            get { return this.notifyWhenHomeTakesObjective; }
            set { this.SetProperty(ref this.notifyWhenHomeTakesObjective, value); }
        }

        /// <summary>
        /// True to enable notifications shown when the player's home world loses an objective
        /// </summary>
        public bool NotifyWhenHomeLosesObjective
        {
            get { return this.notifyWhenHomeLosesObjective; }
            set { this.SetProperty(ref this.notifyWhenHomeLosesObjective, value); }
        }

        /// <summary>
        /// True to enable notifications shown when another world takes another world's objective
        /// </summary>
        public bool NotifyWhenOtherTakesOtherObjective
        {
            get { return this.notifyWhenOtherTakesOtherObjective; }
            set { this.SetProperty(ref this.notifyWhenOtherTakesOtherObjective, value); }
        }

        /// <summary>
        /// True if notifications for castles are enabled, else false
        /// </summary>
        public bool AreCastleNotificationsEnabled
        {
            get { return this.areCastleNotificationsEnabled; }
            set { this.SetProperty(ref this.areCastleNotificationsEnabled, value); }
        }

        /// <summary>
        /// True if notifications for keeps are enabled, else false
        /// </summary>
        public bool AreKeepNotificationsEnabled
        {
            get { return this.areKeepNotificationsEnabled; }
            set { this.SetProperty(ref this.areKeepNotificationsEnabled, value); }
        }

        /// <summary>
        /// True if notifications for towers are enabled, else false
        /// </summary>
        public bool AreTowerNotificationsEnabled
        {
            get { return this.areTowerNotificationsEnabled; }
            set { this.SetProperty(ref this.areTowerNotificationsEnabled, value); }
        }

        /// <summary>
        /// True if notifications for camps are enabled, else false
        /// </summary>
        public bool AreCampNotificationsEnabled
        {
            get { return this.areCampNotificationsEnabled; }
            set { this.SetProperty(ref this.areCampNotificationsEnabled, value); }
        }

        /// <summary>
        /// True if notifications for bloodlust objectives are enabled, else false
        /// </summary>
        public bool AreBloodlustNotificationsEnabled
        {
            get { return this.areBloodlustNotificationsEnabled; }
            set { this.SetProperty(ref this.areBloodlustNotificationsEnabled, value); }
        }

        /// <summary>
        /// True if Castle objectives are shown in the tracker, else false
        /// </summary>
        public bool AreCastlesShown
        {
            get { return this.areCastlesShown; }
            set { this.SetProperty(ref this.areCastlesShown, value); }
        }

        /// <summary>
        /// True if Keep objectives are shown in the tracker, else false
        /// </summary>
        public bool AreKeepsShown
        {
            get { return this.areKeepsShown; }
            set { this.SetProperty(ref this.areKeepsShown, value); }
        }

        /// <summary>
        /// True if Tower objectives are shown in the tracker, else false
        /// </summary>
        public bool AreTowersShown
        {
            get { return this.areTowersShown; }
            set { this.SetProperty(ref this.areTowersShown, value); }
        }

        /// <summary>
        /// True if Camp objectives are shown in the tracker, else false
        /// </summary>
        public bool AreCampsShown
        {
            get { return this.areCampsShown; }
            set { this.SetProperty(ref this.areCampsShown, value); }
        }

        /// <summary>
        /// True if Bloodlust objectives (Orchard Overlook, Bauer's Estate, etc) are shown in the tracker, else false
        /// </summary>
        public bool AreBloodlustObjectivesShown
        {
            get { return this.areBloodlustObjectivesShown; }
            set { this.SetProperty(ref this.areBloodlustObjectivesShown, value); }
        }

        /// <summary>
        /// True if Red-owned objectives are shown, else false
        /// </summary>
        public bool AreRedObjectivesShown
        {
            get { return this.areRedObjectivesShown; }
            set { this.SetProperty(ref this.areRedObjectivesShown, value); }
        }

        /// <summary>
        /// True if Green-owned objectives are shown, else false
        /// </summary>
        public bool AreGreenObjectivesShown
        {
            get { return this.areGreenObjectivesShown; }
            set { this.SetProperty(ref this.areGreenObjectivesShown, value); }
        }

        /// <summary>
        /// True if Blue-owned objectives are shown, else false
        /// </summary>
        public bool AreBlueObjectivesShown
        {
            get { return this.areBlueObjectivesShown; }
            set { this.SetProperty(ref this.areBlueObjectivesShown, value); }
        }

        /// <summary>
        /// True if Neutral objectives are shown, else false
        /// </summary>
        public bool AreNeutralObjectivesShown
        {
            get { return this.areNeutralObjectivesShown; }
            set { this.SetProperty(ref this.areNeutralObjectivesShown, value); }
        }

        /// <summary>
        /// True if short names are shown, else false (cardinal directions are shown)
        /// </summary>
        public bool AreShortNamesShown
        {
            get { return this.areShortNamesShown; }
            set { this.SetProperty(ref this.areShortNamesShown, value); }
        }

        /// <summary>
        /// The property name to use when sorting objectives in the WvW tracker
        /// </summary>
        public string ObjectivesSortProperty
        {
            get { return this.objectivesSortProperty; }
            set { this.SetProperty(ref objectivesSortProperty, value); }
        }

        /// <summary>
        /// User-configured map override
        /// </summary>
        public WvWMap MapOverride
        {
            get { return this.mapOverride; }
            set { this.SetProperty(ref this.mapOverride, value); }
        }

        /// <summary>
        /// Units in which to display distances
        /// </summary>
        public Units DistanceUnits
        {
            get { return this.distanceUnits; }
            set { this.SetProperty(ref this.distanceUnits, value); }
        }

        /// <summary>
        /// True if the tracker should automatically open/close when entering/exiting WvW
        /// </summary>
        public bool AutoOpenCloseTracker
        {
            get { return this.autoOpenCloseTracker; }
            set { SetProperty(ref this.autoOpenCloseTracker, value); }
        }

        /// <summary>
        /// The amount of time to display notifications, in seconds
        /// </summary>
        public uint NotificationDuration
        {
            get { return this.notificationDuration; }
            set { SetProperty(ref this.notificationDuration, value); }
        }

        /// <summary>
        /// Collection of user-configured Hidden Objectives
        /// </summary>
        public ObservableCollection<WvWObjectiveId> HiddenObjectives { get { return this.hiddenObjectives; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public WvWUserData()
        {
            this.WorldSelection = new World() { ID = 1019, Name = "Blackgate" };
            this.IsTrackerHorizontal = false;
            this.AreTimeDistancesShown = true;
            this.AreNotificationsEnabled = false;
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
            this.NotificationDuration = 10;
        }

        /// <summary>
        /// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
        /// </summary>
        public override void EnableAutoSave()
        {
            logger.Info("Enabling auto save");
            this.PropertyChanged += (o, e) => WvWUserData.SaveData(this, WvWUserData.Filename);
        }
    }
}
