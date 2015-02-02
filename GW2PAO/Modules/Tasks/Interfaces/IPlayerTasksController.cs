using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Modules.Tasks.Models;
using GW2PAO.Modules.Tasks.ViewModels;

namespace GW2PAO.Modules.Tasks.Interfaces
{
    public interface IPlayerTasksController
    {
        /// <summary>
        /// Collection of player tasks
        /// </summary>
        ObservableCollection<PlayerTaskViewModel> PlayerTasks { get; }

        /// <summary>
        /// The player task user data
        /// </summary>
        TasksUserData UserData { get; }

        /// <summary>
        /// Starts the controller
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the controller
        /// </summary>
        void Stop();

        /// <summary>
        /// Forces a shutdown of the controller, including all running timers/threads
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Adds/updates a task in the collection of player tasks
        /// </summary>
        /// <param name="task">The task to add</param>
        void AddOrUpdateTask(PlayerTask task);

        /// <summary>
        /// Deletes a task from the collection of player tasks
        /// </summary>
        /// <param name="task">The task to delete</param>
        void DeleteTask(PlayerTask task);

        /// <summary>
        /// Loads all tasks from the given path
        /// </summary>
        /// <param name="path">The path to import from</param>
        void LoadTasksFile(string path);

        /// <summary>
        /// Exports all tasks to the given path
        /// </summary>
        /// <param name="path">The path to export to</param>
        void ExportTasks(string path);

        /// <summary>
        /// Imports all tasks from the given path
        /// </summary>
        /// <param name="path">The path to import from</param>
        void ImportTasks(string path);
    }
}
