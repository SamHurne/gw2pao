using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FeserWard.Controls;
using GW2PAO.API.Data;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Modules.Commerce.Services;
using GW2PAO.Modules.Tasks.Interfaces;
using GW2PAO.Modules.Tasks.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Tasks.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(NewTaskDialogViewModel))]
    public class NewTaskDialogViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private ItemDBEntry selectedItem;
        private PlayerTask task;
        private ICommerceService commerceService;
        private IPlayerService playerService;
        private IZoneService zoneService;
        private IPlayerTasksController controller;
        private bool hasLocation;

        /// <summary>
        /// The actual player task
        /// </summary>
        public PlayerTask Task
        {
            get { return this.task; }
            set { SetProperty(ref this.task, value); }
        }

        /// <summary>
        /// True if this task will be associated with a location, else false
        /// </summary>
        public bool HasLocation
        {
            get { return this.hasLocation; }
            set { SetProperty(ref this.hasLocation, value);}
        }

        /// <summary>
        /// Name of the map that this task is for, if any
        /// </summary>
        public string MapName
        {
            get
            {
                string name;
                if (this.Task != null)
                    name = this.zoneService.GetZoneName(this.Task.MapID);
                else
                    name = string.Empty;
                if (name.ToLower().Equals("unknown"))
                    name = Properties.Resources.Unknown;
                return name;
            }
        }

        /// <summary>
        /// Provider object for finding an item for the user to select (for task icon)
        /// </summary>
        public IIntelliboxResultsProvider ItemsProvider
        {
            get;
            private set;
        }

        /// <summary>
        /// The item selected by the user (for task icon)
        /// </summary>
        public ItemDBEntry SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                if (this.SetProperty(ref this.selectedItem, value))
                {
                    if (this.selectedItem == null)
                        return;
                    
                    var itemData = this.commerceService.GetItem(this.selectedItem.ID);
                    if (itemData == null)
                        return;

                    this.Task.IconUri = itemData.Icon.ToString();
                }
            }
        }

        /// <summary>
        /// Collection of existing categories the user can optionally choose from
        /// </summary>
        public List<string> ExistingCategories
        {
            get;
            private set;
        }

        /// <summary>
        /// Command to update the location used for the task
        /// </summary>
        public ICommand RefreshLocationCommand { get; private set; }

        /// <summary>
        /// Command to apply the add/edit of the task
        /// </summary>
        public ICommand ApplyCommand { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        [ImportingConstructor]
        public NewTaskDialogViewModel(ICommerceService commerceService, IPlayerService playerService, IZoneService zoneService, IPlayerTasksController controller)
        {
            this.commerceService = commerceService;
            this.playerService = playerService;
            this.zoneService = zoneService;
            this.controller = controller;

            this.ItemsProvider = new ItemResultsProvider(this.commerceService);

            this.ExistingCategories = new List<string>();
            foreach (var task in controller.PlayerTasks)
            {
                if (!string.IsNullOrEmpty(task.Category) && !this.ExistingCategories.Contains(task.Category))
                    this.ExistingCategories.Add(task.Category);
            }

            this.Task = new PlayerTask();
            this.RefreshLocationCommand = new DelegateCommand(this.RefreshLocation);
            this.ApplyCommand = new DelegateCommand(this.AddOrUpdateTask);
            this.RefreshLocation();
        }

        /// <summary>
        /// Refreshes the location for the task
        /// </summary>
        private void RefreshLocation()
        {
            if (this.playerService.HasValidMapId)
            {
                var map = this.zoneService.GetMap(this.playerService.MapId);
                this.task.ContinentId = map.ContinentId;
                this.Task.MapID = map.Id;
                this.Task.Location = this.playerService.PlayerPosition;
                this.Task.ContinentLocation = API.Util.MapsHelper.ConvertToWorldPos(map.ContinentRectangle, map.MapRectangle, API.Util.CalcUtil.ConvertToMapPosition(this.Task.Location));

                this.OnPropertyChanged(() => this.MapName);
            }
        }

        /// <summary>
        /// Adds/updates the task
        /// </summary>
        private void AddOrUpdateTask()
        {
            if (!this.HasLocation)
            {
                this.task.ContinentId = -1;
                this.Task.MapID = -1;
                this.Task.Location = null;
                this.task.ContinentLocation = null;
            }

            this.controller.AddOrUpdateTask(this.Task);
            this.Task = new PlayerTask(this.Task);
            this.Task.ID = Guid.NewGuid();
            this.Task.Name = string.Empty;
            this.Task.Description = string.Empty;
            this.Task.CharacterCompletions.Clear();
        }
    }
}
