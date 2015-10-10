using GW2PAO.API.Data.Entities;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.API.Util;
using GW2PAO.Modules.ZoneCompletion;
using GW2PAO.Modules.ZoneCompletion.Interfaces;
using GW2PAO.Modules.ZoneCompletion.ViewModels;
using GW2PAO.Utility;
using MapControl;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

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

        private MapUserData userData;
        private ZoneCompletionUserData zoneUserData;
        private IZoneCompletionController zoneController;
        private IZoneService zoneService;
        private IPlayerService playerService;
        private int floorId;
        private Location mapCenter;
        private Location charLocation;
        private MercatorTransform locationTransform = new MercatorTransform();

        private bool snapToCharacter;

        /// <summary>
        /// The ID of the active continent to show on the map
        /// </summary>
        public int ContinentId
        {
            get
            {
                if (this.zoneController.ActiveContinent != null)
                    return this.zoneController.ActiveContinent.Id;
                else
                    return 1;
            }
        }

        /// <summary>
        /// Data for the active continent
        /// TODO: Do we really want this to be the active continent?
        /// </summary>
        public Continent ContinentData
        {
            get
            {
                return this.zoneController.ActiveContinent;
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
                return string.Format("https://tiles.guildwars2.com/{0}/{1}/{2}.jpg", this.ContinentId, this.FloorId, "{z}/{x}/{y}");
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
        /// The player character's location on the map
        /// </summary>
        public Location CharacterLocation
        {
            get { return this.charLocation; }
            set { SetProperty(ref this.charLocation, value); }
        }

        /// <summary>
        /// True if the map should snap to the active character's position, else false
        /// </summary>
        public bool SnapToCharacter
        {
            get { return this.snapToCharacter; }
            set
            {
                if (SetProperty(ref this.snapToCharacter, value))
                {
                    this.RefreshCharacterLocation();
                }
            }
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


        /// <summary>
        /// Constructs a new MapViewModel
        /// </summary>
        [ImportingConstructor]
        public MapViewModel(IZoneCompletionController zoneController, IZoneService zoneService, IPlayerService playerService,
            MapUserData userData, ZoneCompletionUserData zoneUserData)
        {
            this.zoneController = zoneController;
            this.playerService = playerService;
            this.zoneService = zoneService;
            this.zoneUserData = zoneUserData;
            this.userData = userData;
            this.FloorId = 1;
            this.SnapToCharacter = false;

            this.Dungeons = new ObservableCollection<ZoneItemViewModel>();
            this.HeartQuests = new ObservableCollection<ZoneItemViewModel>();
            this.HeroPoints = new ObservableCollection<ZoneItemViewModel>();
            this.POIs = new ObservableCollection<ZoneItemViewModel>();
            this.Vistas = new ObservableCollection<ZoneItemViewModel>();
            this.Waypoints = new ObservableCollection<ZoneItemViewModel>();

            ((INotifyPropertyChanged)this.zoneController).PropertyChanged += ZoneControllerPropertyChanged;
            this.zoneController.Start();

            // Start a task to retrieve all zone items across all zones
            Task.Factory.StartNew(this.RebuildZoneItemCollections);
        }

        private void RebuildZoneItemCollections()
        {
            var zoneItems = this.zoneService.GetZoneItemsByContinent(this.ContinentId);
            foreach (var entity in zoneItems)
            {
                var vm = new ZoneItemViewModel(entity, this.playerService, this.zoneUserData);

                Threading.InvokeOnUI(() =>
                {
                    switch (entity.Type)
                    {
                        case API.Data.Enums.ZoneItemType.Dungeon:
                            this.Dungeons.Add(vm);
                            break;
                        case API.Data.Enums.ZoneItemType.HeartQuest:
                            this.HeartQuests.Add(vm);
                            break;
                        case API.Data.Enums.ZoneItemType.HeroPoint:
                            this.HeroPoints.Add(vm);
                            break;
                        case API.Data.Enums.ZoneItemType.PointOfInterest:
                            this.POIs.Add(vm);
                            break;
                        case API.Data.Enums.ZoneItemType.Vista:
                            this.Vistas.Add(vm);
                            break;
                        case API.Data.Enums.ZoneItemType.Waypoint:
                            this.Waypoints.Add(vm);
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the Zone Controller
        /// </summary>
        private void ZoneControllerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.RefreshCharacterLocation();
        }

        private void RefreshCharacterLocation()
        {
            var charPos = this.zoneController.CharacterPosition;
            var cont = this.zoneController.ActiveContinent;
            var map = this.zoneController.ActiveMap;

            if (cont != null && map != null)
            {
                double charX = map.ContinentRectangle.X + (charPos.X - map.MapRectangle.X) * MapsHelper.MapToWorldRatio;
                double charY = map.ContinentRectangle.Y + ((map.MapRectangle.Y + map.MapRectangle.Height) - charPos.Y) * MapsHelper.MapToWorldRatio;

                var location = this.locationTransform.Transform(new System.Windows.Point(
                    (charX - (cont.Width / 2)) / cont.Width * 360.0,
                    ((cont.Height / 2) - charY) / cont.Height * 360.0));

                this.CharacterLocation = location;

                if (this.SnapToCharacter)
                    this.MapCenter = this.CharacterLocation;
            }
        }
    }
}
