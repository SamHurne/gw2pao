using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.API.Util;
using GW2PAO.Modules.Tasks.Interfaces;
using GW2PAO.Modules.Tasks.Models;
using GW2PAO.Modules.Tasks.ViewModels;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Tasks
{
    /// <summary>
    /// The Player Tasks controller
    /// </summary>
    [Export(typeof(IPlayerTasksController))]
    public class TasksController : BindableBase, IPlayerTasksController
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Service responsible for Zone information
        /// </summary>
        private IZoneService zoneService;

        /// <summary>
        /// Service responsible for Player information
        /// </summary>
        private IPlayerService playerService;

        /// <summary>
        /// Timer for the periodic refresh thread
        /// </summary>
        private Timer refreshTimer;

        /// <summary>
        /// Keeps track of how many times Start() has been called in order
        /// to support reuse of a single object
        /// </summary>
        private int startCallCount;

        /// <summary>
        /// True if the controller's timers are no longer running, else false
        /// </summary>
        private bool isStopped;

        /// <summary>
        /// Locking object for operations performed with the refresh timer
        /// </summary>
        private readonly object refreshLock = new object();

        /// <summary>
        /// The collection of player tasks
        /// </summary>
        public ObservableCollection<PlayerTaskViewModel> PlayerTasks { get; private set; }

        /// <summary>
        /// The player task user data
        /// </summary>
        public TasksUserData UserData { get; private set; }

        /// <summary>
        /// The interval by which to refresh
        /// </summary>
        public int RefreshInterval { get; set; }

        /// <summary>
        /// The ID of the current map/zone
        /// </summary>
        public int CurrentMapID { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="zoneService">The zone service</param>
        /// <param name="playerService">The player service</param>
        /// <param name="userData">The loaded user data</param>
        [ImportingConstructor]
        public TasksController(IZoneService zoneService, IPlayerService playerService, TasksUserData userData)
        {
            logger.Debug("Initializing Player Tasks Controller");
            this.zoneService = zoneService;
            this.playerService = playerService;
            this.isStopped = false;

            this.UserData = userData;
            this.PlayerTasks = new ObservableCollection<PlayerTaskViewModel>();

            // Initialize all loaded tasks
            logger.Info("Initializing all loaded player tasks");
            foreach (var task in this.UserData.Tasks)
                this.PlayerTasks.Add(new PlayerTaskViewModel(task, zoneService, this));

            // Initialize refresh timers
            this.refreshTimer = new Timer(this.Refresh);
            this.RefreshInterval = 125;
            this.CurrentMapID = -1;

            logger.Info("Player Tasks Controller initialized");
        }

        /// <summary>
        /// Starts the automatic refresh of the TasksController
        /// </summary>
        public void Start()
        {
            logger.Debug("Start called");
            Task.Factory.StartNew(() =>
            {
                // Start the timer if this is the first time that Start() has been called
                if (this.startCallCount == 0)
                {
                    this.isStopped = false;
                    logger.Debug("Starting refresh timer");
                    this.Refresh();
                }

                this.startCallCount++;
                logger.Debug("startCallCount = " + this.startCallCount);

            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Stops the automatic refresh of the TasksController
        /// </summary>
        public void Stop()
        {
            this.startCallCount--;
            logger.Debug("Stop called - startCallCount = " + this.startCallCount);

            // Stop the refresh timer if all calls to Start() have had a matching call to Stop()
            if (this.startCallCount == 0)
            {
                logger.Debug("Stopping refresh timers");
                this.isStopped = true;
                lock (this.refreshLock)
                {
                    this.refreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Forces a shutdown of the controller, including all running timers/threads
        /// </summary>
        public void Shutdown()
        {
            logger.Debug("Shutdown called");
            logger.Debug("Stopping refresh timers");
            this.isStopped = true;
            lock (this.refreshLock)
            {
                this.refreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Adds a new task to the collection of player tasks
        /// </summary>
        /// <param name="task">The task to add</param>
        public void AddTask(PlayerTask task)
        {
            // Lock so the refresh thread doesn't use the collection while we are modifying it
            lock (this.refreshLock)
            {
                Threading.InvokeOnUI(() =>
                    {
                        this.UserData.Tasks.Add(task);
                        this.PlayerTasks.Add(new PlayerTaskViewModel(task, zoneService, this));
                    });
            }
        }

        /// <summary>
        /// Deletes a task from the collection of player tasks
        /// </summary>
        /// <param name="task">The task to delete</param>
        public void DeleteTask(PlayerTask task)
        {
            // Lock so the refresh thread doesn't use the collection while we are modifying it
            lock (this.refreshLock)
            {
                Threading.InvokeOnUI(() =>
                {
                    var taskToRemove = this.PlayerTasks.FirstOrDefault(t => t.Task == task);
                    if (taskToRemove != null)
                    {
                        this.PlayerTasks.Remove(taskToRemove);
                        this.UserData.Tasks.Remove(task);
                    }
                });
            }
        }

        /// <summary>
        /// Exports all tasks to the given path
        /// </summary>
        /// <param name="path">The path to export to</param>
        public void ExportTasks(string path)
        {
            logger.Info("Exporting tasks to {0}", path);
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<PlayerTask>));

            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, this.UserData.Tasks);
            }
            logger.Info("Successfully exported tasks to {0}", path);
        }

        /// <summary>
        /// Imports all tasks from the given path
        /// </summary>
        /// <param name="path">The path to import from</param>
        public void ImportTasks(string path)
        {
            logger.Info("Importing tasks from {0}", path);

            XmlSerializer deserializer = new XmlSerializer(typeof(ObservableCollection<PlayerTask>));
            object loadedTasks = null;

            try
            {
                using (TextReader reader = new StreamReader(path))
                {
                    loadedTasks = deserializer.Deserialize(reader);
                }

                Threading.InvokeOnUI(() =>
                    {
                        foreach (var task in (ObservableCollection<PlayerTask>)loadedTasks)
                        {
                            this.UserData.Tasks.Add(task);
                            this.PlayerTasks.Add(new PlayerTaskViewModel(task, this.zoneService, this));
                        }
                    });

                logger.Info("Successfully imported tasks from {0}", path);
            }
            catch (Exception ex)
            {
                logger.Error("Unable to import tasks!");
                logger.Error(ex);
            }
        }

        /// <summary>
        /// The main refresh method
        /// </summary>
        private void Refresh(object state = null)
        {
            lock (this.refreshLock)
            {
                if (this.isStopped)
                    return; // Immediately return if we are supposed to be stopped

                // Check for the daily reset
                if (DateTime.UtcNow.Date.CompareTo(this.UserData.LastResetDateTime.Date) != 0)
                {
                    this.OnDailyReset();
                    this.UserData.LastResetDateTime = DateTime.UtcNow.Date;
                }

                // Refresh task distances/angles
                if (this.playerService.HasValidMapId)
                {
                    this.RefreshTaskDistancesAngles();
                }

                this.refreshTimer.Change(this.RefreshInterval, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Performs actions on the daily reset
        /// </summary>
        private void OnDailyReset()
        {
            logger.Info("Daily reset detected");
            foreach (var pt in this.PlayerTasks)
            {
                if (pt.Task.IsCompletable && pt.Task.IsDailyReset)
                    pt.Task.IsCompleted = false;
            }
        }

        /// <summary>
        /// Loops through the collection of tasks and refreshs their distance/angles
        /// </summary>
        private void RefreshTaskDistancesAngles()
        {
            this.CurrentMapID = this.playerService.MapId;

            var playerPos = this.playerService.PlayerPosition;
            var cameraDir = this.playerService.CameraDirection;
            if (playerPos != null && cameraDir != null)
            {
                var playerMapPosition = CalcUtil.ConvertToMapPosition(playerPos);
                var cameraDirectionMapPosition = CalcUtil.ConvertToMapPosition(cameraDir);

                foreach (var ptask in this.PlayerTasks.Where(pt => pt.Task.Location != null
                                                                   && pt.Task.MapID == this.CurrentMapID))
                {
                    var taskMapPosition = CalcUtil.ConvertToMapPosition(ptask.Task.Location);

                    var newDistance = Math.Round(CalcUtil.CalculateDistance(playerMapPosition, taskMapPosition, this.UserData.DistanceUnits));
                    var newAngle = CalcUtil.CalculateAngle(CalcUtil.Vector.CreateVector(playerMapPosition, taskMapPosition),
                                                           CalcUtil.Vector.CreateVector(new API.Data.Entities.Point(0, 0), cameraDirectionMapPosition));

                    Threading.BeginInvokeOnUI(() =>
                        {
                            ptask.IsPlayerOnMap = true;
                            ptask.DistanceFromPlayer = newDistance;
                            ptask.DirectionFromPlayer = newAngle;
                        });
                }

                foreach (var ptask in this.PlayerTasks.Where(pt => pt.Task.MapID != this.CurrentMapID))
                {
                    Threading.BeginInvokeOnUI(() =>
                       {
                           ptask.IsPlayerOnMap = false;
                       });
                }
            }
        }

    }
}
