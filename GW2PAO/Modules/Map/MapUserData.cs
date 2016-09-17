using GW2PAO.Data.UserData;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.Map
{
    /// <summary>
    /// User settings for the Maps Module
    /// </summary>
    [Serializable]
    public class MapUserData : UserData<MapUserData>
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default settings filename
        /// </summary>
        public const string Filename = "MapsUserData.xml";

        private bool areHeartsVisible;
        private bool arePoisVisible;
        private bool areHeroPointsVisible;
        private bool areVistasVisible;
        private bool areWaypointsVisible;
        private bool areDungeonsVisible;
        private bool showEntireContinent;

        private bool snapMapToCharacter;
        private bool showCharacterPointer;
        private bool showPlayerTrail;
        private int playerTrailMaxLength;

        /// <summary>
        /// True if Heart Quests are shown on the map, else false
        /// </summary>
        public bool AreHeartsVisible
        {
            get { return this.areHeartsVisible; }
            set { SetProperty(ref this.areHeartsVisible, value); }
        }

        /// <summary>
        /// True if Points of Interest are shown on the map, else false
        /// </summary>
        public bool ArePoisVisible
        {
            get { return this.arePoisVisible; }
            set { SetProperty(ref this.arePoisVisible, value); }
        }

        /// <summary>
        /// True if Hero Points are shown on the map, else false
        /// </summary>
        public bool AreHeroPointsVisible
        {
            get { return this.areHeroPointsVisible; }
            set { SetProperty(ref this.areHeroPointsVisible, value); }
        }

        /// <summary>
        /// True if Vistas are show on the map, else false
        /// </summary>
        public bool AreVistasVisible
        {
            get { return this.areVistasVisible; }
            set { SetProperty(ref this.areVistasVisible, value); }
        }

        /// <summary>
        /// True if Waypoints are shown on the map, else false
        /// </summary>
        public bool AreWaypointsVisible
        {
            get { return this.areWaypointsVisible; }
            set { SetProperty(ref this.areWaypointsVisible, value); }
        }

        /// <summary>
        /// True if Dungeons are shown on the map, else false
        /// </summary>
        public bool AreDungeonsVisible
        {
            get { return this.areDungeonsVisible; }
            set { SetProperty(ref this.areDungeonsVisible, value); }
        }

        /// <summary>
        /// True if all zone items for the continent should be shown, else false
        /// </summary>
        public bool ShowEntireContinent
        {
            get { return this.showEntireContinent; }
            set { SetProperty(ref this.showEntireContinent, value); }
        }

        /// <summary>
        /// True if the map should snap center to the character, else false
        /// </summary>
        public bool SnapMapToCharacter
        {
            get { return this.snapMapToCharacter; }
            set { SetProperty(ref this.snapMapToCharacter, value); }
        }

        /// <summary>
        /// True if the map show the character pointer, else false
        /// </summary>
        public bool ShowCharacterPointer
        {
            get { return this.showCharacterPointer; }
            set { SetProperty(ref this.showCharacterPointer, value); }
        }

        /// <summary>
        /// True if the map should show the player trail, else false
        /// </summary>
        public bool ShowPlayerTrail
        {
            get { return this.showPlayerTrail; }
            set { SetProperty(ref this.showPlayerTrail, value); }
        }

        /// <summary>
        /// The maximum player trail length
        /// </summary>
        public int PlayerTrailMaxLength
        {
            get { return this.playerTrailMaxLength; }
            set { SetProperty(ref this.playerTrailMaxLength, value); }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MapUserData()
        {
            this.AreHeartsVisible = true;
            this.ArePoisVisible = true;
            this.AreHeroPointsVisible = true;
            this.AreVistasVisible = true;
            this.AreWaypointsVisible = true;
            this.AreDungeonsVisible = true;
            this.ShowEntireContinent = false;
            this.SnapMapToCharacter = true;
            this.ShowCharacterPointer = true;
            this.ShowPlayerTrail = true;
            this.PlayerTrailMaxLength = 100;
        }

        /// <summary>
        /// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
        /// </summary>
        public override void EnableAutoSave()
        {
            logger.Info("Enabling auto save");
            this.PropertyChanged += (o, e) => MapUserData.SaveData(this, MapUserData.Filename);
        }
    }
}
