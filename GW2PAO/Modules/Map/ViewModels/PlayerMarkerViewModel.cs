using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.API.Util;
using GW2PAO.Modules.Map.Interfaces;
using GW2PAO.Modules.Tasks.Models;
using GW2PAO.Modules.Tasks.ViewModels;
using MapControl;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Map.ViewModels
{
    /// <summary>
    /// View model for a player marker (player task)
    /// </summary>
    public class PlayerMarkerViewModel : BindableBase, IHasMapLocation
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private MercatorTransform transform = new MercatorTransform();
        private PlayerTaskViewModel taskViewModel;
        private IZoneService zoneService;
        private IPlayerService playerService;
        private MapControl.Location location;
        private bool isVisible;
        private Continent continent;
        private MapUserData userData;

        /// <summary>
        /// The player task's ID
        /// </summary>
        public Guid ID
        {
            get { return this.taskViewModel.Task.ID; }
        }

        /// <summary>
        /// The player task's icon
        /// </summary>
        public string Icon
        {
            get { return this.taskViewModel.Task.IconUri; }
        }

        /// <summary>
        /// The player task's name
        /// </summary>
        public string Name
        {
            get { return this.taskViewModel.Name; }
        }

        /// <summary>
        /// Description for the task
        /// </summary>
        public string Description
        {
            get { return this.taskViewModel.Task.Description; }
        }

        /// <summary>
        /// True if the task has been completed, else false
        /// 
        /// Takes into account whether or not the task is
        ///  completed on a per-character basis (See TasksController)
        /// </summary>
        public bool IsTaskCompleted
        {
            get { return this.taskViewModel.IsCompleted; }
            set
            {
                if (this.taskViewModel.IsCompleted != value)
                {
                    this.taskViewModel.IsCompleted = value;
                }
            }
        }

        /// <summary>
        /// True if the task has been completed, else false
        /// 
        /// Takes into account whether or not the task is
        ///  completed on a per-character basis (See TasksController)
        /// </summary>
        public bool IsTaskCompletable
        {
            get { return this.taskViewModel.Task.IsCompletable; }
        }

        /// <summary>
        /// Map location of the marker/player task
        /// </summary>
        public MapControl.Location Location
        {
            get { return this.location; }
            set
            {
                if (SetProperty(ref this.location, value))
                {
                    // Figure out what continent we are in
                    if (this.continent == null)
                        this.continent = this.DetermineCurrentContinent();

                    // Update the task's continent location
                    var mapPoint = transform.Transform(this.location);
                    this.taskViewModel.Task.ContinentLocation = new Point(
                        ((this.continent.Width * mapPoint.X) / 360.0) + (continent.Width / 2),
                        (continent.Height / 2) - ((mapPoint.Y * continent.Height) / 360.0));

                    // Determine the map and set the map location accordingly
                    var map = zoneService.GetMap(this.continent.Id, this.taskViewModel.Task.ContinentLocation);
                    if (map == null)
                    {
                        this.taskViewModel.Task.Location = null;
                    }
                    else
                    {
                        if (this.taskViewModel.Task.MapID != map.Id)
                            this.taskViewModel.Task.MapID = map.Id;

                        this.taskViewModel.Task.Location = MapsHelper.ConvertToMapPos(map.ContinentRectangle, map.MapRectangle, this.taskViewModel.Task.ContinentLocation);

                        // Note: we really only do this to keep compatibility with pre-map player tasks (yea, not great I know...)
                        this.taskViewModel.Task.Location.X = this.taskViewModel.Task.Location.X / CalcUtil.MapConversionFactor;
                        this.taskViewModel.Task.Location.Y = this.taskViewModel.Task.Location.Y / CalcUtil.MapConversionFactor;
                    }
                }
            }
        }

        /// <summary>
        /// The location of the task in the corresponding continent, if any
        /// Null if no location exists
        /// </summary>
        public Point TaskContinentLocation
        {
            get { return this.taskViewModel.Task.ContinentLocation; }
        }

        /// <summary>
        /// The location of the task, if any
        /// Null if no map/zone location exists
        /// </summary>
        public Point TaskMapLocation
        {
            get { return this.taskViewModel.Task.Location; }
        }

        /// <summary>
        /// View model object for the corresponding task
        /// </summary>
        public PlayerTaskViewModel TaskViewModel
        {
            get { return this.taskViewModel; }
        }

        /// <summary>
        /// True if this marker is set as visible, else false
        /// </summary>
        public bool IsVisible
        {
            get { return this.isVisible; }
            set { SetProperty(ref this.isVisible, value); }
        }

        /// <summary>
        /// Command to copy the waypoint code for the marker/task, if any
        /// </summary>
        public ICommand CopyWaypointCommand { get { return this.taskViewModel.CopyWaypointCommand; } }

        /// <summary>
        /// Command to edit the marker/task
        /// </summary>
        public ICommand EditCommand { get { return this.taskViewModel.EditCommand; } }

        /// <summary>
        /// Command to delete the task entirely
        /// </summary>
        public ICommand DeleteCommand { get { return this.taskViewModel.DeleteCommand; } }

        /// <summary>
        /// Constructs a new Player Marker view model
        /// </summary>
        /// <param name="taskViewModel">View model of the marker's corresponding task</param>
        public PlayerMarkerViewModel(PlayerTaskViewModel taskViewModel, MapUserData userData, IZoneService zoneService, IPlayerService playerService)
        {
            this.taskViewModel = taskViewModel;
            this.userData = userData;
            this.zoneService = zoneService;
            this.playerService = playerService;

            this.userData.HiddenMarkerCategories.CollectionChanged += HiddenMarkerCategories_CollectionChanged;
            this.taskViewModel.PropertyChanged += TaskViewModel_PropertyChanged;
            this.taskViewModel.Task.PropertyChanged += Task_PropertyChanged;
            this.RefreshVisibility();
        }

        private void HiddenMarkerCategories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.RefreshVisibility();
        }

        private void TaskViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(null);
        }

        private void Task_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(null);
        }

        private Continent DetermineCurrentContinent()
        {
            Continent cont;

            if (this.taskViewModel.Task.MapID > 0)
                cont = this.zoneService.GetContinentByMap(this.taskViewModel.Task.MapID);
            else if (this.playerService.HasValidMapId)
                cont = this.zoneService.GetContinentByMap(this.playerService.MapId);
            else
                cont = this.zoneService.GetContinent(1); // Assume default Tyria continent   TODO: Would be nice to use the continent shown on the map

            return cont;
        }

        private void RefreshVisibility()
        {
            if (this.userData.HiddenMarkerCategories.Contains(this.TaskViewModel.Category))
                this.IsVisible = false;
            else
                this.IsVisible = true;
        }
    }
}
