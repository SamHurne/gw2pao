using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.API.Constants;
using GW2PAO.API.Data.Entities;

namespace GW2PAO.API.Data
{
    /// <summary>
    /// Data table containing meta event cycle information
    /// </summary>
    public class MetaEventsTable
    {
        /// <summary>
        /// File name for the data xml file
        /// </summary>
        public static readonly string Filename = "MetaEventTable.xml";

        /// <summary>
        /// List of meta events and their details
        /// </summary>
        public List<MetaEvent> MetaEvents { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MetaEventsTable()
        {
            this.MetaEvents = new List<MetaEvent>();
        }

        /// <summary>
        /// Loads the meta events data xml file
        /// </summary>
        /// <returns>The loaded meta events data</returns>
        public static MetaEventsTable LoadTable()
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(MetaEventsTable));
            TextReader reader = new StreamReader(MetaEventsTable.Filename);
            MetaEventsTable loadedData = null;
            try
            {
                object obj = deserializer.Deserialize(reader);
                loadedData = (MetaEventsTable)obj;
            }
            finally
            {
                reader.Close();
            }

            return loadedData;
        }

        /// <summary>
        /// Creates the meta events data xml file
        /// </summary>
        /// <returns></returns>
        public static void CreateTable()
        {
            MetaEventsTable met = new MetaEventsTable();

            // Note: Names actually come from a localized names provider, but are left here for reference
            // in the xml file

            met.MetaEvents.Add(new MetaEvent()
            {
                Name = "Dry Top",
                ID = MetaEventID.DryTop,
                MapID = 988,
                StartOffset = new SerializableTimespan(0, 0, 0),
                Stages = new List<MetaEventStage>()
                {
                    new MetaEventStage() { ID = MetaEventStageID.DryTop_CrashSite, Name = "Crash Site", Duration = new SerializableTimespan(0, 40, 0) },
                    new MetaEventStage() { ID = MetaEventStageID.DryTop_Sandstorm, Name = "Sandstorm", Duration = new SerializableTimespan(0, 20, 0) }
                }
            });

            met.MetaEvents.Add(new MetaEvent()
            {
                Name = "Verdant Brink",
                ID = MetaEventID.VerdantBrink,
                MapID = 1052,
                StartOffset = new SerializableTimespan(0, 10, 0),
                Stages = new List<MetaEventStage>()
                {
                    new MetaEventStage() { ID = MetaEventStageID.VerdantBrink_NightBosses, Name = "Night Bosses", Duration = new SerializableTimespan(0, 20, 0) },
                    new MetaEventStage() { ID = MetaEventStageID.VerdantBrink_Daytime, Name = "Daytime", Duration = new SerializableTimespan(1, 15, 0) },
                    new MetaEventStage() { ID = MetaEventStageID.VerdantBrink_Night, Name = "Night", Duration = new SerializableTimespan(0, 25, 0) }
                }
            });

            met.MetaEvents.Add(new MetaEvent()
            {
                Name = "Auric Basin",
                ID = MetaEventID.AuricBasin,
                MapID = 1043,
                StartOffset = new SerializableTimespan(0, 45, 0),
                Stages = new List<MetaEventStage>()
                {
                    new MetaEventStage() { ID = MetaEventStageID.AuricBasin_Challenges, Name = "Challenges", Duration = new SerializableTimespan(0, 15, 0) },
                    new MetaEventStage() { ID = MetaEventStageID.AuricBasin_Octovine, Name = "Octovine", Duration = new SerializableTimespan(0, 20, 0) },
                    new MetaEventStage() { ID = MetaEventStageID.AuricBasin_Reset, Name = "Reset", Duration = new SerializableTimespan(0, 10, 0) },
                    new MetaEventStage() { ID = MetaEventStageID.AuricBasin_Pylons, Name = "Pylons", Duration = new SerializableTimespan(1, 15, 0) }
                }
            });

            met.MetaEvents.Add(new MetaEvent()
            {
                Name = "Tangled Depths",
                ID = MetaEventID.TangledDepths,
                MapID = 1045,
                StartOffset = new SerializableTimespan(0, 25, 0),
                Stages = new List<MetaEventStage>()
                {
                    new MetaEventStage() { ID = MetaEventStageID.TangledDepths_Preparation, Name = "Preparation", Duration = new SerializableTimespan(0, 5, 0) },
                    new MetaEventStage() { ID = MetaEventStageID.TangledDepths_ChakGerent, Name = "Chak Gerent", Duration = new SerializableTimespan(0, 20, 0) },
                    new MetaEventStage() { ID = MetaEventStageID.TangledDepths_HelpOutposts, Name = "Help Outposts", Duration = new SerializableTimespan(1, 35, 0) }
                }
            });

            met.MetaEvents.Add(new MetaEvent()
            {
                Name = "Dragon's Stand",
                ID = MetaEventID.DragonsStand,
                MapID = 1041,
                StartOffset = new SerializableTimespan(1, 30, 0),
                Stages = new List<MetaEventStage>()
                {
                    new MetaEventStage() { ID = MetaEventStageID.DragonsStand_MapActive, Name = "Map Active", Duration = new SerializableTimespan(2, 0, 0) }
                }
            });

            XmlSerializer serializer = new XmlSerializer(typeof(MetaEventsTable));
            TextWriter textWriter = new StreamWriter(MetaEventsTable.Filename);
            serializer.Serialize(textWriter, met);
            textWriter.Close();
        }
    }
}
