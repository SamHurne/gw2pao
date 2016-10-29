using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.API.Data.Enums;
using GW2PAO.Modules.Tasks.Interfaces;
using GW2PAO.Modules.Tasks.Models;
using GW2PAO.Modules.Tasks.Views;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Win32;
using NLog;

namespace GW2PAO.Modules.Tasks.ViewModels
{
    /// <summary>
    /// Task Tracker view model class
    /// </summary>
    [PartCreationPolicy(CreationPolicy.Shared)]
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
        /// The backing collection of task categories
        /// </summary>
        private ObservableCollection<TaskCategoryViewModel> taskCategories;

        /// <summary>
        /// Collection of task categories
        /// </summary>
        public AutoRefreshCollectionViewSource TaskCategories
        {
            get;
            private set;
        }

        /// <summary>
        /// The player tasks user data
        /// </summary>
        public TasksUserData UserData
        {
            get { return this.controller.UserData; }
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
                    this.OnPropertyChanged(() => this.IsFeetSelected);
                    this.OnPropertyChanged(() => this.IsMetersSelected);
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
                    this.OnPropertyChanged(() => this.IsFeetSelected);
                    this.OnPropertyChanged(() => this.IsMetersSelected);
                }
            }
        }

        /// <summary>
        /// True if the tasks should be sorted by Name, else false
        /// </summary>
        public bool SortByName
        {
            get
            {
                return this.UserData.TaskTrackerSortProperty == TasksUserData.TASK_TRACKER_SORT_NAME;
            }
            set
            {
                if (this.UserData.TaskTrackerSortProperty != TasksUserData.TASK_TRACKER_SORT_NAME)
                {
                    this.OnSortingPropertyChanged(TasksUserData.TASK_TRACKER_SORT_NAME, ListSortDirection.Ascending);
                }
            }
        }

        /// <summary>
        /// True if the tasks should be sorted by Distance, else false
        /// </summary>
        public bool SortByDistance
        {
            get
            {
                return this.UserData.TaskTrackerSortProperty == TasksUserData.TASK_TRACKER_SORT_DISTANCE;
            }
            set
            {
                if (this.UserData.TaskTrackerSortProperty != TasksUserData.TASK_TRACKER_SORT_DISTANCE)
                {
                    this.OnSortingPropertyChanged(TasksUserData.TASK_TRACKER_SORT_DISTANCE, ListSortDirection.Ascending);
                }
            }
        }

        /// <summary>
        /// Command to add a new task to the task list
        /// </summary>
        public ICommand AddNewTaskCommand { get; private set; }

        /// <summary>
        /// Command to delete all tasks from the task list
        /// </summary>
        public ICommand DeleteAllCommand { get; private set; }

        /// <summary>
        /// Command to load tasks from a file
        /// </summary>
        public ICommand LoadTasksCommand { get; private set; }

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

            this.AddNewTaskCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(this.AddNewTask);
            this.DeleteAllCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(this.DeleteAllTasks);
            this.LoadTasksCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(this.LoadTasks);
            this.ImportTasksCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(this.ImportTasks);
            this.ExportTasksCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(this.ExportTasks);

            this.taskCategories = new ObservableCollection<TaskCategoryViewModel>();
            this.TaskCategories = new AutoRefreshCollectionViewSource();
            this.TaskCategories.Source = this.taskCategories;
            foreach (var t in this.controller.PlayerTasks)
            {
                var category = this.taskCategories.FirstOrDefault(c => c.CategoryName == t.Category);
                if (category == null)
                    this.taskCategories.Add(new TaskCategoryViewModel(t, this.controller, this.UserData));
                else
                    category.Add(t);
                t.PropertyChanged += Task_PropertyChanged;
            }
            this.controller.PlayerTasks.CollectionChanged += PlayerTasks_CollectionChanged;
            this.TaskCategories.SortDescriptions.Add(new SortDescription("CategoryName", ListSortDirection.Ascending));
        }

        private void PlayerTasks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (PlayerTaskViewModel t in e.NewItems)
                    {
                        var category = this.taskCategories.FirstOrDefault(c => c.CategoryName == t.Category);
                        if (category == null)
                            this.taskCategories.Add(new TaskCategoryViewModel(t, this.controller, this.UserData));
                        else
                            category.Add(t);
                        t.PropertyChanged += this.Task_PropertyChanged;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (PlayerTaskViewModel t in e.OldItems)
                    {
                        t.PropertyChanged -= this.Task_PropertyChanged;
                        var category = this.taskCategories.FirstOrDefault(c => c.CategoryName == t.Category);
                        if (category != null)
                            category.Remove(t);

                        if (category.IsEmpty)
                            this.taskCategories.Remove(category);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var category in this.taskCategories)
                    {
                        var tasks = (ICollection<PlayerTaskViewModel>)category.PlayerTasks.Source;
                        foreach (var t in tasks)
                        {
                            t.PropertyChanged -= this.Task_PropertyChanged;
                        }
                    }
                    this.taskCategories.Clear();
                    break;
                default:
                    break;
            }
        }

        private void Task_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Category")
            {
                // Move the task to it's new category
                var task = sender as PlayerTaskViewModel;

                var prevCategory = this.taskCategories.FirstOrDefault(c => c.Contains(task));
                if (prevCategory != null)
                    prevCategory.Remove(task);
                if (prevCategory.IsEmpty)
                    this.taskCategories.Remove(prevCategory);

                var category = this.taskCategories.FirstOrDefault(c => c.CategoryName == task.Category);
                if (category == null)
                    this.taskCategories.Add(new TaskCategoryViewModel(task, this.controller, this.UserData));
                else
                    category.Add(task);
            }
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
        /// Deletes all tasks from the collection of tasks
        /// </summary>
        private void DeleteAllTasks()
        {
            var result = Xceed.Wpf.Toolkit.MessageBox.Show(Properties.Resources.DeleteTasksConfirmation, string.Empty, System.Windows.MessageBoxButton.YesNo);
            if (result == System.Windows.MessageBoxResult.Yes)
            {
                logger.Info("Deleting all tasks");
                var tasksToDelete = new List<PlayerTaskViewModel>(this.controller.PlayerTasks);
                foreach (var pt in tasksToDelete)
                {
                    this.controller.DeleteTask(pt.Task);
                }
            }
        }

        /// <summary>
        /// Imports a file containing tasks
        /// </summary>
        private void LoadTasks()
        {
            logger.Info("Loading tasks");

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckPathExists = true;
            openFileDialog.Filter = "Player Task Files (*.xml)|*.xml";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                this.controller.LoadTasksFile(openFileDialog.FileName);
            }
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

        /// <summary>
        /// Handles updating the sorting descriptions of the Objectives collection
        /// and raising INotifyPropertyChanged for all sort properties
        /// </summary>
        private void OnSortingPropertyChanged(string property, ListSortDirection direction)
        {
            foreach (var category in this.taskCategories)
            {
                category.SortBy = property;
            }

            this.UserData.TaskTrackerSortProperty = property;
            this.OnPropertyChanged(() => this.SortByName);
            this.OnPropertyChanged(() => this.SortByDistance);
        }
    }
}
