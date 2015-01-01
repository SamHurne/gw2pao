using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;
using GW2PAO.Data;
using GW2PAO.Data.UserData;
using GW2PAO.Modules.ZoneCompletion.Interfaces;
using GW2PAO.PresentationCore;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GW2PAO.Modules.ZoneCompletion.ViewModels
{
    /// <summary>
    /// Primatry Zone Completion Assistant view model class
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class ZoneCompletionViewModel : BindableBase
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
        [Import]
        public IHasZoneName ZoneNameVM { get; private set; }

        /// <summary>
        /// True if the selected distance units are Feet, else false
        /// </summary>
        public bool IsFeetSelected
        {
            get { return this.UserData.DistanceUnits == Units.Feet; }
            set
            {
                if (value)
                {
                    this.UserData.DistanceUnits = Units.Feet;
                    this.NotifyUnitsSelectionChanged();
                }
            }
        }

        /// <summary>
        /// True if the selected distance units are Meters, else false
        /// </summary>
        public bool IsMetersSelected
        {
            get { return this.UserData.DistanceUnits == Units.Meters; }
            set
            {
                if (value)
                {
                    this.UserData.DistanceUnits = Units.Meters;
                    this.NotifyUnitsSelectionChanged();
                }
            }
        }

        /// <summary>
        /// True if the selected distance units are Time-Distances, else false
        /// </summary>
        public bool IsTimeDistanceSelected
        {
            get { return this.UserData.DistanceUnits == Units.TimeDistance; }
            set
            {
                if (value)
                {
                    this.UserData.DistanceUnits = Units.TimeDistance;
                    this.NotifyUnitsSelectionChanged();
                }
            }
        }

        /// <summary>
        /// The user settings
        /// </summary>
        public ZoneCompletionUserData UserData { get { return this.controller.UserData; } }

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
        [ImportingConstructor]
        public ZoneCompletionViewModel(IZoneCompletionController zoneCompletionController)
        {
            this.controller = zoneCompletionController;
            this.controller.Start();
        }

        /// <summary>
        /// Shuts down the ViewModel for the zone completion assistant
        /// </summary>
        public void Shutdown()
        {
            this.controller.Stop();
        }

        /// <summary>
        /// Resets the unlocked items/points in the current zone
        /// </summary>
        private void ResetUnlockedPoints_Zone()
        {
            logger.Debug("Resetting unlocked points for zone {0}", this.controller.CurrentMapID);
            List<ZoneItem> toRemove = new List<ZoneItem>();

            // Only reset unlocked points for this character
            var characterItems = this.UserData.UnlockedZoneItems.FirstOrDefault(czi => czi.Character == this.controller.CharacterName);
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
            this.UserData.UnlockedZoneItems.Clear();
        }

        /// <summary>
        /// Resets the hidden items/points in the current zone
        /// </summary>
        private void ResetHiddenPoints_Zone()
        {
            logger.Debug("Resetting hidden points for zone {0}", this.controller.CurrentMapID);
            List<ZoneItem> toRemove = new List<ZoneItem>();
            foreach (var zoneItem in this.UserData.HiddenZoneItems)
            {
                if (zoneItem.MapId == this.controller.CurrentMapID)
                    toRemove.Add(zoneItem);
            }

            foreach (var zoneItem in toRemove)
            {
                this.UserData.HiddenZoneItems.Remove(zoneItem);
            }
        }

        /// <summary>
        /// Globally resets the hidden items/points
        /// </summary>
        private void ResetHiddenPoints_Global()
        {
            logger.Debug("Globally resetting hidden points");
            this.UserData.HiddenZoneItems.Clear();
        }

        /// <summary>
        /// Sets all types of zone items/points as visible
        /// </summary>
        private void ShowAllPoints()
        {
            logger.Debug("Showing all points");
            this.UserData.AreWaypointsVisible = true;
            this.UserData.ArePoisVisible = true;
            this.UserData.AreHeartsVisible = true;
            this.UserData.AreVistasVisible = true;
            this.UserData.AreSkillChallengesVisible = true;
        }

        /// <summary>
        /// Sets all types of zone items/points as not visible
        /// </summary>
        private void ShowNoPoints()
        {
            logger.Debug("Showing no points");
            this.UserData.AreWaypointsVisible = false;
            this.UserData.ArePoisVisible = false;
            this.UserData.AreHeartsVisible = false;
            this.UserData.AreVistasVisible = false;
            this.UserData.AreSkillChallengesVisible = false;
        }

        /// <summary>
        /// Raises property changed events for all of the unit selection properties
        /// </summary>
        private void NotifyUnitsSelectionChanged()
        {
            this.OnPropertyChanged(() => this.IsFeetSelected);
            this.OnPropertyChanged(() => this.IsMetersSelected);
            this.OnPropertyChanged(() => this.IsTimeDistanceSelected);
        }
    }
}
