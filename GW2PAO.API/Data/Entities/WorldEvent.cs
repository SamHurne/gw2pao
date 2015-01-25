using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GW2PAO.API.Data.Entities
{
    public class WorldEvent
    {
        public string Name { get; set; }
        public Guid ID { get; set; }
        public int MapID { get; set; }
        public string MapName { get; set; }
        public List<EventTimespan> ActiveTimes { get; set; }
        public EventTimespan Duration { get; set; }
        public EventTimespan WarmupDuration { get; set; }
        public string WaypointCode { get; set; }
        public List<Point> CompletionLocations { get; set; }
        public double CompletionRadius { get; set; }

        public WorldEvent()
        {
            this.ActiveTimes = new List<EventTimespan>();
            this.Duration = new EventTimespan();
            this.WarmupDuration = new EventTimespan();
            this.CompletionLocations = new List<Point>();
        }
    }

    /// <summary>
    /// Helper class due to limitation in serializing timespan objects
    /// </summary>
    [Serializable]
    public class EventTimespan
    {
        // Public Property - XmlIgnore as it doesn't serialize anyway
        [XmlIgnore]
        public TimeSpan Time { get; set; }

        // Pretend property for serialization
        [XmlElement("Time")]
        public string XmlTime
        {
            get { return Time.ToString(); }
            set { Time = TimeSpan.Parse(value); }
        }

        public EventTimespan(int hours, int minutes, int seconds)
        {
            this.Time = new TimeSpan(hours, minutes, seconds);
        }

        public EventTimespan()
        {
            this.Time = new TimeSpan();
        }
    }
}
