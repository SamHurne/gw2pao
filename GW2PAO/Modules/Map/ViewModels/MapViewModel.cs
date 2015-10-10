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

        private const int DEFAULT_CONTINENT_ID = 1;

        private MapUserData userData;
        private ZoneCompletionUserData zoneUserData;
        private IZoneCompletionController zoneController;
        private IZoneService zoneService;
        private IPlayerService playerService;
        private int floorId;
        private Continent continentData;
        private Location mapCenter;
        private Location charLocation;
        private double cameraDirection;
        private MercatorTransform locationTransform = new MercatorTransform();

        private bool displayCharacterPointer;
        private bool canDisplayCharacterPointer;

        private bool snapToCharacter;

        /// <summary>
        /// Data for the displayed continent
        /// </summary>
        public Continent ContinentData
        {
            get { return this.continentData; }
            set { SetProperty(ref this.continentData, value); }
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
        /// The player character's location on the map
        /// </summary>
        public Location CharacterLocation
        {
            get { return this.charLocation; }
            set { SetProperty(ref this.charLocation, value); }
        }

        /// <summary>
        /// Direction of the player's camera, in degrees
        /// </summary>
        public double CameraDirection
        {
            get { return this.cameraDirection; }
            set { SetProperty(ref this.cameraDirection, value); }
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
        /// True if the character pointer should be displayed (user-selectable), else false
        /// </summary>
        public bool DisplayCharacterPointer
        {
            get
            {
                if (this.CanDisplayCharacterPointer)
                    return this.displayCharacterPointer;
                else
                    return false;
            }
            set
            {
                SetProperty(ref this.displayCharacterPointer, value);
            }
        }

        /// <summary>
        /// True if we can display the character pointer, else false
        /// Can be false if the player is not in-game
        /// </summary>
        public bool CanDisplayCharacterPointer
        {
            get { return this.canDisplayCharacterPointer; }
            set
            {
                if (SetProperty(ref this.canDisplayCharacterPointer, value))
                {
                    this.OnPropertyChanged(() => this.DisplayCharacterPointer);
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
            this.DisplayCharacterPointer = true;

            this.Dungeons = new ObservableCollection<ZoneItemViewModel>();
            this.HeartQuests = new ObservableCollection<ZoneItemViewModel>();
            this.HeroPoints = new ObservableCollection<ZoneItemViewModel>();
            this.POIs = new ObservableCollection<ZoneItemViewModel>();
            this.Vistas = new ObservableCollection<ZoneItemViewModel>();
            this.Waypoints = new ObservableCollection<ZoneItemViewModel>();

            if (this.playerService.HasValidMapId)
                this.ContinentData = this.zoneService.GetContinentByMap(this.playerService.MapId);
            else
                this.ContinentData = this.zoneService.GetContinent(DEFAULT_CONTINENT_ID);

            ((INotifyPropertyChanged)this.zoneController).PropertyChanged += ZoneControllerPropertyChanged;
            this.zoneController.Start();

            // Start a task to retrieve all zone items across all zones
            Task.Factory.StartNew(this.RebuildZoneItemCollections);
        }

        /// <summary>
        /// Rebuilds each of the zone item collections
        /// </summary>
        private void RebuildZoneItemCollections()
        {
            var zoneItems = this.zoneService.GetZoneItemsByContinent(this.ContinentData.Id);
            Threading.BeginInvokeOnUI(() =>
            {
                this.Dungeons.Clear();
                this.HeartQuests.Clear();
                this.HeroPoints.Clear();
                this.POIs.Clear();
                this.Vistas.Clear();
                this.Waypoints.Clear();

                foreach (var entity in zoneItems)
                {
                    var vm = new ZoneItemViewModel(entity, this.playerService, this.zoneUserData);
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
                }
            });
        }

        /// <summary>
        /// Handles the PropertyChanged event of the Zone Controller
        /// </summary>
        private void ZoneControllerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveMap")
                this.ContinentData = this.zoneService.GetContinentByMap(this.zoneController.CurrentMapID);

            this.CanDisplayCharacterPointer = this.zoneController.ValidMapID;
            this.RefreshCharacterLocation();
            this.RefreshCharacterDirection();
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

        private void RefreshCharacterDirection()
        {
            var camDir = this.zoneController.CameraDirection;

            var zeroPoint = new API.Data.Entities.Point(0, 0);
            var newAngle = CalcUtil.CalculateAngle(CalcUtil.Vector.CreateVector(zeroPoint, camDir),
                                                   CalcUtil.Vector.CreateVector(zeroPoint, zeroPoint));
            this.CameraDirection = newAngle;
        }
    }
}
