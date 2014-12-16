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
    /// The Dungeons Table containing all information about the various dungeons
    /// </summary>
    public class DungeonsTable
    {
        /// <summary>
        /// File name for the table
        /// </summary>
        public static readonly string FileName = "Dungeons.xml";

        /// <summary>
        /// List of dungeons and their details
        /// </summary>
        public List<Dungeon> Dungeons { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DungeonsTable()
        {
            this.Dungeons = new List<Dungeon>();
        }

        /// <summary>
        /// Loads the world events time table file
        /// </summary>
        /// <returns>The loaded event time table data</returns>
        public static DungeonsTable LoadTable()
        {
            DungeonsTable loadedData = null;
            XmlSerializer deserializer = new XmlSerializer(typeof(DungeonsTable));
            TextReader reader = new StreamReader(FileName);
            try
            {
                object obj = deserializer.Deserialize(reader);
                loadedData = (DungeonsTable)obj;
            }
            finally
            {
                reader.Close();
            }

            return loadedData;
        }

        /// <summary>
        /// Loads the world events time table file
        /// </summary>
        /// <returns>The loaded event time table data</returns>
        public static void CreateTable()
        {
            DungeonsTable dTable = new DungeonsTable();
            dTable.Dungeons.Add(new Dungeon()
                {
                    Name = "Ascalonian Catacombs",
                    ID = DungeonID.AscalonianCatacombs,
                    WorldMapID = 19,
                    MinimumLevel = 30,
                    WaypointCode = "[TBD]",
                    WikiUrl = "http://wiki.guildwars2.com/wiki/Ascalonian_Catacombs",
                    Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = AscalonianCatacombsPathID.Story,
                                InstanceMapID = 33,
                                PathDisplayText = "S",
                                GoldReward = 0.5,
                                EndPoint = new Point(99.5, -62.5, 43.5),
                                PointDetectionRadius = 40,
                                CompletionPrereqPoints = new List<Point> { new Point(-16.8, -96.9, 51.5), new Point(330.1, 83.8, 31.9), new Point(341.5, -47.5, 19.8) }
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = AscalonianCatacombsPathID.P1,
                                InstanceMapID = 36,
                                PathDisplayText = "P1",
                                GoldReward = 1.55,
                                EndPoint = new Point(-316, 193, 0),
                                IdentifyingPoints = new List<Point> { new Point(99, -222, 15), new Point(-87, 98, 54) },
                                PointDetectionRadius = 50
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = AscalonianCatacombsPathID.P2,
                                InstanceMapID = 36,
                                PathDisplayText = "P2",
                                GoldReward = 1.55,
                                EndPoint = new Point(106, 148, 67),
                                IdentifyingPoints = new List<Point> { new Point(315, -185, 0) },
                                PointDetectionRadius = 50
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = AscalonianCatacombsPathID.P3,
                                InstanceMapID = 36,
                                PathDisplayText = "P3",
                                GoldReward = 1.55,
                                EndPoint = new Point(-317, 193, 0),
                                IdentifyingPoints = new List<Point> { new Point(328, -60, 19.4) },
                                PointDetectionRadius = 50
                            }
                        }
                });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Caudecus's Manor",
                ID = DungeonID.CaudecusManor,
                WorldMapID = 15,
                MinimumLevel = 40,
                WaypointCode = "[TBD]",
                WikiUrl = "http://wiki.guildwars2.com/wiki/Caudecus%27s_Manor",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = CaudecusManorPathID.Story,
                                InstanceMapID = 75,
                                PathDisplayText = "S",
                                GoldReward = 0.5
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = CaudecusManorPathID.P1,
                                InstanceMapID = 76,
                                PathDisplayText = "P1",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = CaudecusManorPathID.P2,
                                InstanceMapID = 76,
                                PathDisplayText = "P2",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = CaudecusManorPathID.P3,
                                InstanceMapID = 76,
                                PathDisplayText = "P3",
                                GoldReward = 1.05
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Twilight Arbor",
                ID = DungeonID.TwilightArbor,
                WorldMapID = 34,
                MinimumLevel = 50,
                WaypointCode = "[TBD]",
                WikiUrl = "http://wiki.guildwars2.com/wiki/Twilight_Arbor",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = TwilightArborPathID.Story,
                                InstanceMapID = 68,
                                PathDisplayText = "S",
                                GoldReward = 0.5
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = TwilightArborPathID.P1,
                                InstanceMapID = 67,
                                PathDisplayText = "P1",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = TwilightArborPathID.P2,
                                InstanceMapID = 67,
                                PathDisplayText = "P2",
                                GoldReward = 2.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = TwilightArborPathID.P3,
                                InstanceMapID = 67,
                                PathDisplayText = "P3",
                                GoldReward = 1.05
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Sorrow's Embrace",
                ID = DungeonID.SorrowsEmbrace,
                WorldMapID = 26,
                MinimumLevel = 60,
                WaypointCode = "[TBD]",
                WikiUrl = "http://wiki.guildwars2.com/wiki/Sorrow%27s_Embrace",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = SorrowsEmbracePathID.Story,
                                InstanceMapID = 63,
                                PathDisplayText = "S",
                                GoldReward = 0.5
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = SorrowsEmbracePathID.P1,
                                InstanceMapID = 64,
                                PathDisplayText = "P1",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = SorrowsEmbracePathID.P2,
                                InstanceMapID = 64,
                                PathDisplayText = "P2",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = SorrowsEmbracePathID.P3,
                                InstanceMapID = 64,
                                PathDisplayText = "P3",
                                GoldReward = 1.05
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Citadel of Flame",
                ID = DungeonID.CitadelOfFlame,
                WorldMapID = 22,
                MinimumLevel = 70,
                WaypointCode = "[TBD]",
                WikiUrl = "http://wiki.guildwars2.com/wiki/Citadel_of_Flame",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = CitadelOfFlamePathID.Story,
                                InstanceMapID = 66,
                                PathDisplayText = "S",
                                GoldReward = 0.5
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = CitadelOfFlamePathID.P1,
                                InstanceMapID = 69,
                                PathDisplayText = "P1",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = CitadelOfFlamePathID.P2,
                                InstanceMapID = 69,
                                PathDisplayText = "P2",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = CitadelOfFlamePathID.P3,
                                InstanceMapID = 69,
                                PathDisplayText = "P3",
                                GoldReward = 1.05
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Honor of the Waves",
                ID = DungeonID.HonorOfTheWaves,
                WorldMapID = 30,
                MinimumLevel = 76,
                WaypointCode = "[TBD]",
                WikiUrl = "http://wiki.guildwars2.com/wiki/Honor_of_the_Waves",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = HonorOfTheWavesPathID.Story,
                                InstanceMapID = 70,
                                PathDisplayText = "S",
                                GoldReward = 0.5
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = HonorOfTheWavesPathID.P1,
                                InstanceMapID = 71,
                                PathDisplayText = "P1",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = HonorOfTheWavesPathID.P2,
                                InstanceMapID = 71,
                                PathDisplayText = "P2",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = HonorOfTheWavesPathID.P3,
                                InstanceMapID = 71,
                                PathDisplayText = "P3",
                                GoldReward = 1.05
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Crucible of Eternity",
                ID = DungeonID.CrucibleOfEternity,
                WorldMapID = 39,
                MinimumLevel = 78,
                WaypointCode = "[TBD]",
                WikiUrl = "http://wiki.guildwars2.com/wiki/Crucible_of_Eternity",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = CrucibleOfEternityPathID.Story,
                                InstanceMapID = 81,
                                PathDisplayText = "S",
                                GoldReward = 0.5
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = CrucibleOfEternityPathID.P1,
                                InstanceMapID = 82,
                                PathDisplayText = "P1",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = CrucibleOfEternityPathID.P2,
                                InstanceMapID = 82,
                                PathDisplayText = "P2",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = CrucibleOfEternityPathID.P3,
                                InstanceMapID = 82,
                                PathDisplayText = "P3",
                                GoldReward = 1.05
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Ruined City of Arah",
                ID = DungeonID.RuinedCityOfArah,
                WorldMapID = 62,
                MinimumLevel = 80,
                WaypointCode = "[TBD]",
                WikiUrl = "http://wiki.guildwars2.com/wiki/The_Ruined_City_of_Arah",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = RuinedCityOfArahPathID.Story,
                                InstanceMapID = 111,
                                PathDisplayText = "S",
                                GoldReward = 0.5
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = RuinedCityOfArahPathID.P1,
                                InstanceMapID = 112,
                                PathDisplayText = "P1",
                                GoldReward = 3.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = RuinedCityOfArahPathID.P2,
                                InstanceMapID = 112,
                                PathDisplayText = "P2",
                                GoldReward = 3.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = RuinedCityOfArahPathID.P3,
                                InstanceMapID = 112,
                                PathDisplayText = "P3",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 4,
                                ID = RuinedCityOfArahPathID.P4,
                                InstanceMapID = 112,
                                PathDisplayText = "P4",
                                GoldReward = 3.05
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Fractals of the Mists",
                ID = DungeonID.FractalsOfTheMists,
                WorldMapID = 50,
                MinimumLevel = 1,
                WaypointCode = "[TBD]",
                WikiUrl = "http://wiki.guildwars2.com/wiki/Fractals_of_the_Mists",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = FractalsOfTheMistsPathID.Tier0,
                                PathDisplayText = "10",
                                GoldReward = 0
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = FractalsOfTheMistsPathID.Tier1,
                                PathDisplayText = "20",
                                GoldReward = 0
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = FractalsOfTheMistsPathID.Tier2,
                                PathDisplayText = "30",
                                GoldReward = 0
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = FractalsOfTheMistsPathID.Tier3,
                                PathDisplayText = "40",
                                GoldReward = 0
                            },
                            new DungeonPath()
                            {
                                PathNumber = 4,
                                ID = FractalsOfTheMistsPathID.Tier4,
                                PathDisplayText = "50",
                                GoldReward = 0
                            },
                        }
            });

            XmlSerializer serializer = new XmlSerializer(typeof(DungeonsTable));
            TextWriter textWriter = new StreamWriter(FileName);
            try
            {
                serializer.Serialize(textWriter, dTable);
            }
            finally
            {
                textWriter.Close();
            }
        }
    }
}
