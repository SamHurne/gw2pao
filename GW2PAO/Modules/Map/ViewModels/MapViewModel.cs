using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Modules.ZoneCompletion.Interfaces;
using GW2PAO.Utility;
using MapControl;
using MapControl.Caching;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Map.ViewModels
{
    [Export(typeof(MapViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
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
                    this.ClearMapCache();
                    this.ZoneItems.ContinentID = value.Id;
                    this.OnPropertyChanged(() => this.MapTileSourceString);
                    Task.Delay(1000).ContinueWith(o => this.ResetMapCacheExpiration());
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
                    this.ClearMapCache();
                    this.OnPropertyChanged(() => this.MapTileSourceString);
                    Task.Delay(1000).ContinueWith(o => this.ResetMapCacheExpiration());
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
        public PlayerMarkersViewModel MapMarkers
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
        /// ViewModel object containing all data associated with zone items
        /// </summary>
        public DrawingsViewModel Drawings
        {
            get;
            private set;
        }

        /// <summary>
        /// The maps user data
        /// </summary>
        public MapUserData UserData
        {
            get { return this.userData; }
        }

        /// <summary>
        /// Constructs a new MapViewModel
        /// </summary>
        [ImportingConstructor]
        public MapViewModel(IZoneCompletionController zoneController, IZoneService zoneService, IPlayerService playerService,
            PlayerMarkersViewModel mapMarkers, ZoneItemsViewModel zoneItems, MapUserData userData)
        {
            this.zoneController = zoneController;
            this.zoneService = zoneService;
            this.userData = userData;

            this.CharacterPointer = new CharacterPointerViewModel(zoneController, userData);
            this.CharacterPointer.PropertyChanged += CharacterPointer_PropertyChanged;
            this.Drawings = new DrawingsViewModel(this.CharacterPointer, zoneService, playerService, userData);

            this.MapMarkers = mapMarkers;
            this.ZoneItems = zoneItems;

            // Make sure the zone service is ready
            this.zoneService.Initialize();

            if (playerService.HasValidMapId)
                this.ContinentData = this.zoneService.GetContinentByMap(playerService.MapId);
            else
                this.ContinentData = this.zoneService.GetContinent(DEFAULT_CONTINENT_ID);

            this.FloorId = 1;
            if (playerService.HasValidMapId)
                this.FloorId = this.zoneService.GetMap(playerService.MapId).DefaultFloor;

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
                if (this.ContinentData.Id != this.zoneController.ActiveContinent.Id)
                {
                    this.ContinentData = this.zoneController.ActiveContinent;
                    this.MapMarkers.OnContinentChanged(this.ContinentData.Id);
                    this.Drawings.OnContinentChanged(this.ContinentData.Id);
                }
            }
            else if (e.PropertyName == ReflectionUtility.GetPropertyName(() => this.zoneController.ActiveMap))
            {
                this.FloorId = this.zoneController.ActiveMap.DefaultFloor;
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

        /// <summary>
        /// Clears the in-memory cache of the map tiles
        /// </summary>
        private void ClearMapCache()
        {
            TileImageLoader.MinimumCacheExpiration = TimeSpan.FromTicks(1);
            TileImageLoader.DefaultCacheExpiration = TimeSpan.FromTicks(1);
        }

        /// <summary>
        /// Resets default in-memory map cache expiration times
        /// </summary>
        private void ResetMapCacheExpiration()
        {
            TileImageLoader.MinimumCacheExpiration = TimeSpan.FromHours(1);
            TileImageLoader.DefaultCacheExpiration = TimeSpan.FromHours(1);
        }
    }
}
