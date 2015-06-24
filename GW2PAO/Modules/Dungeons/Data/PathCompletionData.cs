using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.Modules.Dungeons.ViewModels;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Dungeons.Data
{
    /// <summary>
    /// Class containing dungeon path completion data for a dungeon path
    /// </summary>
    public class PathCompletionData : BindableBase
    {
        private Guid pathID;
        private ObservableCollection<PathTime> completionTimes = new ObservableCollection<PathTime>();
        /// <summary>
        /// ID that this path corresponds to
        /// </summary>
        public Guid PathID
        {
            get { return this.pathID; }
            set { this.SetProperty(ref this.pathID, value); }
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
        /// Collection of dungeon path completion times
        /// </summary>
        public ObservableCollection<PathTime> CompletionTimes { get { return this.completionTimes; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PathCompletionData()
        {
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="pathData">The Path data view model object</param>
        public PathCompletionData(PathViewModel pathData)
        {
            this.PathID = pathData.PathId;
            this.PathData = pathData;
        }
    }
}
