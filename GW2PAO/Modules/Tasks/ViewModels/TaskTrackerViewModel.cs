using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.Modules.Tasks.Interfaces;
using GW2PAO.Modules.Tasks.Models;
using GW2PAO.Modules.Tasks.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Win32;
using NLog;

namespace GW2PAO.Modules.Tasks.ViewModels
{
    /// <summary>
    /// Task Tracker view model class
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class TaskTrackerViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The player tasks controller
        /// </summary>
        private IPlayerTasksController controller;

        /// <summary>
        /// Container object for composing parts
        /// </summary>
        private CompositionContainer container;

        /// <summary>
        /// Collection of player tasks
        /// </summary>
        public ObservableCollection<PlayerTaskViewModel> PlayerTasks { get { return this.controller.PlayerTasks; } }

        /// <summary>
        /// The player tasks user data
        /// </summary>
        public TasksUserData UserData
        {
            get { return this.controller.UserData; }
        }

        /// <summary>
        /// Command to add a new task to the task list
        /// </summary>
        public ICommand AddNewTaskCommand { get; private set; }

        /// <summary>
        /// Command to delete a task from the task list
        /// </summary>
        public ICommand DeleteTaskCommand { get; private set; }

        /// <summary>
        /// Command to import all tasks from a file
        /// </summary>
        public ICommand ImportTasksCommand { get; private set; }

        /// <summary>
        /// Command to export all tasks to a file
        /// </summary>
        public ICommand ExportTasksCommand { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="playerTasksController">The player tasks controller</param>
        [ImportingConstructor]
        public TaskTrackerViewModel(
            IPlayerTasksController playerTasksController,
            CompositionContainer container)
        {
            this.controller = playerTasksController;
            this.container = container;

            this.AddNewTaskCommand = new DelegateCommand(this.AddNewTask);
            this.DeleteTaskCommand = new DelegateCommand<PlayerTask>(this.DeleteTask);
            this.ImportTasksCommand = new DelegateCommand(this.ImportTasks);
            this.ExportTasksCommand = new DelegateCommand(this.ExportTasks);
        }

        /// <summary>
        /// Adds a new task to the collection of tasks
        /// </summary>
        private void AddNewTask()
        {
            logger.Info("Displaying add new task dialog");
            AddNewTaskDialog dialog = new AddNewTaskDialog();
            this.container.ComposeParts(dialog);
            dialog.Show();
        }

        /// <summary>
        /// Deletes a task from the collection of tasks
        /// </summary>
        private void DeleteTask(PlayerTask taskToDelete)
        {
            logger.Info("Deleting task {0}", taskToDelete);
            this.controller.DeleteTask(taskToDelete);
        }

        /// <summary>
        /// Imports a file containing tasks
        /// </summary>
        private void ImportTasks()
        {
            logger.Info("Importing tasks");

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckPathExists = true;
            openFileDialog.Filter = "Player Task Files (*.xml)|*.xml";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                this.controller.ImportTasks(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// Exports a file containing tasks
        /// </summary>
        private void ExportTasks()
        {
            logger.Info("Exporting tasks");

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".xml";
            saveFileDialog.Filter = "Player Task Files (*.xml)|*.xml";
            if (saveFileDialog.ShowDialog() == true)
            {
                this.controller.ExportTasks(saveFileDialog.FileName);
            }
        }
    }
}
