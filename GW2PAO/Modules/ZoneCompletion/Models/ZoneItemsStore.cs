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

                Threading.BeginInvokeOnUI(() =>
                {
                    continent.Value.Dungeons.Clear();
                    continent.Value.HeartQuests.Clear();
                    continent.Value.HeroPoints.Clear();
                    continent.Value.POIs.Clear();
                    continent.Value.Vistas.Clear();
                    continent.Value.Waypoints.Clear();
                });

                foreach (var entity in zoneItems)
                {
                    var vm = new ZoneItemViewModel(entity, this.playerService, this.zoneUserData);
                    Threading.BeginInvokeOnUI(() =>
                    {
                        switch (entity.Type)
                        {
                            case API.Data.Enums.ZoneItemType.Dungeon:
                                continent.Value.Dungeons.Add(vm);
                                break;
                            case API.Data.Enums.ZoneItemType.HeartQuest:
                                continent.Value.HeartQuests.Add(vm);
                                break;
                            case API.Data.Enums.ZoneItemType.HeroPoint:
                                continent.Value.HeroPoints.Add(vm);
                                break;
                            case API.Data.Enums.ZoneItemType.PointOfInterest:
                                continent.Value.POIs.Add(vm);
                                break;
                            case API.Data.Enums.ZoneItemType.Vista:
                                continent.Value.Vistas.Add(vm);
                                break;
                            case API.Data.Enums.ZoneItemType.Waypoint:
                                continent.Value.Waypoints.Add(vm);
                                break;
                            default:
                                break;
                        }
                    });
                }
            }
        }
    }

    public class ContinentZoneItems
    {
        public int ContinentId
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Waypoints for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> Waypoints
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Points of Interest for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> POIs
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Vistas for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> Vistas
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Heart Quests for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> HeartQuests
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Hero Points for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> HeroPoints
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of Dungeons for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> Dungeons
        {
            get;
            private set;
        }

        public ContinentZoneItems(int continentId)
        {
            this.ContinentId = continentId;

            this.Waypoints = new ObservableCollection<ZoneItemViewModel>();
            this.Vistas = new ObservableCollection<ZoneItemViewModel>();
            this.POIs = new ObservableCollection<ZoneItemViewModel>();
            this.HeroPoints = new ObservableCollection<ZoneItemViewModel>();
            this.HeartQuests = new ObservableCollection<ZoneItemViewModel>();
            this.Dungeons = new ObservableCollection<ZoneItemViewModel>();
        }
    }

}
