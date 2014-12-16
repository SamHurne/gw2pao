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
        public Guid ID { get; set; }
        public int PathNumber { get; set; }
        public int InstanceMapID { get; set; }
        public string PathDisplayText { get; set; }
        public string Nickname { get; set; }
        public double GoldReward { get; set; }
        public Point EndPoint { get; set; }
        public List<Point> IdentifyingPoints { get; set; }
        public List<Point> CompletionPrereqPoints { get; set; }
        public double PointDetectionRadius { get; set; }

        public DungeonPath()
        {
            this.IdentifyingPoints = new List<Point>();
            this.CompletionPrereqPoints = new List<Point>();
        }
    }
}
