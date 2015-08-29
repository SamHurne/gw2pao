using GW2PAO.API.Data.Enums;
using GW2PAO.Modules.WvW.Interfaces;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GW2PAO.Modules.WvW.ViewModels.WvWTracker
{
    [Export(typeof(WvWTrackerViewModel))]
    public class WvWTrackerViewModel : BindableBase
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
        public AutoRefreshCollectionViewSource Objectives
        {
            get;
            private set;
        }

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
        /// True if the objectives should be sorted by objective Type, else false
        /// </summary>
        public bool SortByType
        {
            get
            {
                return this.UserData.ObjectivesSortProperty == "Type";
            }
            set
            {
                if (this.UserData.ObjectivesSortProperty != "Type")
                {
                    this.OnSortingPropertyChanged("Type", ListSortDirection.Ascending);
                }
            }
        }

        /// <summary>
        /// True if the objectives should be sorted by Distance, else false
        /// </summary>
        public bool SortByDistance
        {
            get
            {
                return this.UserData.ObjectivesSortProperty == "DistanceFromPlayer";
            }
            set
            {
                if (this.UserData.ObjectivesSortProperty != "DistanceFromPlayer")
                {
                    this.OnSortingPropertyChanged("DistanceFromPlayer", ListSortDirection.Ascending);
                }
            }
        }

        /// <summary>
        /// True if the objectives should be sorted by Name, else false
        /// </summary>
        public bool SortByName
        {
            get
            {
                return this.UserData.ObjectivesSortProperty == "ShortName";
            }
            set
            {
                if (this.UserData.ObjectivesSortProperty != "ShortName")
                {
                    this.OnSortingPropertyChanged("ShortName", ListSortDirection.Ascending);
                }
            }
        }

        /// <summary>
        /// True if the objectives should be sorted by Location, else false
        /// </summary>
        public bool SortByLocation
        {
            get
            {
                return this.UserData.ObjectivesSortProperty == "Location";
            }
            set
            {
                if (this.UserData.ObjectivesSortProperty != "Location")
                {
                    this.OnSortingPropertyChanged("Location", ListSortDirection.Ascending);
                }
            }
        }

        /// <summary>
        /// True if the objectives should be sorted by World Owner, else false
        /// </summary>
        public bool SortByOwner
        {
            get
            {
                return this.UserData.ObjectivesSortProperty == "WorldOwner";
            }
            set
            {
                if (this.UserData.ObjectivesSortProperty != "WorldOwner")
                {
                    this.OnSortingPropertyChanged("WorldOwner", ListSortDirection.Ascending);
                }
            }
        }

        /// <summary>
        /// The name of the Red world in the current match
        /// </summary>
        public WvWTeamViewModel RedTeam
        {
            get
            {
                var teams = this.controller.Worlds.Where(w => w.MatchId == this.controller.MatchID);
                return teams.FirstOrDefault(w => w.Color == WorldColor.Red);
            }
        }

        /// <summary>
        /// The name of the Blue world in the current match
        /// </summary>
        public WvWTeamViewModel BlueTeam
        {
            get
            {
                var teams = this.controller.Worlds.Where(w => w.MatchId == this.controller.MatchID);
                return teams.FirstOrDefault(w => w.Color == WorldColor.Blue);
            }
        }

        /// <summary>
        /// The name of the Green world in the current match
        /// </summary>
        public WvWTeamViewModel GreenTeam
        {
            get
            {
                var teams = this.controller.Worlds.Where(w => w.MatchId == this.controller.MatchID);
                return teams.FirstOrDefault(w => w.Color == WorldColor.Green);
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
        [ImportingConstructor]
        public WvWTrackerViewModel(IWvWController wvwController, IHasWvWMap wvwMapVM)
        {
            this.controller = wvwController;
            this.WvWMapVM = wvwMapVM;

            var collectionViewSource = new AutoRefreshCollectionViewSource();
            collectionViewSource.Source = this.controller.CurrentObjectives;
            this.Objectives = collectionViewSource;

            switch (this.UserData.ObjectivesSortProperty)
            {
                case "Type":
                    this.OnSortingPropertyChanged("Type", ListSortDirection.Ascending);
                    break;
                case "DistanceFromPlayer":
                    this.OnSortingPropertyChanged("DistanceFromPlayer", ListSortDirection.Ascending);
                    break;
                case "ShortName":
                    this.OnSortingPropertyChanged("ShortName", ListSortDirection.Ascending);
                    break;
                case "Location":
                    this.OnSortingPropertyChanged("Location", ListSortDirection.Ascending);
                    break;
                case "WorldOwner":
                    this.OnSortingPropertyChanged("WorldOwner", ListSortDirection.Ascending);
                    break;
                default:
                    this.OnSortingPropertyChanged("Type", ListSortDirection.Ascending);
                    break;
            }
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
            this.OnPropertyChanged(() => this.IsUsingPlayerMap);
            this.OnPropertyChanged(() => this.IsUsingRedBorderlands);
            this.OnPropertyChanged(() => this.IsUsingBlueBorderlands);
            this.OnPropertyChanged(() => this.IsUsingGreenBorderlands);
            this.OnPropertyChanged(() => this.IsUsingEternalBattlegrounds);
        }

        /// <summary>
        /// Raises property changed events for all of the orientation selection properties
        /// </summary>
        private void NotifyOrientationSelectionChanged()
        {
            this.OnPropertyChanged(() => this.IsVerticalOrientation);
            this.OnPropertyChanged(() => this.IsHorizontalOrientation);
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

        /// <summary>
        /// Raises property changed events for all of the name selection properties
        /// </summary>
        private void NotifyDisplayedNamesSelectionChanged()
        {
            this.OnPropertyChanged(() => this.IsCardinalDirectionsSelected);
            this.OnPropertyChanged(() => this.IsShortNamesSelected);
        }

        /// <summary>
        /// Handles updating the sorting descriptions of the Objectives collection
        /// and raising INotifyPropertyChanged for all sort properties
        /// </summary>
        private void OnSortingPropertyChanged(string property, ListSortDirection direction)
        {
            this.Objectives.SortDescriptions.Clear();
            this.Objectives.SortDescriptions.Add(new SortDescription(property, direction));
            this.Objectives.View.Refresh();

            this.UserData.ObjectivesSortProperty = property;
            this.OnPropertyChanged(() => this.SortByType);
            this.OnPropertyChanged(() => this.SortByDistance);
            this.OnPropertyChanged(() => this.SortByName);
            this.OnPropertyChanged(() => this.SortByOwner);
        }
    }
}
