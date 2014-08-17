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

        /// <summary>
        /// Creates the world events time table file
        /// </summary>
        /// <returns></returns>
        public static void CreateTable()
        {
            MegaserverEventTimeTable tt = new MegaserverEventTimeTable();
            tt.WorldEvents = new List<WorldEvent>();
            tt.WorldEvents.Add(new WorldEvent()
                {
                    Name = "Megadestroyer",
                    ID = new Guid("C876757A-EF3E-4FBE-A484-07FF790D9B05"),
                    ActiveTimes = new List<EventTimespan>() { new EventTimespan(0, 30, 0), new EventTimespan(3, 30, 0), new EventTimespan(6, 30, 0), new EventTimespan(9, 30, 0), new EventTimespan(12, 30, 0), new EventTimespan(15, 30, 0), new EventTimespan(18, 30, 0), new EventTimespan(21, 30, 0) },
                    Duration = new EventTimespan(0, 12, 0),
                    WarmupDuration = new EventTimespan(0, 2, 0),
                    WaypointCode = "[&BM0CAAA=]"
                });
            tt.WorldEvents.Add(new WorldEvent()
                {
                    Name = "Tequatl",
                    ID = new Guid("568A30CF-8512-462F-9D67-647D69BEFAED"),
                    ActiveTimes = new List<EventTimespan>() { new EventTimespan(0, 0, 0), new EventTimespan(3, 0, 0), new EventTimespan(7, 0, 0), new EventTimespan(11, 30, 0), new EventTimespan(16, 0, 0), new EventTimespan(19, 0, 0) },
                    Duration = new EventTimespan(0, 15, 0),
                    WarmupDuration = new EventTimespan(0, 0, 0),
                    WaypointCode = "[&BNABAAA=]"
                });
            tt.WorldEvents.Add(new WorldEvent()
            {
                Name = "Karka Queen",
                ID = new Guid("F479B4CF-2E11-457A-B279-90822511B53B"),
                ActiveTimes = new List<EventTimespan>() { new EventTimespan(2, 3, 0), new EventTimespan(6, 3, 0), new EventTimespan(10, 33, 0), new EventTimespan(15, 3, 0), new EventTimespan(18, 3, 0), new EventTimespan(23, 3, 0) },
                Duration = new EventTimespan(0, 5, 0),
                WarmupDuration = new EventTimespan(0, 5, 0),
                WaypointCode = "[&BNcGAAA=]"
            });
            tt.WorldEvents.Add(new WorldEvent()
            {
                Name = "Evolved Jungle Wurm",
                ID = new Guid("5A22EAD4-8302-4DA3-A450-3FC051BD6A3C"),
                ActiveTimes = new List<EventTimespan>() { new EventTimespan(1, 0, 0), new EventTimespan(4, 0, 0), new EventTimespan(8, 0, 0), new EventTimespan(12, 30, 0), new EventTimespan(17, 0, 0), new EventTimespan(20, 0, 0) },
                Duration = new EventTimespan(0, 15, 0),
                WarmupDuration = new EventTimespan(0, 10, 0),
                WaypointCode = "[&BKoBAAA=]"
            });
            tt.WorldEvents.Add(new WorldEvent()
            {
                Name = "Shatterer",
                ID = new Guid("03BF176A-D59F-49CA-A311-39FC6F533F2F"),
                ActiveTimes = new List<EventTimespan>() { new EventTimespan(1, 8, 0), new EventTimespan(4, 8, 0), new EventTimespan(7, 8, 0), new EventTimespan(13, 8, 0), new EventTimespan(13, 8, 0), new EventTimespan(19, 8, 0), new EventTimespan(22, 8, 0) },
                Duration = new EventTimespan(0, 7, 0),
                WarmupDuration = new EventTimespan(0, 6, 0),
                WaypointCode = "[&BE4DAAA=]"
            });
            tt.WorldEvents.Add(new WorldEvent()
            {
                Name = "Jormag",
                ID = new Guid("0464CB9E-1848-4AAA-BA31-4779A959DD71"),
                ActiveTimes = new List<EventTimespan>() { new EventTimespan(2, 33, 0), new EventTimespan(5, 33, 0), new EventTimespan(8, 33, 0), new EventTimespan(11, 33, 0), new EventTimespan(14, 33, 0), new EventTimespan(17, 33, 0), new EventTimespan(20, 33, 0), new EventTimespan(23, 33, 0) },
                Duration = new EventTimespan(0, 17, 0),
                WarmupDuration = new EventTimespan(0, 3, 0),
                WaypointCode = "[&BHoCAAA=]"
            });
            tt.WorldEvents.Add(new WorldEvent()
            {
                Name = "Modniir Ulgoth",
                ID = new Guid("E6872A86-E434-4FC1-B803-89921FF0F6D6"),
                ActiveTimes = new List<EventTimespan>() { new EventTimespan(1, 30, 0), new EventTimespan(4, 30, 0), new EventTimespan(7, 30, 0), new EventTimespan(10, 30, 0), new EventTimespan(13, 30, 0), new EventTimespan(16, 30, 0), new EventTimespan(19, 30, 0), new EventTimespan(22, 30, 0) },
                Duration = new EventTimespan(0, 4, 0),
                WarmupDuration = new EventTimespan(0, 15, 0),
                WaypointCode = "[&BLEAAAA=]"
            });
            tt.WorldEvents.Add(new WorldEvent()
            {
                Name = "Inquest Golem Mark II",
                ID = new Guid("9AA133DC-F630-4A0E-BB5D-EE34A2B306C2"),
                ActiveTimes = new List<EventTimespan>() { new EventTimespan(2, 4, 0), new EventTimespan(5, 4, 0), new EventTimespan(8, 4, 0), new EventTimespan(11, 4, 0), new EventTimespan(14, 4, 0), new EventTimespan(17, 4, 0), new EventTimespan(20, 4, 0), new EventTimespan(23, 4, 0) },
                Duration = new EventTimespan(0, 6, 0),
                WarmupDuration = new EventTimespan(0, 2, 0),
                WaypointCode = "[&BNQCAAA=]"
            });
            tt.WorldEvents.Add(new WorldEvent()
            {
                Name = "Taidha Covington",
                ID = new Guid("242BD241-E360-48F1-A8D9-57180E146789"),
                ActiveTimes = new List<EventTimespan>() { new EventTimespan(0, 15, 0), new EventTimespan(3, 15, 0), new EventTimespan(6, 15, 0), new EventTimespan(9, 15, 0), new EventTimespan(12, 15, 0), new EventTimespan(15, 15, 0), new EventTimespan(18, 15, 0), new EventTimespan(21, 15, 0) },
                Duration = new EventTimespan(0, 4, 0),
                WarmupDuration = new EventTimespan(0, 15, 0),
                WaypointCode = "[&BNQCAAA=]"
            });
            tt.WorldEvents.Add(new WorldEvent()
            {
                Name = "Jungle Wurm",
                ID = new Guid("C5972F64-B894-45B4-BC31-2DEEA6B7C033"),
                ActiveTimes = new List<EventTimespan>() { new EventTimespan(1, 15, 0), new EventTimespan(3, 15, 0), new EventTimespan(5, 15, 0), new EventTimespan(7, 15, 0), new EventTimespan(9, 15, 0), new EventTimespan(11, 15, 0), new EventTimespan(13, 15, 0), new EventTimespan(15, 15, 0), new EventTimespan(17, 15, 0), new EventTimespan(19, 15, 0), new EventTimespan(21, 15, 0), new EventTimespan(23, 15, 0) },
                Duration = new EventTimespan(0, 5, 0),
                WarmupDuration = new EventTimespan(0, 2, 0),
                WaypointCode = "[&BEEFAAA=]"
            });
            tt.WorldEvents.Add(new WorldEvent()
            {
                Name = "Shadow Behemoth",
                ID = new Guid("31CEBA08-E44D-472F-81B0-7143D73797F5"),
                ActiveTimes = new List<EventTimespan>() { new EventTimespan(1, 46, 0), new EventTimespan(3, 46, 0), new EventTimespan(5, 46, 0), new EventTimespan(7, 46, 0), new EventTimespan(9, 46, 0), new EventTimespan(11, 46, 0), new EventTimespan(13, 46, 0), new EventTimespan(15, 46, 0), new EventTimespan(17, 46, 0), new EventTimespan(19, 46, 0), new EventTimespan(21, 46, 0), new EventTimespan(23, 46, 0) },
                Duration = new EventTimespan(0, 9, 0),
                WarmupDuration = new EventTimespan(0, 1, 0),
                WaypointCode = "[&BPwAAAA=]"
            });
            tt.WorldEvents.Add(new WorldEvent()
            {
                Name = "Fire Elemental",
                ID = new Guid("33F76E9E-0BB6-46D0-A3A9-BE4CDFC4A3A4"),
                ActiveTimes = new List<EventTimespan>() { new EventTimespan(0, 54, 0), new EventTimespan(2, 54, 0), new EventTimespan(4, 54, 0), new EventTimespan(6, 54, 0), new EventTimespan(8, 54, 0), new EventTimespan(10, 54, 0), new EventTimespan(12, 54, 0), new EventTimespan(14, 54, 0), new EventTimespan(16, 54, 0), new EventTimespan(18, 54, 0), new EventTimespan(20, 54, 0), new EventTimespan(22, 54, 0) },
                Duration = new EventTimespan(0, 3, 0),
                WarmupDuration = new EventTimespan(0, 9, 0),
                WaypointCode = "[&BEYAAAA=]"
            });

            XmlSerializer serializer = new XmlSerializer(typeof(MegaserverEventTimeTable));
            TextWriter textWriter = new StreamWriter(FileName);
            serializer.Serialize(textWriter, tt);
            textWriter.Close();
        }
    }
}
