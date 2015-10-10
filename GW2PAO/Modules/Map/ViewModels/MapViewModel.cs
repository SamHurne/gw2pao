using GW2PAO.API.Util;
using GW2PAO.Modules.ZoneCompletion.Interfaces;
using MapControl;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using System.ComponentModel;
using System.ComponentModel.Composition;

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
        private IZoneCompletionController zoneController;
        private int continentId;
        private int floorId;
        private Location mapCenter;
        private Location charLocation;
        private MercatorTransform locationTransform = new MercatorTransform();

        private bool snapToCharacter;

        /// <summary>
        /// The active continent to show on the map
        /// </summary>
        public int ContinentId
        {
            get { return this.continentId; }
            set
            {
                if (SetProperty(ref this.continentId, value))
                {
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
        /// Constructs a new MapViewModel
        /// </summary>
        [ImportingConstructor]
        public MapViewModel(IZoneCompletionController zoneController, MapUserData userData)
        {
            this.zoneController = zoneController;
            this.userData = userData;
            this.ContinentId = 1;
            this.FloorId = 1;
            this.SnapToCharacter = false;

            ((INotifyPropertyChanged)this.zoneController).PropertyChanged += ZoneControllerPropertyChanged;
            this.zoneController.Start();
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
