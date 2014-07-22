using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Enums;
using GW2PAO.Models;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.ViewModels.ZoneCompletion
{
    /// <summary>
    /// View model for zone items/points
    /// </summary>
    public class ZoneItemViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private double distanceFromPlayer;
        private double directionFromPlayer;
        private bool isVisible;
        private ZoneCompletionSettings userSettings;

        /// <summary>
        /// The primary model object containing the zone item's information
        /// </summary>
        public ZoneItem ItemModel { get; private set; }

        /// <summary>
        /// Zone item/point's ID
        /// </summary>
        public int ItemId { get { return this.ItemModel.ID; } }

        /// <summary>
        /// Zone item/point's Name
        /// </summary>
        public string ItemName
        {
            get
            {
                return this.ItemModel.Name;
            }
        }

        /// <summary>
        /// Zone item/point's Type
        /// </summary>
        public ZoneItemType ItemType
        {
            get { return this.ItemModel.Type; }
        }

        /// <summary>
        /// Distance of the item/point from the player, in ft
        /// </summary>
        public double DistanceFromPlayer
        {
            get { return this.distanceFromPlayer; }
            set { SetField(ref this.distanceFromPlayer, value); }
        }

        /// <summary>
        /// Direction/angle of the zone item/point from the player, based on the location and camera position of the player
        /// </summary>
        public double DirectionFromPlayer
        {
            get { return this.directionFromPlayer; }
            set { SetField(ref this.directionFromPlayer, value); }
        }

        /// <summary>
        /// Unlocked state of this zone item/point
        /// </summary>
        public bool IsUnlocked
        {
            get { return this.userSettings.UnlockedZoneItems.Contains(this.ItemModel); }
            set
            {
                if (value && !this.userSettings.UnlockedZoneItems.Contains(this.ItemModel))
                {
                    logger.Debug("Adding \"{0}\" to UnlockedZoneItems", this.ItemId);
                    this.userSettings.UnlockedZoneItems.Add(this.ItemModel);
                    this.RaisePropertyChanged();
                }
                else if (this.userSettings.UnlockedZoneItems.Contains(this.ItemModel))
                {
                    logger.Debug("Removing \"{0}\" from UnlockedZoneItems", this.ItemId);
                    this.userSettings.UnlockedZoneItems.Remove(this.ItemModel);
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Visibility of the zone item/point
        /// Visibility is based on multiple properties, including:
        ///     - Whether or not this item/points type is configured as shown
        ///     - IsUnlocked and whether or not unlocked item/points are shown
        ///     - Whether or not the event is user-configured as hidden
        /// </summary>
        public bool IsVisible
        {
            get { return this.isVisible; }
            set { SetField(ref this.isVisible, value); }
        }

        /// <summary>
        /// Command to hide this item/point
        /// </summary>
        public DelegateCommand HideCommand { get { return new DelegateCommand(this.AddToHiddenItems); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="zoneItem">The zone item/point's information</param>
        /// <param name="userSettings">User settings</param>
        public ZoneItemViewModel(ZoneItem zoneItem, ZoneCompletionSettings userSettings)
        {
            this.DistanceFromPlayer = -1;
            this.ItemModel = zoneItem;
            this.userSettings = userSettings;
            this.userSettings.PropertyChanged += (o, e) => this.RefreshVisibility();
            this.userSettings.HiddenZoneItems.CollectionChanged += (o, e) => this.RefreshVisibility();
            this.userSettings.UnlockedZoneItems.CollectionChanged += UnlockedZoneItems_CollectionChanged;
            this.RefreshVisibility();
        }

        /// <summary>
        /// Handles the collection changed event of the user setting's UnlockZoneItems collection
        /// Depending on the user settings, visibility of this item may change if it is unlocked
        /// </summary>
        private void UnlockedZoneItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            const string isUnlockedPropertyName = "IsUnlocked";

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems.Contains(this.ItemModel))
                {
                    this.OnPropertyChanged(isUnlockedPropertyName);
                    this.RefreshVisibility();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems.Contains(this.ItemModel))
                {
                    this.OnPropertyChanged(isUnlockedPropertyName);
                    this.RefreshVisibility();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.OnPropertyChanged(isUnlockedPropertyName);
                this.RefreshVisibility();
            }
        }

        /// <summary>
        /// Adds this item/point to the list of hidden zone item/points
        /// </summary>
        private void AddToHiddenItems()
        {
            logger.Debug("Adding {0} to HiddenZoneItems", this.ItemId);
            this.userSettings.HiddenZoneItems.Add(this.ItemModel);
        }

        /// <summary>
        /// Refreshes the visibility of this zone item/point
        /// </summary>
        private void RefreshVisibility()
        {
            logger.Trace("Refreshing visibility of \"{0}\"", this.ItemId);
            if (this.userSettings.HiddenZoneItems.Any(item => item.ID == this.ItemId))
            {
                this.IsVisible = false;
            }
            else if (this.IsUnlocked && !this.userSettings.ShowUnlockedPoints)
            {
                this.IsVisible = false;
            }
            else
            {
                switch (this.ItemType)
                {
                    case ZoneItemType.HeartQuest:
                        this.IsVisible = this.userSettings.AreHeartsVisible;
                        break;
                    case ZoneItemType.PointOfInterest:
                        this.IsVisible = this.userSettings.ArePoisVisible;
                        break;
                    case ZoneItemType.SkillChallenge:
                        this.IsVisible = this.userSettings.AreSkillChallengesVisible;
                        break;
                    case ZoneItemType.Vista:
                        this.IsVisible = this.userSettings.AreVistasVisible;
                        break;
                    case ZoneItemType.Waypoint:
                        this.IsVisible = this.userSettings.AreWaypointsVisible;
                        break;
                    default:
                        this.IsVisible = true;
                        break;
                }
            }
            logger.Trace("IsVisible = {0}", this.IsVisible);
        }
    }
}
