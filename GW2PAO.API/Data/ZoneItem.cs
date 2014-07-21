using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.API.Data
{
    public class ZoneItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ZoneItemType Type { get; set; }
        public int MapId { get; set; }
        public string MapName { get; set; }
        public int Level { get; set; }
        public Point Location { get; set; }
    }
}
