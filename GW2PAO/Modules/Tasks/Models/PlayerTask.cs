using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Entities;
using Microsoft.Practices.Prism.Mvvm;
using GW2PAO.Utility;

namespace GW2PAO.Modules.Tasks.Models
{
    [Serializable]
    public class PlayerTask : BindableBase
    {
        private Guid id;
        private string name;
        private string description;
        private bool isCompletable;
        private bool isAccountCompleted;
        private bool isCompletedPerCharacter;
        private bool autoComplete;
        private bool isDailyReset;
        private Point continentLocation;
        private Point location;
        private int mapId;
        private string iconUri;
        private string waypointCode;
        private string category;

        private ObservableDictionary<string, bool> characterCompletions = new ObservableDictionary<string, bool>();

        /// <summary>
        /// Unique ID of the player task
        /// </summary>
        public Guid ID
        {
            get { return this.id; }
            set { SetProperty(ref this.id, value); }
        }

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
        /// True if the task is completed account-wide, else false
        /// </summary>
        public bool IsAccountCompleted
        {
            get { return this.isAccountCompleted; }
            set { SetProperty(ref this.isAccountCompleted, value); }
        }

        /// <summary>
        /// True if the task is completed on a per-character basis, else false
        /// </summary>
        public bool IsCompletedPerCharacter
        {
            get { return this.isCompletedPerCharacter; }
            set { SetProperty(ref this.isCompletedPerCharacter, value); }
        }

        /// <summary>
        /// Dictionary containing the character completions, if completed on a per-character basis
        /// NOTE: The setter is public only for serialization purposes!!!
        /// </summary>
        public ObservableDictionary<string, bool> CharacterCompletions
        {
            get { return this.characterCompletions; }
            set { this.characterCompletions = value; }
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
        /// The location of the task in the corresponding continent, if any
        /// Null if no location exists
        /// </summary>
        public Point ContinentLocation
        {
            get { return this.continentLocation; }
            set { SetProperty(ref this.continentLocation, value); }
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
        /// True if the task should automatically be marked as completed
        /// (based on location), else false
        /// </summary>
        public bool AutoComplete
        {
            get { return this.autoComplete; }
            set { SetProperty(ref this.autoComplete, value); }
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
            set
            {
                if (value == string.Empty)
                    value = null;

                SetProperty(ref this.iconUri, value);
            }
        }

        /// <summary>
        /// Waypoint code to use for the task, if any
        /// </summary>
        public string WaypointCode
        {
            get { return this.waypointCode; }
            set
            {
                if (value == string.Empty)
                    value = null;

                SetProperty(ref this.waypointCode, value);
            }
        }

        /// <summary>
        /// Category of the player task
        /// </summary>
        public string Category
        {
            get { return this.category; }
            set { SetProperty(ref this.category, value); }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PlayerTask()
        {
            this.ID = Guid.NewGuid();
        }

        /// <summary>
        /// Constructs a new player task object using the data from the given task
        /// </summary>
        /// <param name="other">The other task to construct from</param>
        public PlayerTask(PlayerTask other)
        {
            this.ID = other.ID;
            this.Name = other.Name;
            this.Description = other.Description;
            this.IsCompletable = other.IsCompletable;
            this.IsAccountCompleted = other.IsAccountCompleted;
            this.IsCompletedPerCharacter = other.IsCompletedPerCharacter;
            this.AutoComplete = other.AutoComplete;
            this.IsDailyReset = other.IsDailyReset;
            this.ContinentLocation = other.Location;
            this.Location = other.Location;
            this.MapID = other.MapID;
            this.IconUri = other.IconUri;
            this.WaypointCode = other.WaypointCode;
            this.Category = other.Category;
            
            foreach (var character in other.CharacterCompletions.Keys)
            {
                this.CharacterCompletions.Add(character, other.CharacterCompletions[character]);
            }
        }
    }
}
