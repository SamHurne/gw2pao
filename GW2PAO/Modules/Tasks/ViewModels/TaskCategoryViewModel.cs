using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Modules.Tasks.Interfaces;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

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
        private string sortBy;

        /// <summary>
        /// The category's name
        /// </summary>
        public string CategoryName
        {
            get { return this.categoryName; }
            set { SetProperty(ref this.categoryName, value); }
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

        public TaskCategoryViewModel(PlayerTaskViewModel initialTask, TasksUserData userData)
        {
            this.CategoryName = initialTask.Category;
            this.playerTasks = new ObservableCollection<PlayerTaskViewModel>();
            this.PlayerTasks = new AutoRefreshCollectionViewSource();
            this.PlayerTasks.Source = this.playerTasks;
            this.playerTasks.Add(initialTask);

            this.userData = userData;
            this.SortBy = this.userData.TaskTrackerSortProperty;
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
    }
}
