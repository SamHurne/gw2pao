using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Enums;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.Data;
using GW2PAO.Data.UserData;
using GW2PAO.PresentationCore;
using GW2PAO.ViewModels.Interfaces;
using NLog;

namespace GW2PAO.ViewModels.WvWTracker
{
    public class WvWTrackerViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The WvW controller
        /// </summary>
        private IWvWController controller;

        /// <summary>
        /// WvW Map view model object
        /// </summary>
        public IHasWvWMap WvWMapVM { get; private set; }

        /// <summary>
        /// Collection of WvW objectives for the configured map
        /// </summary>
        public ObservableCollection<WvWObjectiveViewModel> Objectives { get { return this.controller.CurrentObjectives; } }

        /// <summary>
        /// Command to reset all hidden objectives
        /// </summary>
        public DelegateCommand ResetHiddenObjectivesCommand { get { return new DelegateCommand(this.ResetHiddenObjectives); } }

        /// <summary>
        /// True if the view should use a vertical orientation, else false
        /// Note: I would prefer to have this in the view, but we save it the user settings
        /// </summary>
        public bool IsVerticalOrientation
        {
            get { return !this.UserData.IsTrackerHorizontal; }
            set
            {
                if (value)
                {
                    this.UserData.IsTrackerHorizontal = false;
                    this.NotifyOrientationSelectionChanged();
                }
            }
        }

        /// <summary>
        /// True if the view should use a horizontal orientation, else false
        /// Note: I would prefer to have this in the view, but we save it the user settings
        /// </summary>
        public bool IsHorizontalOrientation
        {
            get { return this.UserData.IsTrackerHorizontal; }
            set
            {
                if (value)
                {
                    this.UserData.IsTrackerHorizontal = true;
                    this.NotifyOrientationSelectionChanged();
                }
            }
        }

        /// <summary>
        /// True if the controller's current map is not overriden, else false
        /// </summary>
        public bool IsUsingPlayerMap
        {
            get { return this.controller.MapOverride == WvWMap.Unknown; }
            set
            {
                if (value)
                {
                    this.controller.MapOverride = WvWMap.Unknown;
                    this.NotifyMapSelectionChanged();
                }
            }
        }

        /// <summary>
        /// True if the controller's current map is overriden as Red Borderlands, else false
        /// </summary>
        public bool IsUsingRedBorderlands
        {
            get { return this.controller.MapOverride == WvWMap.RedBorderlands; }
            set
            {
                if (value)
                {
                    this.controller.MapOverride = WvWMap.RedBorderlands;
                    this.NotifyMapSelectionChanged();
                }
            }
        }

        /// <summary>
        /// True if the controller's current map is overriden as Blue Borderlands, else false
        /// </summary>
        public bool IsUsingBlueBorderlands
        {
            get { return this.controller.MapOverride == WvWMap.BlueBorderlands; }
            set
            {
                if (value)
                {
                    this.controller.MapOverride = WvWMap.BlueBorderlands;
                    this.NotifyMapSelectionChanged();
                }
            }
        }

        /// <summary>
        /// True if the controller's current map is overriden as Green Borderlands, else false
        /// </summary>
        public bool IsUsingGreenBorderlands
        {
            get { return this.controller.MapOverride == WvWMap.GreenBorderlands; }
            set
            {
                if (value)
                {
                    this.controller.MapOverride = WvWMap.GreenBorderlands;
                    this.NotifyMapSelectionChanged();
                }
            }
        }

        /// <summary>
        /// True if the controller's current map is overriden as Eternal Battlegrounds, else false
        /// </summary>
        public bool IsUsingEternalBattlegrounds
        {
            get { return this.controller.MapOverride == WvWMap.EternalBattlegrounds; }
            set
            {
                if (value)
                {
                    this.controller.MapOverride = WvWMap.EternalBattlegrounds;
                    this.NotifyMapSelectionChanged();
                }
            }
        }

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
        /// True if cardinal directions should be shown, else false
        /// </summary>
        public bool IsCardinalDirectionsSelected
        {
            get { return !this.UserData.AreShortNamesShown; }
            set
            {
                this.UserData.AreShortNamesShown = !value;
                this.NotifyDisplayedNamesSelectionChanged();
            }
        }

        /// <summary>
        /// True if short names should be shown, else false
        /// </summary>
        public bool IsShortNamesSelected
        {
            get { return this.UserData.AreShortNamesShown; }
            set
            {
                this.UserData.AreShortNamesShown = value;
                this.NotifyDisplayedNamesSelectionChanged();
            }
        }

        /// <summary>
        /// WvW user data
        /// </summary>
        public WvWUserData UserData { get { return this.controller.UserData; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="wvwController">The WvW controller</param>
        /// <param name="map">Map for this view model</param>
        public WvWTrackerViewModel(IWvWController wvwController, IHasWvWMap wvwMapVM)
        {
            this.controller = wvwController;
            this.WvWMapVM = wvwMapVM;
        }

        /// <summary>
        /// Resets all hidden objectives
        /// </summary>
        private void ResetHiddenObjectives()
        {
            logger.Debug("Resetting hidden objectives");
            this.UserData.HiddenObjectives.Clear();
        }

        /// <summary>
        /// Raises property changed events for all of the map selection properties
        /// </summary>
        private void NotifyMapSelectionChanged()
        {
            this.RaisePropertyChanged("IsUsingPlayerMap");
            this.RaisePropertyChanged("IsUsingRedBorderlands");
            this.RaisePropertyChanged("IsUsingBlueBorderlands");
            this.RaisePropertyChanged("IsUsingGreenBorderlands");
            this.RaisePropertyChanged("IsUsingEternalBattlegrounds");
        }

        /// <summary>
        /// Raises property changed events for all of the orientation selection properties
        /// </summary>
        private void NotifyOrientationSelectionChanged()
        {
            this.RaisePropertyChanged("IsVerticalOrientation");
            this.RaisePropertyChanged("IsHorizontalOrientation");
        }

        /// <summary>
        /// Raises property changed events for all of the unit selection properties
        /// </summary>
        private void NotifyUnitsSelectionChanged()
        {
            this.RaisePropertyChanged("IsFeetSelected");
            this.RaisePropertyChanged("IsMetersSelected");
            this.RaisePropertyChanged("IsTimeDistanceSelected");
        }

        /// <summary>
        /// Raises property changed events for all of the name selection properties
        /// </summary>
        private void NotifyDisplayedNamesSelectionChanged()
        {
            this.RaisePropertyChanged("IsCardinalDirectionsSelected");
            this.RaisePropertyChanged("IsShortNamesSelected");
        }
    }
}
