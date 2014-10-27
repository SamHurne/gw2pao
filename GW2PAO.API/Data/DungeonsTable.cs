using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
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
                    ID = Guid.NewGuid(),
                    Location = "Plains of Ashford",
                    MinimumLevel = 30,
                    WaypointCode = "[TBD]",
                    WikiUrl = "http://wiki.guildwars2.com/wiki/Ascalonian_Catacombs",
                    Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "S",
                                Nickname = "Story",
                                GoldReward = 0.5
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P1",
                                Nickname = "Hodgins",
                                GoldReward = 1.55
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P2",
                                Nickname = "Detha",
                                GoldReward = 1.55
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P3",
                                Nickname = "Tzark",
                                GoldReward = 1.55
                            }
                        }
                });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Caudecus's Manor",
                ID = Guid.NewGuid(),
                Location = "Queensdale",
                MinimumLevel = 40,
                WaypointCode = "[TBD]",
                WikiUrl = "http://wiki.guildwars2.com/wiki/Caudecus%27s_Manor",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "S",
                                Nickname = "Story",
                                GoldReward = 0.5
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P1",
                                Nickname = "Asura",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P2",
                                Nickname = "Seraph",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P3",
                                Nickname = "Butler",
                                GoldReward = 1.05
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Twilight Arbor",
                ID = Guid.NewGuid(),
                Location = "Caledon Forest",
                MinimumLevel = 50,
                WaypointCode = "[TBD]",
                WikiUrl = "http://wiki.guildwars2.com/wiki/Twilight_Arbor",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "S",
                                Nickname = "Story",
                                GoldReward = 0.5
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P1",
                                Nickname = "Leurent (Up)",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P2",
                                Nickname = "Aetherpath",
                                GoldReward = 2.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P3",
                                Nickname = "Vevina (Forward)",
                                GoldReward = 1.05
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Sorrow's Embrace",
                ID = Guid.NewGuid(),
                Location = "Dredgehaunt Cliffs",
                MinimumLevel = 60,
                WaypointCode = "[TBD]",
                WikiUrl = "http://wiki.guildwars2.com/wiki/Sorrow%27s_Embrace",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "S",
                                Nickname = "Story",
                                GoldReward = 0.5
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P1",
                                Nickname = "Fergg",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P2",
                                Nickname = "Rasolov",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P3",
                                Nickname = "Koptev",
                                GoldReward = 1.05
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Citadel of Flame",
                ID = Guid.NewGuid(),
                Location = "Fireheart Rise",
                MinimumLevel = 70,
                WaypointCode = "[TBD]",
                WikiUrl = "http://wiki.guildwars2.com/wiki/Citadel_of_Flame",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "S",
                                Nickname = "Story",
                                GoldReward = 0.5
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P1",
                                Nickname = "Ferrah",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P2",
                                Nickname = "Magg",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P3",
                                Nickname = "Rhiannon",
                                GoldReward = 1.05
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Honor of the Waves",
                ID = Guid.NewGuid(),
                Location = "Frostgorge Sound",
                MinimumLevel = 76,
                WaypointCode = "[TBD]",
                WikiUrl = "http://wiki.guildwars2.com/wiki/Honor_of_the_Waves",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "S",
                                Nickname = "Story",
                                GoldReward = 0.5
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P1",
                                Nickname = "Butcher",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P2",
                                Nickname = "Plunderer",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P3",
                                Nickname = "Zealot",
                                GoldReward = 1.05
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Crucible of Eternity",
                ID = Guid.NewGuid(),
                Location = "Mount Maelstrom",
                MinimumLevel = 78,
                WaypointCode = "[TBD]",
                WikiUrl = "http://wiki.guildwars2.com/wiki/Crucible_of_Eternity",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "S",
                                Nickname = "Story",
                                GoldReward = 0.5
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P1",
                                Nickname = "Submarine",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P2",
                                Nickname = "Teleporter",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P3",
                                Nickname = "Front door",
                                GoldReward = 1.05
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Ruined City of Arah",
                ID = Guid.NewGuid(),
                Location = "Cursed Shore",
                MinimumLevel = 80,
                WaypointCode = "[TBD]",
                WikiUrl = "http://wiki.guildwars2.com/wiki/The_Ruined_City_of_Arah",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "S",
                                Nickname = "Story",
                                GoldReward = 0.5
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P1",
                                Nickname = "Jotun",
                                GoldReward = 3.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P2",
                                Nickname = "Mursaat",
                                GoldReward = 3.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P3",
                                Nickname = "Forgotten",
                                GoldReward = 1.05
                            },
                            new DungeonPath()
                            {
                                PathNumber = 4,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "P4",
                                Nickname = "Seer",
                                GoldReward = 3.05
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Fractals of the Mists",
                ID = Guid.NewGuid(),
                Location = "Lion's Arch",
                MinimumLevel = 1,
                WaypointCode = "[TBD]",
                WikiUrl = "http://wiki.guildwars2.com/wiki/Fractals_of_the_Mists",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "10",
                                Nickname = "Tier 0 (1-10)",
                                GoldReward = 0
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "20",
                                Nickname = "Tier 1 (11-20)",
                                GoldReward = 0
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "30",
                                Nickname = "Tier 2 (21-30)",
                                GoldReward = 0
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "40",
                                Nickname = "Tier 3 (31-40)",
                                GoldReward = 0
                            },
                            new DungeonPath()
                            {
                                PathNumber = 4,
                                ID = Guid.NewGuid(),
                                PathDisplayText = "50",
                                Nickname = "Tier 4 (41-50)",
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
