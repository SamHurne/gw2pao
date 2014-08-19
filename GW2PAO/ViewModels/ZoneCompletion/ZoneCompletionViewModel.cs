using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Enums;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.Models;
using GW2PAO.ViewModels.Interfaces;
using GW2PAO.PresentationCore;
using NLog;
using GW2PAO.Utility;

namespace GW2PAO.ViewModels.ZoneCompletion
{
    /// <summary>
    /// Primatry Zone Completion Assistant view model class
    /// </summary>
    public class ZoneCompletionViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The Zone Completion Assistant controller
        /// </summary>
        private IZoneCompletionController controller;

        /// <summary>
        /// Collection of items/points in the current zone
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> ZoneItems { get { return this.controller.ZoneItems; } }

        /// <summary>
        /// Zone Name view model object
        /// </summary>
        public IHasZoneName ZoneNameVM { get; private set; }

        /// <summary>
        /// True if the selected distance units are Feet, else false
        /// </summary>
        public bool IsFeetSelected
        {
            get { return this.UserSettings.DistanceUnits == Units.Feet; }
            set
            {
                if (value)
                {
                    this.UserSettings.DistanceUnits = Units.Feet;
                    this.RaisePropertyChanged();

                    // Also raise property changed for the other options
                    // Should also probably refresh the value on all zone items, but we'll let the controller just handle it
                    this.RaisePropertyChanged("IsMetersSelected");
                    this.RaisePropertyChanged("IsTimeDistanceSelected");
                }
            }
        }

        /// <summary>
        /// True if the selected distance units are Meters, else false
        /// </summary>
        public bool IsMetersSelected
        {
            get { return this.UserSettings.DistanceUnits == Units.Meters; }
            set
            {
                if (value)
                {
                    this.UserSettings.DistanceUnits = Units.Meters;
                    this.RaisePropertyChanged();

                    // Also raise property changed for the other options
                    // Should also probably refresh the value on all zone items, but we'll let the controller just handle it
                    this.RaisePropertyChanged("IsFeetSelected");
                    this.RaisePropertyChanged("IsTimeDistanceSelected");
                }
            }
        }

        /// <summary>
        /// True if the selected distance units are Time-Distances, else false
        /// </summary>
        public bool IsTimeDistanceSelected
        {
            get { return this.UserSettings.DistanceUnits == Units.TimeDistance; }
            set
            {
                if (value)
                {
                    this.UserSettings.DistanceUnits = Units.TimeDistance;
                    this.RaisePropertyChanged();

                    // Also raise property changed for the other options
                    // Should also probably refresh the value on all zone items, but we'll let the controller just handle it
                    this.RaisePropertyChanged("IsFeetSelected");
                    this.RaisePropertyChanged("IsMetersSelected");
                }
            }
        }

        /// <summary>
        /// The user settings
        /// </summary>
        public ZoneCompletionSettings UserSettings { get { return this.controller.UserSettings; } }

        /// <summary>
        /// Command to reset all unlocked zone items/points in the current zone
        /// </summary>
        public DelegateCommand ResetZoneUnlocksCommand { get { return new DelegateCommand(this.ResetUnlockedPoints_Zone); } }

        /// <summary>
        /// Command to globally reset all unlocked zone items/points
        /// </summary>
        public DelegateCommand ResetAllUnlocksCommand { get { return new DelegateCommand(this.ResetUnlockedPoints_Global); } }

        /// <summary>
        /// Command to reset all hidden zone items/points in the current zone
        /// </summary>
        public DelegateCommand ResetZoneHiddenCommand { get { return new DelegateCommand(this.ResetHiddenPoints_Zone); } }

        /// <summary>
        /// Command to globally reset all hidden zone items/points
        /// </summary>
        public DelegateCommand ResetAllHiddenCommand { get { return new DelegateCommand(this.ResetHiddenPoints_Global); } }

        /// <summary>
        /// Command to show all types of zone items/points
        /// </summary>
        public DelegateCommand ShowAllCommand { get { return new DelegateCommand(this.ShowAllPoints); } }

        /// <summary>
        /// Command to hide all types of zone items/points
        /// </summary>
        public DelegateCommand ShowNoneCommand { get { return new DelegateCommand(this.ShowNoPoints); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="zoneCompletionController">The zone completion controller</param>
        /// <param name="zoneNameVm">The zone name view model</param>
        public ZoneCompletionViewModel(IZoneCompletionController zoneCompletionController, IHasZoneName zoneNameVm)
        {
            this.controller = zoneCompletionController;
            this.ZoneNameVM = zoneNameVm;
        }

        /// <summary>
        /// Resets the unlocked items/points in the current zone
        /// </summary>
        private void ResetUnlockedPoints_Zone()
        {
            logger.Debug("Resetting unlocked points for zone {0}", this.controller.CurrentMapID);
            List<ZoneItem> toRemove = new List<ZoneItem>();

            // Only reset unlocked points for this character
            var characterItems = this.UserSettings.UnlockedZoneItems.FirstOrDefault(czi => czi.Character == this.controller.CharacterName);
            if (characterItems != null)
            {
                foreach (var zoneItem in characterItems.ZoneItems)
                {
                    if (zoneItem.MapId == this.controller.CurrentMapID)
                        toRemove.Add(zoneItem);
                }

                foreach (var zoneItem in toRemove)
                {
                    characterItems.ZoneItems.Remove(zoneItem);
                }
            }
        }

        /// <summary>
        /// Globally resets the unlocked items/points
        /// </summary>
        private void ResetUnlockedPoints_Global()
        {
            logger.Debug("Globally resetting unlocked points");
            this.UserSettings.UnlockedZoneItems.Clear();
        }

        /// <summary>
        /// Resets the hidden items/points in the current zone
        /// </summary>
        private void ResetHiddenPoints_Zone()
        {
            logger.Debug("Resetting hidden points for zone {0}", this.controller.CurrentMapID);
            List<ZoneItem> toRemove = new List<ZoneItem>();
            foreach (var zoneItem in this.UserSettings.HiddenZoneItems)
            {
                if (zoneItem.MapId == this.controller.CurrentMapID)
                    toRemove.Add(zoneItem);
            }

            foreach (var zoneItem in toRemove)
            {
                this.UserSettings.HiddenZoneItems.Remove(zoneItem);
            }
        }

        /// <summary>
        /// Globally resets the hidden items/points
        /// </summary>
        private void ResetHiddenPoints_Global()
        {
            logger.Debug("Globally resetting hidden points");
            this.UserSettings.HiddenZoneItems.Clear();
        }

        /// <summary>
        /// Sets all types of zone items/points as visible
        /// </summary>
        private void ShowAllPoints()
        {
            logger.Debug("Showing all points");
            this.UserSettings.AreWaypointsVisible = true;
            this.UserSettings.ArePoisVisible = true;
            this.UserSettings.AreHeartsVisible = true;
            this.UserSettings.AreVistasVisible = true;
            this.UserSettings.AreSkillChallengesVisible = true;
        }

        /// <summary>
        /// Sets all types of zone items/points as not visible
        /// </summary>
        private void ShowNoPoints()
        {
            logger.Debug("Showing no points");
            this.UserSettings.AreWaypointsVisible = false;
            this.UserSettings.ArePoisVisible = false;
            this.UserSettings.AreHeartsVisible = false;
            this.UserSettings.AreVistasVisible = false;
            this.UserSettings.AreSkillChallengesVisible = false;
        }
    }
}
