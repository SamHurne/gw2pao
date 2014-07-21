using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.API.Data
{
    public class DynamicEvent
    {
        public string Name { get; set; }
        public Guid ID { get; set; }
        public int MapId { get; set; }
        public string MapName { get; set; }
        public int WorldId { get; set; }
        public int Level { get; set; }
        public Point Location { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public List<EventFlag> Flags { get; set; }

        public DynamicEvent()
        {
            this.Flags = new List<EventFlag>();
        }
    }
}
