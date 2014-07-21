using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.API.Data;

namespace GW2PAO.API.Services
{
    /// <summary>
    /// The Megaserver event time table
    /// </summary>
    public class MegaserverEventTimeTable
    {
        /// <summary>
        /// File name for the time table
        /// </summary>
        public static readonly string FileName = "EventTimeTable.xml";

        /// <summary>
        /// List of world events and their details
        /// </summary>
        public List<WorldEvent> WorldEvents { get; set; }

        /// <summary>
        /// Loads the world events time table file
        /// </summary>
        /// <returns>The loaded event time table data</returns>
        public static MegaserverEventTimeTable LoadTable()
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(MegaserverEventTimeTable));
            TextReader reader = new StreamReader(FileName);
            object obj = deserializer.Deserialize(reader);
            MegaserverEventTimeTable loadedData = (MegaserverEventTimeTable)obj;
            reader.Close();

            return loadedData;
        }
    }
}
