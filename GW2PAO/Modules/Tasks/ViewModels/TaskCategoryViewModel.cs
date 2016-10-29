using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.Modules.Tasks.Interfaces;
using GW2PAO.Modules.Tasks.Views;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using Xceed.Wpf.Toolkit;

namespace GW2PAO.Modules.Tasks.ViewModels
{
    public class TaskCategoryViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private string categoryName;
        private ObservableCollection<PlayerTaskViewModel> playerTasks;
        private TasksUserData userData;
        IPlayerTasksController playerTasksController;
        private string sortBy;

        /// <summary>
        /// The category's name
        /// </summary>
        public string CategoryName
        {
            get { return this.categoryName; }
            set
            {
                if (SetProperty(ref this.categoryName, value))
                {
                    var toEdit = new List<PlayerTaskViewModel>(this.playerTasks);
                    foreach (var t in toEdit)
                    {
                        if (t.Task.Category != value)
                            t.Task.Category = value;
                    }
                }
            }
        }

        /// <summary>
        /// Collection of player tasks
        /// </summary>
        public AutoRefreshCollectionViewSource PlayerTasks
        {
            get;
            private set;
        }

        /// <summary>
        /// True if this category is empty, else false
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.playerTasks.Count == 0;
            }
        }

        /// <summary>
        /// The property by which to sort the tasks in this category
        /// </summary>
        public string SortBy
        {
            get { return this.sortBy; }
            set
            {
                if (SetProperty(ref this.sortBy, value))
                {
                    this.PlayerTasks.SortDescriptions.Clear();
                    this.PlayerTasks.SortDescriptions.Add(new SortDescription(this.sortBy, ListSortDirection.Ascending));
                    this.PlayerTasks.View.Refresh();
                }
            }
        }

        /// <summary>
        /// Command to edit all tasks' category name under this category
        /// </summary>
        public ICommand EditCategoryCommand { get; private set; }

        /// <summary>
        /// Command to delete the all tasks under this category
        /// </summary>
        public ICommand DeleteAllCommand { get; private set; }

        public TaskCategoryViewModel(PlayerTaskViewModel initialTask, IPlayerTasksController playerTasksController, TasksUserData userData)
        {
            this.playerTasks = new ObservableCollection<PlayerTaskViewModel>();
            this.PlayerTasks = new AutoRefreshCollectionViewSource();
            this.PlayerTasks.Source = this.playerTasks;
            this.playerTasks.Add(initialTask);
            this.CategoryName = initialTask.Category;

            this.userData = userData;
            this.playerTasksController = playerTasksController;
            this.SortBy = this.userData.TaskTrackerSortProperty;
            this.EditCategoryCommand = new DelegateCommand(this.Edit);
            this.DeleteAllCommand = new DelegateCommand(this.DeleteAll);
        }

        public void Add(PlayerTaskViewModel playerTask)
        {
            this.playerTasks.Add(playerTask);
        }

        public void Remove(PlayerTaskViewModel playerTask)
        {
            if (this.playerTasks.Contains(playerTask))
                this.playerTasks.Remove(playerTask);
        }

        public bool Contains(PlayerTaskViewModel playerTask)
        {
            return this.playerTasks.Contains(playerTask);
        }

        private void Edit()
        {
            logger.Info("Displaying edit category dialog");
            EditCategoryViewModel vm = new EditCategoryViewModel(this);
            EditCategoryDialog dialog = new EditCategoryDialog(vm);
            dialog.Show();
        }

        private void DeleteAll()
        {
            var result = Xceed.Wpf.Toolkit.MessageBox.Show(Properties.Resources.DeleteCategoryConfirmation, string.Empty, System.Windows.MessageBoxButton.YesNo);
            if (result == System.Windows.MessageBoxResult.Yes)
            {
                var toRemove = new List<PlayerTaskViewModel>(this.playerTasks);
                foreach (var t in toRemove)
                {
                    this.playerTasksController.DeleteTask(t.Task);
                }
            }
        }
    }
}
