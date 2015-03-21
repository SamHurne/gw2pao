using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Data.Entities
{
    public class Dungeon
    {
        public string Name { get; set; }
        public Guid ID { get; set; }
        public int WorldMapID { get; set; }
        public string MapName { get; set; }
        public int MinimumLevel { get; set; }
        public string WaypointCode { get; set; }
        public string WikiUrl { get; set; }
        public List<DungeonPath> Paths { get; set; }
    }

    public class DungeonPath
    {
        /// <summary>
        /// Unique ID for the dungeon path
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Number of the dungeon path (1, 2, 3, 4, etc)
        /// </summary>
        public int PathNumber { get; set; }

        /// <summary>
        /// Map ID for the instance used by the dungeon path
        /// </summary>
        public int InstanceMapID { get; set; }

        /// <summary>
        /// Text displayed in the tracker for the dungeon path (ex: "P1")
        /// </summary>
        public string PathDisplayText { get; set; }

        /// <summary>
        /// Nickname for the dungeon path (ex: "Aetherpath")
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Amount of gold rewarded for completing the dungeon path
        /// </summary>
        public double GoldReward { get; set; }

        /// <summary>
        /// Collection of points that identify the path. These are ORed in order
        /// to detect which path the user is completing
        /// </summary>
        public List<DetectionPoint> IdentifyingPoints { get; set; }

        /// <summary>
        /// Endpoint of the dungeon path
        /// </summary>
        public DetectionPoint EndPoint { get; set; }

        /// <summary>
        /// Number of cutscenes shown while at the endpoint
        /// </summary>
        public int EndCutsceneCount { get; set; }

        /// <summary>
        /// Constructs a new DungeonPath object
        /// </summary>
        public DungeonPath()
        {
            this.IdentifyingPoints = new List<DetectionPoint>();
        }
    }
}
