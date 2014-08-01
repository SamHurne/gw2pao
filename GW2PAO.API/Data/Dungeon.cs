using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Data
{
    public class Dungeon
    {
        public string Name { get; set; }
        public Guid ID { get; set; }
        public string Location { get; set; }
        public int MinimumLevel { get; set; }
        public string WaypointCode { get; set; }
        public string WikiUrl { get; set; }
        public List<DungeonPath> Paths { get; set; }
    }

    public class DungeonPath
    {
        public int PathNumber { get; set; }
        public Guid ID { get; set; }
        public string PathDisplayText { get; set; }
        public string Nickname { get; set; }
        public double GoldReward { get; set; }
    }
}
