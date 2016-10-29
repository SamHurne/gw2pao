using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Modules.Tasks;
using GW2PAO.Modules.Tasks.Interfaces;
using GW2PAO.Modules.Tasks.ViewModels;
using GW2PAO.PresentationCore;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Map.ViewModels
{
    [Export(typeof(PlayerMarkersViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class PlayerMarkersViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private TaskTrackerViewModel taskTrackerVm;
        private PlayerTasksFactory playerTaskFactory;
        private IPlayerTasksController tasksController;
        private ObservableCollection<PlayerTaskViewModel> playerTasksCollection;
        private IZoneService zoneService;
        private IPlayerService playerService;
        private MapUserData userData;

        private static readonly List<string> TemplateIcons = new List<string>()
        {
            @"/Images/Map/nodeMining.png",
            @"/Images/Map/nodeLogging.png",
            @"/Images/Map/nodeHarvesting.png",
            @"/Images/Map/pointA.png",
            @"/Images/Map/pointB.png",
            @"/Images/Map/pointC.png",
            @"/Images/Map/siegeBlue.png",
            @"/Images/Map/siegeRed.png",
            @"/Images/Map/bookBlue.png",
            @"/Images/Map/bookBrown.png",
            @"/Images/Map/asuraGateBlue.png",
            @"/Images/Map/asuraGatePurple.png",
            @"/Images/Map/flagGreen.png",
            @"/Images/Map/flagRed.png",
            @"/Images/Map/redCog.png",
            @"/Images/Map/redShield.png",
            @"/Images/Map/bossGreen.png",
            @"/Images/Map/bossBlue.png",
            @"/Images/Map/bossPurple.png",
            @"/Images/Map/bossRed.png",
            @"/Images/Map/fancySquare.png",
            @"/Images/Map/fancyCircle.png",
            @"/Images/Map/fancyTriangle.png",
            @"/Images/Map/fancyX.png",
            @"/Images/Map/fancySpiral.png",
            @"/Images/Map/fancyArrow.png",
            @"/Images/Map/fancyHeart.png",
            @"/Images/Map/fancyStar.png",
            @"/Images/Map/shieldBlue.png",
            @"/Images/Map/shieldGrey.png",
            @"/Images/Map/shieldDarkBlue.png",
            @"/Images/Map/shieldGreen.png",
            @"/Images/Map/shieldRed.png",
            @"/Images/Map/shieldOrange.png",
            @"/Images/Map/starBlue.png",
            @"/Images/Map/starGreen.png",
            @"/Images/Map/starYellow.png",
            @"/Images/Map/starYellow2.png",
            @"/Images/Map/whiteOrb.png",
            @"/Images/Map/downedAlly.png",
            @"/Images/Map/downedEnemy.png",
            @"/Images/Map/enslaved.png",
            @"/Images/Map/activity.png",
            @"/Images/Map/adventure.png",
            @"/Images/Map/dialog.png",
            @"/Images/Map/quaggan.png",
            @"/Images/Map/ship.png",
            @"/Images/Map/scout.png",
            @"/Images/Map/anvil.png",
            @"/Images/Map/mentor.png",
            @"/Images/Map/multitag.png",
            @"/Images/Map/swirlDiamond.png",
            @"/Images/Map/swords.png",
            @"/Images/Map/updraft.png",
        };

        /// <summary>
        /// The collection of player markers to show on the map
        /// </summary>
        public ObservableCollection<PlayerMarkerViewModel> PlayerMarkers
        {
            get;
            private set;
        }

        /// <summary>
        /// The collection of template player markers to show on the map
        /// </summary>
        public ObservableCollection<PlayerMarkerViewModel> MarkerTemplates
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of marker categories
        /// </summary>
        public AutoRefreshCollectionViewSource MarkerCategories
        {
            get { return this.taskTrackerVm.TaskCategories; }
        }

        /// <summary>
        /// Command to delete all player markers
        /// </summary>
        public ICommand DeleteAllCommand { get { return this.taskTrackerVm.DeleteAllCommand; } }

        /// <summary>
        /// Command to load markers/tasks from a file
        /// </summary>
        public ICommand LoadCommand { get { return this.taskTrackerVm.LoadTasksCommand; } }

        /// <summary>
        /// Command to import all markers/tasks from a file
        /// </summary>
        public ICommand ImportCommand { get { return this.taskTrackerVm.ImportTasksCommand; } }

        /// <summary>
        /// Command to export all markers/tasks to a file
        /// </summary>
        public ICommand ExportCommand { get { return this.taskTrackerVm.ExportTasksCommand; } }

        /// <summary>
        /// Command to change the visibility of a category of markers
        /// </summary>
        public ICommand ToggleCategoryVisibiltyCommand { get; private set; }

        /// <summary>
        /// Constructs a new MarkersViewModel object
        /// </summary>
        [ImportingConstructor]
        public PlayerMarkersViewModel(TaskTrackerViewModel taskTrackerVm,
            MapUserData userData,
            PlayerTasksFactory playerTaskFactory,
            IPlayerTasksController tasksController,
            IZoneService zoneService,
            IPlayerService playerService)
        {
            this.taskTrackerVm = taskTrackerVm;
            this.playerTaskFactory = playerTaskFactory;
            this.tasksController = tasksController;
            this.zoneService = zoneService;
            this.playerService = playerService;
            this.userData = userData;

            this.PlayerMarkers = new ObservableCollection<PlayerMarkerViewModel>();

            this.playerTasksCollection = (ObservableCollection<PlayerTaskViewModel>)this.tasksController.PlayerTasks;
            foreach (var task in this.playerTasksCollection)
            {
                task.PropertyChanged += Task_PropertyChanged;
                if (task.HasContinentLocation)
                    this.PlayerMarkers.Add(new PlayerMarkerViewModel(task, this.userData, this.zoneService, this.playerService));
            }
            this.playerTasksCollection.CollectionChanged += PlayerTasksCollection_CollectionChanged;

            this.InitializeTemplates();
            this.PlayerMarkers.CollectionChanged += PlayerMarkers_CollectionChanged;

            this.ToggleCategoryVisibiltyCommand = new DelegateCommand<string>(this.ToggleCategoryVisibility);
        }

        private void InitializeTemplates()
        {
            this.MarkerTemplates = new ObservableCollection<PlayerMarkerViewModel>();
            foreach (var icon in TemplateIcons)
            {
                var task = this.playerTaskFactory.GetPlayerTask();
                task.IconUri = icon;
                var vm = this.playerTaskFactory.GetPlayerTaskViewModel(task);
                this.MarkerTemplates.Add(new PlayerMarkerViewModel(vm, this.userData, this.zoneService, this.playerService));
            }
        }

        private void PlayerMarkers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // When a player marker is added, check to see if we need to reset the template for that marker
            // (for example, when the user added the marker by drag/dropping it onto the map)
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (PlayerMarkerViewModel newItem in e.NewItems)
                {
                    var template = this.MarkerTemplates.FirstOrDefault(m => m == newItem);
                    if (template != null)
                    {
                        // This marker came from a template

                        // Replace the template
                        var idx = this.MarkerTemplates.IndexOf(template);
                        this.MarkerTemplates.Remove(template);
                        var task = this.playerTaskFactory.GetPlayerTask();
                        task.IconUri = newItem.Icon;
                        var vm = this.playerTaskFactory.GetPlayerTaskViewModel(task);
                        var newTemplate = new PlayerMarkerViewModel(vm, this.userData, this.zoneService, this.playerService);
                        this.MarkerTemplates.Insert(idx, newTemplate);

                        // Then add the corresponding task
                        this.tasksController.AddOrUpdateTask(newItem.TaskViewModel);
                    }
                }
            }
        }

        private void PlayerTasksCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (PlayerTaskViewModel taskVm in e.NewItems)
                    {
                        taskVm.PropertyChanged += Task_PropertyChanged;
                        if (taskVm.HasContinentLocation)
                        {
                            var playerMarker = this.PlayerMarkers.FirstOrDefault(m => m.ID.Equals(taskVm.Task.ID));
                            if (playerMarker == null)
                                this.PlayerMarkers.Add(new PlayerMarkerViewModel(taskVm, this.userData, this.zoneService, this.playerService));
                        }
                    }                    
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (PlayerTaskViewModel taskVm in e.OldItems)
                    {
                        taskVm.PropertyChanged -= Task_PropertyChanged;
                        var playerMarker = this.PlayerMarkers.FirstOrDefault(m => m.ID.Equals(taskVm.Task.ID));
                        if (playerMarker != null)
                            this.PlayerMarkers.Remove(playerMarker);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (PlayerTaskViewModel taskVm in e.NewItems)
                    {
                        taskVm.PropertyChanged += Task_PropertyChanged;
                        if (taskVm.HasContinentLocation)
                        {
                            var playerMarker = this.PlayerMarkers.FirstOrDefault(m => m.ID.Equals(taskVm.Task.ID));
                            if (playerMarker == null)
                                this.PlayerMarkers.Add(new PlayerMarkerViewModel(taskVm, this.userData, this.zoneService, this.playerService));
                        }
                    }
                    foreach (PlayerTaskViewModel taskVm in e.OldItems)
                    {
                        taskVm.PropertyChanged -= Task_PropertyChanged;
                        var playerMarker = this.PlayerMarkers.FirstOrDefault(m => m.ID.Equals(taskVm.Task.ID));
                        if (playerMarker != null)
                            this.PlayerMarkers.Remove(playerMarker);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (PlayerMarkerViewModel markerVm in this.PlayerMarkers)
                    {
                        markerVm.TaskViewModel.PropertyChanged -= Task_PropertyChanged;
                    }
                    this.PlayerMarkers.Clear();
                    break;
                default:
                    break;
            }
        }

        private void Task_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HasContinentLocation")
            {
                var taskVm = (PlayerTaskViewModel)sender;
                if (taskVm.HasContinentLocation)
                {
                    var playerMarker = this.PlayerMarkers.FirstOrDefault(m => m.ID.Equals(taskVm.Task.ID));
                    if (playerMarker == null)
                    {
                        this.PlayerMarkers.Add(new PlayerMarkerViewModel(taskVm, this.userData, this.zoneService, this.playerService));
                    }
                }
                else
                {
                    var playerMarker = this.PlayerMarkers.FirstOrDefault(m => m.ID.Equals(taskVm.Task.ID));
                    if (playerMarker != null)
                    {
                        this.PlayerMarkers.Remove(playerMarker);
                    }
                }
            }
        }

        private void ToggleCategoryVisibility(string categoryName)
        {
            if (this.userData.HiddenMarkerCategories.Contains(categoryName))
            {
                this.userData.HiddenMarkerCategories.Remove(categoryName);
            }
            else
            {
                this.userData.HiddenMarkerCategories.Add(categoryName);
            }
        }
    }
}
