using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Enums;
using GW2PAO.Data.UserData;
using GW2PAO.Modules.Tasks.Models;
using GW2PAO.Modules.Tasks.ViewModels;
using NLog;

namespace GW2PAO.Modules.Tasks
{
    /// <summary>
    /// User settings for the Tasks Tracker and Task Notifications
    /// </summary>
    [Serializable]
    public class TasksUserData : UserData<TasksUserData>
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default settings filename
        /// </summary>
        public const string Filename = "TasksUserData.xml";

        public const string TASK_TRACKER_SORT_NAME = "Name";
        public const string TASK_TRACKER_SORT_DISTANCE = "DistanceFromPlayer";

        private Units distanceUnits;
        private DateTime lastCompletionDateTime;
        private string taskTrackerSortProperty;
        private ObservableCollection<PlayerTask> tasks = new ObservableCollection<PlayerTask>();

        /// <summary>
        /// The units used for calculated distances
        /// </summary>
        public Units DistanceUnits
        {
            get { return this.distanceUnits; }
            set { SetProperty(ref this.distanceUnits, value); }
        }

        /// <summary>
        /// The last time the tasks were reset
        /// </summary>
        public DateTime LastResetDateTime
        {
            get { return this.lastCompletionDateTime; }
            set { SetProperty(ref this.lastCompletionDateTime, value); }
        }

        /// <summary>
        /// The property name to use when sorting items in the Task Tracker
        /// </summary>
        public string TaskTrackerSortProperty
        {
            get { return this.taskTrackerSortProperty; }
            set { this.SetProperty(ref taskTrackerSortProperty, value); }
        }

        /// <summary>
        /// Collection of user-configured Tasks
        /// </summary>
        public ObservableCollection<PlayerTask> Tasks { get { return this.tasks; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksUserData()
        {
        }

        /// <summary>
        /// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
        /// </summary>
        public override void EnableAutoSave()
        {
            logger.Info("Enabling auto save");
            this.PropertyChanged += (o, e) => TasksUserData.SaveData(this, TasksUserData.Filename);
            this.Tasks.CollectionChanged += (o, e) =>
            {
                TasksUserData.SaveData(this, TasksUserData.Filename);
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (PlayerTask task in e.NewItems)
                    {
                        task.PropertyChanged += (obj, arg) => TasksUserData.SaveData(this, TasksUserData.Filename);
                    }
                }
            };
            foreach (PlayerTask task in this.Tasks)
            {
                task.PropertyChanged += (obj, arg) => TasksUserData.SaveData(this, TasksUserData.Filename);
            }
        }
    }
}
