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
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.Data.UserData
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
        private bool areBlueBorderlandsNotificationsEnabled;
        private bool areGreenBorderlandsNotificationsEnabled;
        private bool areRedBorderlandsNotificationsEnabled;
        private bool areEternalBattlegroundsNotificationsEnabled;

        private bool notifyWhenHomeTakesObjective;
        private bool notifyWhenHomeLosesObjective;
        private bool notifyWhenOtherTakesOtherObjective;

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
        /// True to enable notifications shown when the player's home world takes an objective
        /// </summary>
        public bool NotifyWhenHomeTakesObjective
        {
            get { return this.notifyWhenHomeTakesObjective; }
            set { this.SetField(ref this.notifyWhenHomeTakesObjective, value); }
        }

        /// <summary>
        /// True to enable notifications shown when the player's home world loses an objective
        /// </summary>
        public bool NotifyWhenHomeLosesObjective
        {
            get { return this.notifyWhenHomeLosesObjective; }
            set { this.SetField(ref this.notifyWhenHomeLosesObjective, value); }
        }

        /// <summary>
        /// True to enable notifications shown when another world takes another world's objective
        /// </summary>
        public bool NotifyWhenOtherTakesOtherObjective
        {
            get { return this.notifyWhenOtherTakesOtherObjective; }
            set { this.SetField(ref this.notifyWhenOtherTakesOtherObjective, value); }
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
        /// True if short names are shown, else false (cardinal directions are shown)
        /// </summary>
        public bool AreShortNamesShown
        {
            get { return this.areShortNamesShown; }
            set { this.SetField(ref this.areShortNamesShown, value); }
        }

        /// <summary>
        /// User-configured map override
        /// </summary>
        public WvWMap MapOverride
        {
            get { return this.mapOverride; }
            set { this.SetField(ref this.mapOverride, value); }
        }

        /// <summary>
        /// Units in which to display distances
        /// </summary>
        public Units DistanceUnits
        {
            get { return this.distanceUnits; }
            set { SetField(ref this.distanceUnits, value); }
        }

        /// <summary>
        /// Collection of user-configured Hidden Objectives
        /// </summary>
        public ObservableCollection<int> HiddenObjectives { get { return this.hiddenObjectives; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public WvWUserData()
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
        public override void EnableAutoSave()
        {
            logger.Info("Enabling auto save");
            this.PropertyChanged += (o, e) => WvWUserData.SaveData(this, WvWUserData.Filename);
        }
    }
}
