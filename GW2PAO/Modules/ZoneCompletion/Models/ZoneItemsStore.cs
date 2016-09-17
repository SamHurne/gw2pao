using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Modules.ZoneCompletion.ViewModels;
using GW2PAO.Utility;

namespace GW2PAO.Modules.ZoneCompletion.Models
{
    [Export(typeof(ZoneItemsStore))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ZoneItemsStore
    {
        private IZoneService zoneService;
        private IPlayerService playerService;
        private ZoneCompletionUserData zoneUserData;

        /// <summary>
        /// Event raised when all data is finished loading
        /// </summary>
        public event EventHandler DataLoaded;

        /// <summary>
        /// Dictionary containing all zone items for each continent
        /// Key: Continent ID
        /// Value: Zone items for the corresponding continent
        /// </summary>
        public Dictionary<int, ContinentZoneItems> Data
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructs a new ZoneItemsStore
        /// </summary>
        /// <param name="zoneService">The zone service used for retriving zone items data</param>
        /// <param name="zoneUserData">Zone completion user data</param>
        [ImportingConstructor]
        public ZoneItemsStore(IZoneService zoneService, IPlayerService playerService, ZoneCompletionUserData zoneUserData)
        {
            this.zoneService = zoneService;
            this.playerService = playerService;
            this.zoneUserData = zoneUserData;

            this.Data = new Dictionary<int, ContinentZoneItems>();

            var continents = this.zoneService.GetContinents();
            foreach (var continent in continents)
            {
                this.Data.Add(continent.Id, new ContinentZoneItems(continent.Id));
            }
        }

        /// <summary>
        /// Initializes the store of zone items with data
        /// </summary>
        public void InitializeStore()
        {
            this.zoneService.Initialize();

            foreach (var continent in this.Data)
            {
                var zoneItems = this.zoneService.GetZoneItemsByContinent(continent.Key);

                Threading.InvokeOnUI(() => continent.Value.Clear());

                foreach (var entity in zoneItems)
                {
                    var zoneItem = new ZoneItemViewModel(entity, this.playerService, this.zoneUserData);
                    Threading.InvokeOnUI(() =>
                    {
                        continent.Value.Add(zoneItem);
                    });
                }
            }

            Threading.BeginInvokeOnUI(() => this.RaiseDataLoadedEvent());
        }

        private void RaiseDataLoadedEvent()
        {
            if (this.DataLoaded != null)
                this.DataLoaded(this, new EventArgs());
        }
    }

    /// <summary>
    /// Zone items container for an entire continent
    /// </summary>
    public class ContinentZoneItems
    {
        public int ContinentId
        {
            get;
            private set;
        }

        /// <summary>
        /// Zone items organized by map ID
        /// </summary>
        public ObservableDictionary<int, ZoneItems> ZoneItemsByMap
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Waypoints for the continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> Waypoints
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Points of Interest for the continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> POIs
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Vistas for the continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> Vistas
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Heart Quests for the continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> HeartQuests
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Hero Points for the continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> HeroPoints
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Dungeons for the continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> Dungeons
        {
            get;
            private set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="continentId">ID of the continent</param>
        public ContinentZoneItems(int continentId)
        {
            this.ContinentId = continentId;
            this.ZoneItemsByMap = new ObservableDictionary<int, ZoneItems>();

            this.Waypoints = new ObservableCollection<ZoneItemViewModel>();
            this.Vistas = new ObservableCollection<ZoneItemViewModel>();
            this.POIs = new ObservableCollection<ZoneItemViewModel>();
            this.HeroPoints = new ObservableCollection<ZoneItemViewModel>();
            this.HeartQuests = new ObservableCollection<ZoneItemViewModel>();
            this.Dungeons = new ObservableCollection<ZoneItemViewModel>();
        }

        /// <summary>
        /// Adds a new zone item/objective various collections 
        /// </summary>
        /// <param name="zoneItem">The zone item/objective to add</param>
        public void Add(ZoneItemViewModel zoneItem)
        {
            var mapId = zoneItem.ItemModel.MapId;
            if (!this.ZoneItemsByMap.ContainsKey(mapId))
                this.ZoneItemsByMap.Add(mapId, new ZoneItems(mapId));

            switch (zoneItem.ItemType)
            {
                case API.Data.Enums.ZoneItemType.Dungeon:
                    this.Dungeons.Add(zoneItem);
                    this.ZoneItemsByMap[mapId].Dungeons.Add(zoneItem);
                    break;
                case API.Data.Enums.ZoneItemType.HeartQuest:
                    this.HeartQuests.Add(zoneItem);
                    this.ZoneItemsByMap[mapId].HeartQuests.Add(zoneItem);
                    break;
                case API.Data.Enums.ZoneItemType.HeroPoint:
                    this.HeroPoints.Add(zoneItem);
                    this.ZoneItemsByMap[mapId].HeroPoints.Add(zoneItem);
                    break;
                case API.Data.Enums.ZoneItemType.PointOfInterest:
                    this.POIs.Add(zoneItem);
                    this.ZoneItemsByMap[mapId].POIs.Add(zoneItem);
                    break;
                case API.Data.Enums.ZoneItemType.Vista:
                    this.Vistas.Add(zoneItem);
                    this.ZoneItemsByMap[mapId].Vistas.Add(zoneItem);
                    break;
                case API.Data.Enums.ZoneItemType.Waypoint:
                    this.Waypoints.Add(zoneItem);
                    this.ZoneItemsByMap[mapId].Waypoints.Add(zoneItem);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Clears out all zone items/objectives
        /// </summary>
        public void Clear()
        {
            this.ZoneItemsByMap.Clear();
            this.Waypoints.Clear();
            this.Vistas.Clear();
            this.POIs.Clear();
            this.HeroPoints.Clear();
            this.HeartQuests.Clear();
            this.Dungeons.Clear();
        }
    }

    /// <summary>
    /// Zone items container for a single zone
    /// </summary>
    public class ZoneItems
    {
        public int MapId
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Waypoints for the zone
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> Waypoints
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Points of Interest for the zone
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> POIs
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Vistas for the zone
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> Vistas
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Heart Quests for the zone
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> HeartQuests
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Hero Points for the zone
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> HeroPoints
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Dungeons for the zone
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> Dungeons
        {
            get;
            private set;
        }

        public ZoneItems(int mapId)
        {
            this.MapId = mapId;

            this.Waypoints = new ObservableCollection<ZoneItemViewModel>();
            this.Vistas = new ObservableCollection<ZoneItemViewModel>();
            this.POIs = new ObservableCollection<ZoneItemViewModel>();
            this.HeroPoints = new ObservableCollection<ZoneItemViewModel>();
            this.HeartQuests = new ObservableCollection<ZoneItemViewModel>();
            this.Dungeons = new ObservableCollection<ZoneItemViewModel>();
        }
    }
}
