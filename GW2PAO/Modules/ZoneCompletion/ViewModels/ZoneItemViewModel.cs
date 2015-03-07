using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Data;
using GW2PAO.Data.UserData;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Utility;

namespace GW2PAO.Modules.ZoneCompletion.ViewModels
{
    /// <summary>
    /// View model for zone items/points
    /// </summary>
    public class ZoneItemViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IPlayerService playerService;
        private double distanceFromPlayer;
        private double directionFromPlayer;
        private bool isVisible;
        private ZoneCompletionUserData userData;

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
        /// Distance of the item/point from the player
        /// Units depend on user selection
        /// </summary>
        public double DistanceFromPlayer
        {
            get { return this.distanceFromPlayer; }
            set { SetProperty(ref this.distanceFromPlayer, value); }
        }

        /// <summary>
        /// Direction/angle of the zone item/point from the player, based on the location and camera position of the player
        /// </summary>
        public double DirectionFromPlayer
        {
            get { return this.directionFromPlayer; }
            set { SetProperty(ref this.directionFromPlayer, value); }
        }

        /// <summary>
        /// Unlocked state of this zone item/point
        /// </summary>
        public bool IsUnlocked
        {
            get
            {
                bool isUnlocked = false;
                Threading.InvokeOnUI(() => // UnlockZoneItems should only ever be accessed from the UI thread
                    {
                        var characterItems = this.userData.UnlockedZoneItems.FirstOrDefault(czi => czi.Character == this.playerService.CharacterName);
                        if (characterItems != null)
                            isUnlocked = characterItems.ZoneItems.Contains(this.ItemModel);
                        else
                            isUnlocked = false;
                    });
                return isUnlocked;
            }
            set
            {
                Threading.InvokeOnUI(() => // UnlockZoneItems should only ever be accessed from the UI thread
                    {
                        var characterItems = this.userData.UnlockedZoneItems.FirstOrDefault(czi => czi.Character == this.playerService.CharacterName);
                        if (value) // Add to UnlockedZoneItems
                        {
                            if (characterItems != null)
                            {
                                // Already saved stuff for this character, add to collection for character
                                if (!characterItems.ZoneItems.Contains(this.ItemModel))
                                {
                                    characterItems.ZoneItems.Add(this.ItemModel);
                                    this.OnPropertyChanged(() => this.IsUnlocked);
                                }
                            }
                            else
                            {
                                // Nothing saved yet for this character, add character then add zone item
                                characterItems = new CharacterZoneItems() { Character = this.playerService.CharacterName };
                                characterItems.ZoneItems.Add(this.ItemModel);
                                this.userData.UnlockedZoneItems.Add(characterItems);
                                this.OnPropertyChanged(() => this.IsUnlocked);
                            }
                        }
                        else  // Remove from UnlockedZoneItems
                        {
                            if (characterItems != null)
                            {
                                if (characterItems.ZoneItems.Remove(this.ItemModel))
                                {
                                    this.OnPropertyChanged(() => this.IsUnlocked);
                                }
                            }
                        }
                    });
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
            set { SetProperty(ref this.isVisible, value); }
        }

        /// <summary>
        /// The zone completion assistant data
        /// </summary>
        public ZoneCompletionUserData UserData { get { return this.userData; } }

        /// <summary>
        /// Command to hide this item/point
        /// </summary>
        public DelegateCommand HideCommand { get { return new DelegateCommand(this.AddToHiddenItems); } }

        /// <summary>
        /// Command to copy the zone item's chat code to the clipboard
        /// </summary>
        public DelegateCommand CopyChatCodeCommand { get { return new DelegateCommand(this.CopyChatCode, this.CanCopyChatCode); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="zoneItem">The zone item/point's information</param>
        /// <param name="userData">User data</param>
        public ZoneItemViewModel(ZoneItem zoneItem, IPlayerService playerService, ZoneCompletionUserData userData)
        {
            this.DistanceFromPlayer = -1;
            this.ItemModel = zoneItem;
            this.playerService = playerService;
            this.userData = userData;
            this.userData.PropertyChanged += (o, e) => this.RefreshVisibility();

            // Set up handling for collection changed events on the unlocked zone items collections
            foreach (CharacterZoneItems charItems in this.userData.UnlockedZoneItems)
            {
                charItems.ZoneItems.CollectionChanged += UnlockedZoneItems_CollectionChanged;
            }
            this.userData.UnlockedZoneItems.CollectionChanged += (o, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (CharacterZoneItems itemAdded in e.NewItems)
                    {
                        itemAdded.ZoneItems.CollectionChanged += UnlockedZoneItems_CollectionChanged;
                    }
                }
            };
            this.userData.UnlockedZoneItems.CollectionChanged += UnlockedZoneItems_CollectionChanged;

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
            this.userData.HiddenZoneItems.Add(this.ItemModel);
        }

        /// <summary>
        /// Refreshes the visibility of this zone item/point
        /// </summary>
        private void RefreshVisibility()
        {
            logger.Trace("Refreshing visibility of \"{0}\"", this.ItemId);
            if (this.userData.HiddenZoneItems.Any(item => item.ID == this.ItemId))
            {
                this.IsVisible = false;
            }
            else if (this.IsUnlocked && !this.userData.ShowUnlockedPoints)
            {
                this.IsVisible = false;
            }
            else
            {
                switch (this.ItemType)
                {
                    case ZoneItemType.HeartQuest:
                        this.IsVisible = this.userData.AreHeartsVisible;
                        break;
                    case ZoneItemType.PointOfInterest:
                        this.IsVisible = this.userData.ArePoisVisible;
                        break;
                    case ZoneItemType.SkillChallenge:
                        this.IsVisible = this.userData.AreSkillChallengesVisible;
                        break;
                    case ZoneItemType.Vista:
                        this.IsVisible = this.userData.AreVistasVisible;
                        break;
                    case ZoneItemType.Waypoint:
                        this.IsVisible = this.userData.AreWaypointsVisible;
                        break;
                    default:
                        this.IsVisible = true;
                        break;
                }
            }
            logger.Trace("IsVisible = {0}", this.IsVisible);
        }

        /// <summary>
        /// Returns true if this zone item has a chat code, else false
        /// </summary>
        private bool CanCopyChatCode()
        {
            return !string.IsNullOrWhiteSpace(this.ItemModel.ChatCode);
        }

        /// <summary>
        /// Copies the item's chat code, if it has one
        /// </summary>
        private void CopyChatCode()
        {
            logger.Debug("Copying chat code of \"{0}\" as \"{1}\"", this.ItemName, this.ItemModel.ChatCode);
            System.Windows.Clipboard.SetDataObject(this.ItemModel.ChatCode);
        }
    }
}
