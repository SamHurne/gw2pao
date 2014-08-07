using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.API.Data
{
    public class WvWObjective
    {
        public int ID { get; set; }
        public ObjectiveType Type { get; set; }
        public WvWMap Map { get; set; }
        public string MatchId { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Location { get; set; }
        public Point MapLocation { get; set; }
        public WorldColor WorldOwner { get; set; }
        public Guid GuildOwner { get; set; }
        public int Points { get; set; }
    }
}
