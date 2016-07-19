using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GW2PAO.API.Data.Entities
{
    /// <summary>
    /// Helper class due to limitation in serializing timespan objects
    /// </summary>
    [Serializable]
    public class SerializableTimespan
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

        public SerializableTimespan(int hours, int minutes, int seconds)
        {
            this.Time = new TimeSpan(hours, minutes, seconds);
        }

        public SerializableTimespan()
        {
            this.Time = new TimeSpan();
        }
    }
}
