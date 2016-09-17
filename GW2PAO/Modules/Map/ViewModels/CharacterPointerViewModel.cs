using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Util;
using GW2PAO.Modules.ZoneCompletion.Interfaces;
using GW2PAO.Utility;
using MapControl;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Map.ViewModels
{
    public class CharacterPointerViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IZoneCompletionController zoneController;
        private MapUserData userData;
        private Location charLocation;
        private double cameraDirection;
        private MercatorTransform locationTransform = new MercatorTransform();

        // TODO: Consider moving these to the UserData class
        private bool displayCharacterPointer;
        private bool canDisplayCharacterPointer;
        private bool snapToCharacter;
        private bool showPlayerTrail;
        private int playerTrailLength;

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
        /// Collection of location objects making up the player trail
        /// </summary>
        public ObservableCollection<Location> PlayerTrail
        {
            get;
            private set;
        }

        /// <summary>
        /// True if the player trail should be shown, else false
        /// </summary>
        public bool ShowPlayerTrail
        {
            get { return this.showPlayerTrail; }
            set { SetProperty(ref this.showPlayerTrail, value); }
        }

        /// <summary>
        /// Maximum length of the player trail to show before
        /// </summary>
        public int PlayerTrailLength
        {
            get { return this.playerTrailLength; }
            set
            {
                if (SetProperty(ref this.playerTrailLength, value))
                {
                    while (this.PlayerTrail.Count > value)
                    {
                        this.PlayerTrail.RemoveAt(0);
                    }
                }
            }
        }

        /// <summary>
        /// Constructs a new CharacterPointerViewModel
        /// </summary>
        public CharacterPointerViewModel(IZoneCompletionController zoneController, MapUserData userData)
        {
            this.zoneController = zoneController;
            this.userData = userData;

            this.PlayerTrail = new ObservableCollection<Location>();

            this.DisplayCharacterPointer = true;
            this.SnapToCharacter = false;
            this.ShowPlayerTrail = true;
            this.PlayerTrailLength = 100;
            this.CanDisplayCharacterPointer = this.zoneController.ValidMapID;
            this.RefreshCharacterLocation();
            this.RefreshCharacterDirection();

            string validMapProp = ReflectionUtility.GetPropertyName(() => this.zoneController.ValidMapID);
            string activeMapProp = ReflectionUtility.GetPropertyName(() => this.zoneController.ActiveMap);
            string charPosProp = ReflectionUtility.GetPropertyName(() => this.zoneController.CharacterPosition);
            string camDirProp = ReflectionUtility.GetPropertyName(() => this.zoneController.CameraDirection);
            ((INotifyPropertyChanged)this.zoneController).PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == validMapProp)
                {
                    this.CanDisplayCharacterPointer = this.zoneController.ValidMapID;
                }
                else if (e.PropertyName == activeMapProp)
                {
                    this.RefreshCharacterLocation();
                    this.RefreshCharacterDirection();
                }
                else if (e.PropertyName == charPosProp)
                {
                    this.RefreshCharacterLocation();
                }
                else if (e.PropertyName == camDirProp)
                {
                    this.RefreshCharacterDirection();
                }
            };
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

                if (this.CharacterLocation != location)
                {
                    if (this.CharacterLocation != null)
                    {
                        // If the location changed by a large amount, the player probably changed map or used a waypoint
                        // In that case, reset the player trail
                        var distance = CalcUtil.CalculateDistance(
                            new API.Data.Entities.Point(location.Longitude, location.Latitude),
                            new API.Data.Entities.Point(this.CharacterLocation.Longitude, this.CharacterLocation.Latitude));
                        if (distance > 1)
                        {
                            // Since we are dealing with lat/lng values, even a distance of 1 can be considered a 
                            // warp/teleport
                            this.PlayerTrail.Clear();
                        }
                        else
                        {
                            // Add the new location to the player trail
                            this.PlayerTrail.Add(location);
                            if (this.PlayerTrail.Count > this.PlayerTrailLength)
                                this.PlayerTrail.RemoveAt(0);
                        }
                    }

                    this.CharacterLocation = location;
                }
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
