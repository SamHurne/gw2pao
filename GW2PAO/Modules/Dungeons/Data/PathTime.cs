using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.Modules.Dungeons.ViewModels;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Dungeons.Data
{
    /// <summary>
    /// Class containing a timespan associated with a dungeon path completion
    /// </summary>
    public class PathTime : BindableBase
    {
        private Guid pathID;
        private DateTime timestamp;

        /// <summary>
        /// ID that this path corresponds to
        /// </summary>
        public Guid PathID
        {
            get { return this.pathID; }
            set { this.SetProperty(ref this.pathID, value); }
        }

        /// <summary>
        /// The actual path time
        /// </summary>
        [XmlIgnore]
        public TimeSpan Time
        {
            get { return this.SavedTime.Time; }
            set { this.SetProperty(ref this.SavedTime.Time, value); }
        }

        /// <summary>
        /// Timestamp for this path time
        /// </summary>
        public DateTime Timestamp
        {
            get { return this.timestamp; }
            set { this.SetProperty(ref this.timestamp, value); }
        }

        /// <summary>
        /// Detailed information for the dungeon path
        /// </summary>
        [XmlIgnore]
        public PathViewModel PathData
        {
            get;
            set;
        }

        /// <summary>
        /// Serializable version of PathTime
        /// </summary>
        public SerializableTimespan SavedTime
        {
            get;
            set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PathTime()
        {
            this.SavedTime = new SerializableTimespan();
        }

        /// <summary>
        /// Parameterizes constructor
        /// </summary>
        /// <param name="pathId">The Path ID</param>
        /// <param name="pathName">The Path Name</param>
        public PathTime(PathViewModel pathData)
        {
            this.PathID = pathData.PathId;
            this.PathData = pathData;
            this.SavedTime = new SerializableTimespan();
        }
    }
}

