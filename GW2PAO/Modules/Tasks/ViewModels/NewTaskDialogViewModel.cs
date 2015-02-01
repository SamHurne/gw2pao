using System;
using System.Collections.Generic;
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
    [Export]
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
            get { return this.zoneService.GetZoneName(this.Task.MapID); }
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
                    this.Task.IconUri = this.commerceService.GetItem(this.selectedItem.ID).Icon.ToString();
                }
            }
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

            this.Task = new PlayerTask();
            this.Task.MapID = this.playerService.MapId;
            this.Task.Location = this.playerService.PlayerPosition;
            this.RefreshLocationCommand = new DelegateCommand(this.RefreshLocation);
            this.ApplyCommand = new DelegateCommand(this.AddOrUpdateTask);
        }

        /// <summary>
        /// Refreshes the location for the task
        /// </summary>
        private void RefreshLocation()
        {
            this.Task.MapID = this.playerService.MapId;
            this.Task.Location = this.playerService.PlayerPosition;

            this.OnPropertyChanged(() => this.MapName);
        }

        /// <summary>
        /// Adds/updates the task
        /// </summary>
        private void AddOrUpdateTask()
        {
            if (!this.HasLocation)
            {
                this.Task.MapID = -1;
                this.Task.Location = null;
            }

            this.controller.AddTask(this.Task);
        }
    }
}
