using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GW2PAO.API.Data.Entities
{
    public class WorldBossEvent
    {
        public string Name { get; set; }
        public Guid ID { get; set; }
        public int MapID { get; set; }
        public string MapName { get; set; }
        public List<SerializableTimespan> ActiveTimes { get; set; }
        public SerializableTimespan Duration { get; set; }
        public SerializableTimespan WarmupDuration { get; set; }
        public string WaypointCode { get; set; }
        public List<Point> CompletionLocations { get; set; }
        public double CompletionRadius { get; set; }

        public WorldBossEvent()
        {
            this.ActiveTimes = new List<SerializableTimespan>();
            this.Duration = new SerializableTimespan();
            this.WarmupDuration = new SerializableTimespan();
            this.CompletionLocations = new List<Point>();
        }
    }
}
