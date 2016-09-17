using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.API.Util;
using GW2PAO.Modules.ZoneCompletion;
using GW2PAO.Modules.ZoneCompletion.Interfaces;
using GW2PAO.Modules.ZoneCompletion.Models;
using GW2PAO.Modules.ZoneCompletion.ViewModels;
using GW2PAO.Utility;
using MapControl;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Map.ViewModels
{
    [Export(typeof(MapViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MapViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private const int DEFAULT_CONTINENT_ID = 1;

        private MapUserData userData;
        private IZoneCompletionController zoneController;
        private IZoneService zoneService;

        private int floorId;
        private Continent continentData;
        private Location mapCenter;

        /// <summary>
        /// Data for the displayed continent
        /// </summary>
        public Continent ContinentData
        {
            get { return this.continentData; }
            set
            {
                if (SetProperty(ref this.continentData, value))
                {
                    this.ZoneItems.ContinentID = value.Id;
                    this.OnPropertyChanged(() => this.MapTileSourceString);
                }
            }
        }

        /// <summary>
        /// The active floor to show on the map
        /// </summary>
        public int FloorId
        {
            get { return this.floorId; }
            set
            {
                if (SetProperty(ref this.floorId, value))
                {
                    this.OnPropertyChanged(() => this.MapTileSourceString);
                }
            }
        }

        /// <summary>
        /// Final source string to use when retrieving map tiles
        /// </summary>
        public string MapTileSourceString
        {
            get
            {
                return string.Format("https://tiles.guildwars2.com/{0}/{1}/{2}.jpg", this.ContinentData.Id, this.FloorId, "{z}/{x}/{y}");
            }
        }

        /// <summary>
        /// The center location of the map
        /// </summary>
        public Location MapCenter
        {
            get { return this.mapCenter; }
            set { SetProperty(ref this.mapCenter, value); }
        }

        /// <summary>
        /// ViewModel object containing all data associated with the character pointer
        /// </summary>
        public CharacterPointerViewModel CharacterPointer
        {
            get;
            private set;
        }

        /// <summary>
        /// ViewModel object containing all data associated with map markers
        /// </summary>
        public MarkersViewModel MapMarkers
        {
            get;
            private set;
        }

        /// <summary>
        /// ViewModel object containing all data associated with zone items
        /// </summary>
        public ZoneItemsViewModel ZoneItems
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructs a new MapViewModel
        /// </summary>
        [ImportingConstructor]
        public MapViewModel(IZoneCompletionController zoneController, IZoneService zoneService, IPlayerService playerService,
            ZoneItemsStore zoneItemsStore, MapUserData userData)
        {
            this.zoneController = zoneController;
            this.zoneService = zoneService;
            this.userData = userData;

            this.CharacterPointer = new CharacterPointerViewModel(zoneController, userData);
            this.CharacterPointer.PropertyChanged += CharacterPointer_PropertyChanged;

            this.MapMarkers = new MarkersViewModel(userData);
            this.ZoneItems = new ZoneItemsViewModel(zoneItemsStore, zoneController, userData);

            if (playerService.HasValidMapId)
                this.ContinentData = this.zoneService.GetContinentByMap(playerService.MapId);
            else
                this.ContinentData = this.zoneService.GetContinent(DEFAULT_CONTINENT_ID);

            this.FloorId = 1;

            ((INotifyPropertyChanged)this.zoneController).PropertyChanged += ZoneControllerPropertyChanged;
            this.zoneController.Start();
        }

        /// <summary>
        /// Handles the PropertyChanged event of the Zone Controller
        /// </summary>
        private void ZoneControllerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ReflectionUtility.GetPropertyName(() => this.zoneController.ActiveContinent))
            {
                this.ContinentData = this.zoneController.ActiveContinent;
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the CharacterPointer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CharacterPointer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ReflectionUtility.GetPropertyName(() => this.CharacterPointer.CharacterLocation))
            {
                if (this.CharacterPointer.SnapToCharacter)
                    this.MapCenter = this.CharacterPointer.CharacterLocation;
            }
            else if (e.PropertyName == ReflectionUtility.GetPropertyName(() => this.CharacterPointer.SnapToCharacter))
            {
                if (this.CharacterPointer.SnapToCharacter)
                    this.MapCenter = this.CharacterPointer.CharacterLocation;
            }
        }
    }
}
