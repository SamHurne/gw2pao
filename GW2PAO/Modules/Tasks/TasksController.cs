using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
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
using GW2PAO.Modules.Tasks.Views;
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
        /// The MEF composition container
        /// </summary>
        private CompositionContainer container;

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
        /// The current player's character name
        /// </summary>
        public string CharacterName { get; private set; }

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
        public TasksController(IZoneService zoneService, IPlayerService playerService, TasksUserData userData, CompositionContainer container)
        {
            logger.Debug("Initializing Player Tasks Controller");
            this.zoneService = zoneService;
            this.playerService = playerService;
            this.container = container;
            this.isStopped = false;

            this.CharacterName = this.playerService.CharacterName;
            this.UserData = userData;
            this.PlayerTasks = new ObservableCollection<PlayerTaskViewModel>();

            // Initialize all loaded tasks
            logger.Info("Initializing all loaded player tasks");
            foreach (var task in this.UserData.Tasks)
            {
                var taskVm = new PlayerTaskViewModel(task, zoneService, this, this.container);
                taskVm.OnNewCharacterDetected(this.CharacterName);
                this.PlayerTasks.Add(taskVm);
            }
            this.EnsureTasksHaveContinentLocation();

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
        public void AddOrUpdateTask(PlayerTask task)
        {
            // Lock so the refresh thread doesn't use the collection while we are modifying it
            lock (this.refreshLock)
            {
                Threading.InvokeOnUI(() =>
                    {
                        var existingTask = this.PlayerTasks.FirstOrDefault(t => t.Task.ID == task.ID);
                        if (existingTask == null)
                        {
                            this.UserData.Tasks.Add(task);
                            this.PlayerTasks.Add(new PlayerTaskViewModel(task, zoneService, this, this.container));
                        }
                        else
                        {
                            existingTask.Task.Name = task.Name;
                            existingTask.Task.Description = task.Description;
                            existingTask.Task.IsCompletable = task.IsCompletable;
                            existingTask.Task.IsAccountCompleted = task.IsAccountCompleted;
                            existingTask.Task.IsCompletedPerCharacter = task.IsCompletedPerCharacter;
                            existingTask.Task.IsDailyReset = task.IsDailyReset;
                            existingTask.Task.AutoComplete = task.AutoComplete;
                            existingTask.Task.Location = task.Location;
                            existingTask.Task.ContinentId = task.ContinentId;
                            existingTask.Task.MapID = task.MapID;
                            existingTask.Task.IconUri = task.IconUri;
                            existingTask.Task.WaypointCode = task.WaypointCode;
                            existingTask.Task.Category = task.Category;
                            foreach (var character in task.CharacterCompletions.Keys)
                            {
                                if (!existingTask.Task.CharacterCompletions.ContainsKey(character))
                                    existingTask.Task.CharacterCompletions.Add(character, task.CharacterCompletions[character]);
                                else
                                    existingTask.Task.CharacterCompletions[character] = task.CharacterCompletions[character];
                            }
                        }
                    });
            }
        }

        /// <summary>
        /// Adds a new task to the collection of player tasks
        /// </summary>
        /// <param name="task">The task and viewmodel to add</param>
        public void AddOrUpdateTask(PlayerTaskViewModel taskViewModel)
        {
            // Lock so the refresh thread doesn't use the collection while we are modifying it
            lock (this.refreshLock)
            {
                Threading.InvokeOnUI(() =>
                {
                    var existingTask = this.PlayerTasks.FirstOrDefault(t => t.Task.ID == taskViewModel.Task.ID);
                    if (existingTask == null)
                    {
                        this.UserData.Tasks.Add(taskViewModel.Task);
                        this.PlayerTasks.Add(taskViewModel);
                    }
                    else
                    {
                        existingTask.Task.Name = taskViewModel.Task.Name;
                        existingTask.Task.Description = taskViewModel.Task.Description;
                        existingTask.Task.IsCompletable = taskViewModel.Task.IsCompletable;
                        existingTask.Task.IsAccountCompleted = taskViewModel.Task.IsAccountCompleted;
                        existingTask.Task.IsCompletedPerCharacter = taskViewModel.Task.IsCompletedPerCharacter;
                        existingTask.Task.IsDailyReset = taskViewModel.Task.IsDailyReset;
                        existingTask.Task.AutoComplete = taskViewModel.Task.AutoComplete;
                        existingTask.Task.Location = taskViewModel.Task.Location;
                        existingTask.Task.ContinentId = taskViewModel.Task.ContinentId;
                        existingTask.Task.MapID = taskViewModel.Task.MapID;
                        existingTask.Task.IconUri = taskViewModel.Task.IconUri;
                        existingTask.Task.WaypointCode = taskViewModel.Task.WaypointCode;
                        existingTask.Task.Category = taskViewModel.Task.Category;
                        foreach (var character in taskViewModel.Task.CharacterCompletions.Keys)
                        {
                            if (!existingTask.Task.CharacterCompletions.ContainsKey(character))
                                existingTask.Task.CharacterCompletions.Add(character, taskViewModel.Task.CharacterCompletions[character]);
                            else
                                existingTask.Task.CharacterCompletions[character] = taskViewModel.Task.CharacterCompletions[character];
                        }
                    }
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
                    var taskToRemove = this.PlayerTasks.FirstOrDefault(t => t.Task.ID == task.ID);
                    if (taskToRemove != null)
                    {
                        this.PlayerTasks.Remove(taskToRemove);
                        this.UserData.Tasks.Remove(task);
                    }
                });
            }
        }

        /// <summary>
        /// Loads all tasks from the given path
        /// </summary>
        /// <param name="path">The path to import from</param>
        public void LoadTasksFile(string path)
        {
            logger.Info("Loading tasks file from {0}", path);

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
                    this.PlayerTasks.Clear();
                    this.UserData.Tasks.Clear();
                    foreach (var task in (ObservableCollection<PlayerTask>)loadedTasks)
                    {
                        task.IsAccountCompleted = false;
                        this.UserData.Tasks.Add(task);
                        this.PlayerTasks.Add(new PlayerTaskViewModel(task, this.zoneService, this, this.container));
                    }
                    this.EnsureTasksHaveContinentLocation();
                });

                logger.Info("Successfully loaded tasks from {0}", path);
            }
            catch (Exception ex)
            {
                logger.Error("Unable to load tasks!");
                logger.Error(ex);
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
                            task.IsAccountCompleted = false;
                            this.UserData.Tasks.Add(task);
                            this.PlayerTasks.Add(new PlayerTaskViewModel(task, this.zoneService, this, this.container));
                        }
                        this.EnsureTasksHaveContinentLocation();
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

                // Check for a new character
                if (this.CharacterName != this.playerService.CharacterName)
                {
                    this.CharacterName = this.playerService.CharacterName;
                    Threading.BeginInvokeOnUI(() =>
                    {
                        foreach (var pt in this.PlayerTasks)
                        {
                            pt.OnNewCharacterDetected(this.CharacterName);
                        }
                    });
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
                {
                    Threading.BeginInvokeOnUI(() => pt.IsCompleted = false);
                    pt.Task.IsAccountCompleted = false;
                    foreach (var charCompletion in pt.Task.CharacterCompletions.Keys)
                        pt.Task.CharacterCompletions[charCompletion] = false;
                }
            }
        }

        /// <summary>
        /// Loops through the collection of tasks and refreshs their distance/angles
        /// </summary>
        private void RefreshTaskDistancesAngles()
        {
            const int ABOVE_BELOW_THRESHOLD = 150;

            this.CurrentMapID = this.playerService.MapId;

            var playerPos = this.playerService.PlayerPosition;
            var cameraDir = this.playerService.CameraDirection;
            if (playerPos != null && cameraDir != null)
            {
                var playerMapPosition = CalcUtil.ConvertToMapPosition(playerPos);
                var cameraDirectionMapPosition = CalcUtil.ConvertToMapPosition(cameraDir);

                foreach (var ptask in this.PlayerTasks.Where(pt => pt.Task.Location != null && pt.Task.MapID == this.CurrentMapID))
                {
                    var taskMapPosition = CalcUtil.ConvertToMapPosition(ptask.Task.Location);

                    // Update distances and angles
                    var newDistance = Math.Round(CalcUtil.CalculateDistance(playerMapPosition, taskMapPosition, this.UserData.DistanceUnits));
                    var newAngle = CalcUtil.CalculateAngle(CalcUtil.Vector.CreateVector(playerMapPosition, taskMapPosition),
                                                           CalcUtil.Vector.CreateVector(new API.Data.Entities.Point(0, 0), cameraDirectionMapPosition));

                    bool isAbove = (ptask.Task.Location.Z > 0) && (taskMapPosition.Z - playerMapPosition.Z > ABOVE_BELOW_THRESHOLD);
                    bool isBelow = (ptask.Task.Location.Z > 0) && (playerMapPosition.Z - taskMapPosition.Z > ABOVE_BELOW_THRESHOLD);

                    Threading.BeginInvokeOnUI(() =>
                        {
                            ptask.IsPlayerOnMap = true;
                            ptask.DistanceFromPlayer = newDistance;
                            ptask.DirectionFromPlayer = newAngle;
                            ptask.IsAbovePlayer = isAbove;
                            ptask.IsBelowPlayer = isBelow;
                        });

                    // Check for auto-completion detection
                    if (ptask.Task.AutoComplete && CalcUtil.CalculateDistance(playerMapPosition, taskMapPosition, API.Data.Enums.Units.Feet) < 10)
                    {
                        Threading.BeginInvokeOnUI(() =>
                        {
                            ptask.IsCompleted = true;
                        });
                    }
                }

                // Player is not on the map
                foreach (var ptask in this.PlayerTasks.Where(pt => pt.Task.MapID != this.CurrentMapID))
                {
                    Threading.BeginInvokeOnUI(() =>
                       {
                           ptask.IsPlayerOnMap = false;
                       });
                }
            }
        }

        /// <summary>
        /// Loops through the collection of tasks and ensure that they all
        /// have a corresponding continent location
        /// </summary>
        private void EnsureTasksHaveContinentLocation()
        {
            foreach (var ptask in this.PlayerTasks.Where(pt => pt.Task.Location != null && pt.Task.ContinentLocation == null))
            {
                var continent = this.zoneService.GetContinentByMap(ptask.Task.MapID);
                var map = this.zoneService.GetMap(ptask.Task.MapID);
                ptask.Task.ContinentId = map.ContinentId;
                ptask.Task.ContinentLocation = API.Util.MapsHelper.ConvertToWorldPos(map.ContinentRectangle, map.MapRectangle, CalcUtil.ConvertToMapPosition(ptask.Task.Location));
            }
        }
    }
}
