using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Modules.Tasks.Interfaces;
using GW2PAO.Modules.Tasks.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Tasks.ViewModels
{
    public class PlayerTaskViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private PlayerTask task;
        private IZoneService zoneService;
        private IPlayerTasksController controller;
        private bool isPlayerOnMap;
        private double distanceFromPlayer;
        private double directionFromPlayer;

        /// <summary>
        /// The actual player task
        /// </summary>
        public PlayerTask Task
        {
            get { return this.task; }
            private set { SetProperty(ref this.task, value); }
        }

        /// <summary>
        /// True if this task is associated with a location, else false
        /// </summary>
        public bool HasLocation
        {
            get { return this.Task.Location != null; }
        }

        /// <summary>
        /// Name of the map that this task is for, if any
        /// </summary>
        public string MapName
        {
            get { return this.zoneService.GetZoneName(this.Task.MapID); }
        }

        /// <summary>
        /// True if the player is currently on the same map as this task, else false
        /// </summary>
        public bool IsPlayerOnMap
        {
            get { return this.isPlayerOnMap; }
            set { SetProperty(ref this.isPlayerOnMap, value); }
        }

        /// <summary>
        /// The distance from the player's position
        /// </summary>
        public double DistanceFromPlayer
        {
            get { return this.distanceFromPlayer; }
            set { SetProperty(ref this.distanceFromPlayer, value); }
        }

        /// <summary>
        /// Direction/Angle from the player's position
        /// </summary>
        public double DirectionFromPlayer
        {
            get { return this.directionFromPlayer; }
            set { SetProperty(ref this.directionFromPlayer, value); }
        }

        /// <summary>
        /// The tasks user data
        /// </summary>
        public TasksUserData UserData
        {
            get { return this.controller.UserData; }
        }

        /// <summary>
        /// Command to delete the task entirely
        /// </summary>
        public ICommand DeleteCommand { get; private set;}

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="task">The task that this view model wraps</param>
        /// <param name="zoneService">Service that provides zone information, such as map name</param>
        public PlayerTaskViewModel(PlayerTask task, IZoneService zoneService, IPlayerTasksController controller)
        {
            this.Task = task;
            this.zoneService = zoneService;
            this.controller = controller;
            this.DeleteCommand = new DelegateCommand(this.Delete);
        }

        /// <summary>
        /// Deletes/removes the task entirely
        /// </summary>
        private void Delete()
        {
            this.controller.DeleteTask(this.Task);
        }
    }
}
