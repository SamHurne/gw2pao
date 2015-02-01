using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Entities;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Tasks.Models
{
    [Serializable]
    public class PlayerTask : BindableBase
    {
        private string name;
        private string description;
        private bool isCompletable;
        private bool isCompleted;
        private bool isDailyReset;
        private Point location;
        private int mapId;
        private string iconUri;

        /// <summary>
        /// Name of the task
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { SetProperty(ref this.name, value); }
        }

        /// <summary>
        /// Description for the task
        /// </summary>
        public string Description
        {
            get { return this.description; }
            set { SetProperty(ref this.description, value); }
        }

        /// <summary>
        /// True if the task can be completed in some way, else false
        /// </summary>
        public bool IsCompletable
        {
            get { return this.isCompletable; }
            set { SetProperty(ref this.isCompletable, value); }
        }

        /// <summary>
        /// True if the task has a checkbox and is completed, else false
        /// </summary>
        public bool IsCompleted
        {
            get { return this.isCompleted; }
            set { SetProperty(ref this.isCompleted, value); }
        }

        /// <summary>
        /// True if the task is automatically reset on a daily basis, else false
        /// </summary>
        public bool IsDailyReset
        {
            get { return this.isDailyReset; }
            set { SetProperty(ref this.isDailyReset, value); }
        }

        /// <summary>
        /// The location of the task, if any
        /// Null if no location exists
        /// </summary>
        public Point Location
        {
            get { return this.location; }
            set { SetProperty(ref this.location, value); }
        }

        /// <summary>
        /// The map ID of the task, if any
        /// -1 if no location exists
        /// </summary>
        public int MapID
        {
            get { return this.mapId; }
            set { SetProperty(ref this.mapId, value); }
        }

        /// <summary>
        /// Icon for the task, if any
        /// Null if no icon exists
        /// </summary>
        public string IconUri
        {
            get { return this.iconUri; }
            set { SetProperty(ref this.iconUri, value); }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PlayerTask()
        {

        }
    }
}
