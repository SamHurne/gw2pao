using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;

namespace GW2PAO.API.Data
{
    /// <summary>
    /// The Worlds Table containing all IDs and Names for the worlds
    /// </summary>
    public class WorldsTable
    {
        /// <summary>
        /// File name for the table
        /// </summary>
        public static readonly string FileName = "Worlds.xml";

        /// <summary>
        /// List of worlds and their details
        /// </summary>
        public List<World> Worlds { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public WorldsTable()
        {
            this.Worlds = new List<World>();
        }

        /// <summary>
        /// Loads the world table file
        /// </summary>
        /// <returns>The loaded world names table data</returns>
        public static WorldsTable LoadTable()
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(WorldsTable));
            TextReader reader = new StreamReader(FileName);
            object obj = deserializer.Deserialize(reader);
            WorldsTable loadedData = (WorldsTable)obj;
            reader.Close();

            return loadedData;
        }

        /// <summary>
        /// Creates the world table file
        /// </summary>
        public static void CreateTable()
        {
            WorldsTable wnTable = new WorldsTable();
            wnTable.Worlds.Add(new World { ID = 1001, Name = "Anvil Rock" });
            wnTable.Worlds.Add(new World { ID = 1002, Name = "Borlis Pass" });
            wnTable.Worlds.Add(new World { ID = 1003, Name = "Yak's Bend" });
            wnTable.Worlds.Add(new World { ID = 1004, Name = "Henge of Denravi" });
            wnTable.Worlds.Add(new World { ID = 1005, Name = "Maguuma" });
            wnTable.Worlds.Add(new World { ID = 1006, Name = "Sorrow's Furnace" });
            wnTable.Worlds.Add(new World { ID = 1007, Name = "Gate of Madness" });
            wnTable.Worlds.Add(new World { ID = 1008, Name = "Jade Quarry" });
            wnTable.Worlds.Add(new World { ID = 1009, Name = "Fort Aspenwood" });
            wnTable.Worlds.Add(new World { ID = 1010, Name = "Ehmry Bay" });
            wnTable.Worlds.Add(new World { ID = 1011, Name = "Stormbluff Isle" });
            wnTable.Worlds.Add(new World { ID = 1012, Name = "Darkhaven" }); 
            wnTable.Worlds.Add(new World { ID = 1013, Name = "Sanctum of Rall" });
            wnTable.Worlds.Add(new World { ID = 1014, Name = "Crystal Desert" });
            wnTable.Worlds.Add(new World { ID = 1015, Name = "Isle of Janthir" });
            wnTable.Worlds.Add(new World { ID = 1016, Name = "Sea of Sorrows" });
            wnTable.Worlds.Add(new World { ID = 1017, Name = "Tarnished Coast" });
            wnTable.Worlds.Add(new World { ID = 1018, Name = "Northern Shiverpeaks" });
            wnTable.Worlds.Add(new World { ID = 1019, Name = "Blackgate" });
            wnTable.Worlds.Add(new World { ID = 1020, Name = "Ferguson's Crossing" });
            wnTable.Worlds.Add(new World { ID = 1021, Name = "Dragonbrand" });
            wnTable.Worlds.Add(new World { ID = 1022, Name = "Kaineng" });
            wnTable.Worlds.Add(new World { ID = 1023, Name = "Devona's Rest" });
            wnTable.Worlds.Add(new World { ID = 1024, Name = "Eredon Terrace" });
            wnTable.Worlds.Add(new World { ID = 2001, Name = "Fissure of Woe" });
            wnTable.Worlds.Add(new World { ID = 2002, Name = "Desolation" });
            wnTable.Worlds.Add(new World { ID = 2003, Name = "Gandara" });
            wnTable.Worlds.Add(new World { ID = 2004, Name = "Blacktide" });
            wnTable.Worlds.Add(new World { ID = 2005, Name = "Ring of Fire" });
            wnTable.Worlds.Add(new World { ID = 2006, Name = "Underworld" });
            wnTable.Worlds.Add(new World { ID = 2007, Name = "Far Shiverpeaks" });
            wnTable.Worlds.Add(new World { ID = 2008, Name = "Whiteside Ridge" });
            wnTable.Worlds.Add(new World { ID = 2009, Name = "Ruins of Surmia" });
            wnTable.Worlds.Add(new World { ID = 2010, Name = "Seafarer's Rest" });
            wnTable.Worlds.Add(new World { ID = 2011, Name = "Vabbi" });
            wnTable.Worlds.Add(new World { ID = 2012, Name = "Piken Square" });
            wnTable.Worlds.Add(new World { ID = 2013, Name = "Aurora Glade" });
            wnTable.Worlds.Add(new World { ID = 2014, Name = "Gunnar's Hold" });
            wnTable.Worlds.Add(new World { ID = 2101, Name = "Jade Sea [FR]" });
            wnTable.Worlds.Add(new World { ID = 2102, Name = "Fort Ranik [FR]" });
            wnTable.Worlds.Add(new World { ID = 2103, Name = "Augury Rock [FR]" });
            wnTable.Worlds.Add(new World { ID = 2104, Name = "Vizunah Square [FR]" });
            wnTable.Worlds.Add(new World { ID = 2105, Name = "Arborstone [FR]" });
            wnTable.Worlds.Add(new World { ID = 2201, Name = "Kodash [DE]" });
            wnTable.Worlds.Add(new World { ID = 2202, Name = "Riverside [DE]" });
            wnTable.Worlds.Add(new World { ID = 2203, Name = "Elona Reach [DE]" });
            wnTable.Worlds.Add(new World { ID = 2204, Name = "Abaddon's Mouth [DE]" });
            wnTable.Worlds.Add(new World { ID = 2205, Name = "Drakkar Lake [DE]" });
            wnTable.Worlds.Add(new World { ID = 2206, Name = "Miller's Sound [DE]" });
            wnTable.Worlds.Add(new World { ID = 2207, Name = "Dzagonur [DE]" });
            wnTable.Worlds.Add(new World { ID = 2301, Name = "Baruch Bay [SP]" });

            XmlSerializer serializer = new XmlSerializer(typeof(WorldsTable));
            TextWriter textWriter = new StreamWriter(FileName);
            serializer.Serialize(textWriter, wnTable);
            textWriter.Close();
        }
    }
}
