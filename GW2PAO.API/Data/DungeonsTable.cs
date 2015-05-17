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
                    WaypointCode = "[&BIYBAAA=]",
                    Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = AscalonianCatacombsPathID.Story,
                                InstanceMapID = 33,
                                PathDisplayText = "S",
                                GuideUrl = "http://gw2dungeons.net/ACS",
                                GoldReward = 0.5,
                                EndPoint = new DetectionPoint(99.5, -62.5, 43.5, 75),
                                EndCutsceneCount = 2
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = AscalonianCatacombsPathID.P1,
                                InstanceMapID = 36,
                                PathDisplayText = "P1",
                                GuideUrl = "http://gw2dungeons.net/AC1",
                                GoldReward = 1.55,
                                IdentifyingPoints = new List<DetectionPoint> { new DetectionPoint(99, -222, 15, 50), new DetectionPoint(-87, 98, 54, 50) },
                                EndPoint = new DetectionPoint(-322.01, 195.11, 0.87, 75),
                                EndCutsceneCount = 1
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = AscalonianCatacombsPathID.P2,
                                InstanceMapID = 36,
                                PathDisplayText = "P2",
                                GuideUrl = "http://gw2dungeons.net/AC2",
                                GoldReward = 1.55,
                                IdentifyingPoints = new List<DetectionPoint> { new DetectionPoint(315, -185, 0, 50) },
                                EndPoint = new DetectionPoint(106, 148, 67, 75),
                                EndCutsceneCount = 1
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = AscalonianCatacombsPathID.P3,
                                InstanceMapID = 36,
                                PathDisplayText = "P3",
                                GuideUrl = "http://gw2dungeons.net/AC3",
                                GoldReward = 1.55,
                                IdentifyingPoints = new List<DetectionPoint> { new DetectionPoint(328, -60, 19.4, 50) },
                                EndPoint = new DetectionPoint(-317, 193, 0, 75),
                                EndCutsceneCount = 1
                            }
                        }
                });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Caudecus's Manor",
                ID = DungeonID.CaudecusManor,
                WorldMapID = 15,
                MinimumLevel = 40,
                WaypointCode = "[&BPoAAAA=]",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = CaudecusManorPathID.Story,
                                InstanceMapID = 75,
                                PathDisplayText = "S",
                                GuideUrl = "http://gw2dungeons.net/CMS",
                                GoldReward = 0.5,
                                EndPoint = new DetectionPoint(252.10, 163.32, 37.48, 75),
                                EndCutsceneCount = 1
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = CaudecusManorPathID.P1,
                                InstanceMapID = 76,
                                PathDisplayText = "P1",
                                GuideUrl = "http://gw2dungeons.net/CM1",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint>
                                {
                                    new DetectionPoint(127.11, 87.91, 22.54, 5.00),
                                    new DetectionPoint(148.50, 88.26, 22.54, 5.00),
                                    new DetectionPoint(151.55, 65.30, 22.54, 5.00),
                                    new DetectionPoint(124.51, 21.21, 22.54, 5.00),
                                    new DetectionPoint(135.41, 40.18, 22.54, 5.00),
                                    new DetectionPoint(105.46, 91.48, 22.54, 5.00),
                                    new DetectionPoint(95.84, 127.75, 22.54, 5.00),
                                    new DetectionPoint(135.57, 102.49, 22.54, 5.00)
                                },
                                EndPoint = new DetectionPoint(-195.62, 193.663, 2.571, 100),
                                EndCutsceneCount = 2
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = CaudecusManorPathID.P2,
                                InstanceMapID = 76,
                                PathDisplayText = "P2",
                                GuideUrl = "http://gw2dungeons.net/CM2",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint> 
                                { 
                                    new DetectionPoint(46.79, 58.08, 40.08, 30)
                                },
                                EndPoint = new DetectionPoint(252.08, 268.50, 41.53, 100),
                                EndCutsceneCount = 1
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = CaudecusManorPathID.P3,
                                InstanceMapID = 76,
                                PathDisplayText = "P3",
                                GuideUrl = "http://gw2dungeons.net/CM3",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint>
                                { 
                                    new DetectionPoint(15.41, 26.50, 16.28, 10.00),
                                    new DetectionPoint(19.70, 47.41, 16.33, 10.00),
                                    new DetectionPoint(5.45, 43.40, 15.59, 10.00)
                                },
                                EndPoint = new DetectionPoint(-215.50, 233.00, 0.49, 100),
                                EndCutsceneCount = 1
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Twilight Arbor",
                ID = DungeonID.TwilightArbor,
                WorldMapID = 34,
                MinimumLevel = 50,
                WaypointCode = "[&BEEFAAA=]",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = TwilightArborPathID.Story,
                                InstanceMapID = 68,
                                PathDisplayText = "S",
                                GuideUrl = "http://gw2dungeons.net/TAS",
                                GoldReward = 0.5,
                                IdentifyingPoints = new List<DetectionPoint>() {},
                                EndPoint = new DetectionPoint(68.70, -0.59, 45.21, 75),
                                EndCutsceneCount = 2
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = TwilightArborPathID.P1,
                                InstanceMapID = 67,
                                PathDisplayText = "F",
                                GuideUrl = "http://gw2dungeons.net/TAF",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint> { new DetectionPoint(-55.25, -137.32, 2.77, 35) },
                                EndPoint = new DetectionPoint(57.02, -0.15, 45.16, 75),
                                EndCutsceneCount = 2
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = TwilightArborPathID.P2,
                                InstanceMapID = 67,
                                PathDisplayText = "U",
                                GuideUrl = "http://gw2dungeons.net/TAU",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint> { new DetectionPoint(42.90, -176.14, 32.26, 35) },
                                EndPoint = new DetectionPoint(65.51, 1.04, 45.51, 75),
                                EndCutsceneCount = 2
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = TwilightArborPathID.P3,
                                InstanceMapID = 67,
                                PathDisplayText = "AE",
                                GuideUrl = "http://gw2dungeons.net/TAAE",
                                GoldReward = 2.05,
                                IdentifyingPoints = new List<DetectionPoint> { new DetectionPoint(-31.42, -262.85, -0.54, 35) },
                                EndPoint = new DetectionPoint(-201.52, 249.99, 20.56, 75),
                                EndCutsceneCount = 1
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Sorrow's Embrace",
                ID = DungeonID.SorrowsEmbrace,
                WorldMapID = 26,
                MinimumLevel = 60,
                WaypointCode = "[&BD8FAAA=]",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = SorrowsEmbracePathID.Story,
                                InstanceMapID = 63,
                                PathDisplayText = "S",
                                GuideUrl = "http://gw2dungeons.net/SES",
                                GoldReward = 0.5,
                                IdentifyingPoints = new List<DetectionPoint>() { },
                                EndPoint = new DetectionPoint(127.33, -278.88, 93.87, 100),
                                EndCutsceneCount = 2
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = SorrowsEmbracePathID.P1,
                                InstanceMapID = 64,
                                PathDisplayText = "P1",
                                GuideUrl = "http://gw2dungeons.net/SE1",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint> { new DetectionPoint(-127.39, -66.08, 162.89, 35) },
                                EndPoint = new DetectionPoint(351.03, -105, 143.36, 100),
                                EndCutsceneCount = 2
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = SorrowsEmbracePathID.P2,
                                InstanceMapID = 64,
                                PathDisplayText = "P2",
                                GuideUrl = "http://gw2dungeons.net/SE2",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint> { new DetectionPoint(-85.47, 157.55, 211.13, 35) },
                                EndPoint = new DetectionPoint(144.48, 41.61, 143.45, 100),  // TODO: Retest this
                                EndCutsceneCount = 1
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = SorrowsEmbracePathID.P3,
                                InstanceMapID = 64,
                                PathDisplayText = "P3",
                                GuideUrl = "http://gw2dungeons.net/SE3",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint>
                                {
                                    new DetectionPoint(-244.83, 19.78, 204.50, 35.00),
                                    new DetectionPoint(-258.47, -135.50, 168.38, 35.00),
                                    new DetectionPoint(-242.73, -197.97, 165.74, 35.00),
                                    new DetectionPoint(-242.24, -252.07, 199.46, 35.00)
                                },
                                EndPoint = new DetectionPoint(-242.24, -252.07, 199.46, 100), // TODO: Retest this
                                EndCutsceneCount = 1
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Citadel of Flame",
                ID = DungeonID.CitadelOfFlame,
                WorldMapID = 22,
                MinimumLevel = 70,
                WaypointCode = "[&BEAFAAA=]",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = CitadelOfFlamePathID.Story,
                                InstanceMapID = 66,
                                PathDisplayText = "S",
                                GuideUrl = "http://gw2dungeons.net/CoFS",
                                GoldReward = 0.5,
                                IdentifyingPoints = new List<DetectionPoint>() { },
                                EndPoint = new DetectionPoint(76.62, 69.63, 146.58, 100),
                                EndCutsceneCount = 3
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = CitadelOfFlamePathID.P1,
                                InstanceMapID = 69,
                                PathDisplayText = "P1",
                                GuideUrl = "http://gw2dungeons.net/CoF1",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint>
                                { 
                                    new DetectionPoint(-227.21, 42.86, 72.88, 35.00),
                                    new DetectionPoint(-292.84, 61.11, 63.66, 35.00),
                                    new DetectionPoint(-260.00, 119.42, 76.65, 35.00),
                                    new DetectionPoint(-124.13, 264.86, 110.43, 35.00),
                                    new DetectionPoint(-21.09, 187.31, 108.72, 35.00)
                                },
                                EndPoint = new DetectionPoint(50.80, 301.16, 121.76, 175.00),
                                EndCutsceneCount = 2
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = CitadelOfFlamePathID.P2,
                                InstanceMapID = 69,
                                PathDisplayText = "P2",
                                GuideUrl = "http://gw2dungeons.net/CoF2",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint> { new DetectionPoint(-9.03, -147.03, 73.00, 35) },
                                EndPoint = new DetectionPoint(328.02, -17.45, 101.33, 100),
                                EndCutsceneCount = 2
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = CitadelOfFlamePathID.P3,
                                InstanceMapID = 69,
                                PathDisplayText = "P3",
                                GuideUrl = "http://gw2dungeons.net/CoF3",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint> { new DetectionPoint(-25.65, -79.68, 8.89, 35) },
                                EndPoint = new DetectionPoint(255.89, 278.00, 128.17, 100),
                                EndCutsceneCount = 2
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Honor of the Waves",
                ID = DungeonID.HonorOfTheWaves,
                WorldMapID = 30,
                MinimumLevel = 76,
                WaypointCode = "[&BEMFAAA=]",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = HonorOfTheWavesPathID.Story,
                                InstanceMapID = 70,
                                PathDisplayText = "S",
                                GuideUrl = "http://gw2dungeons.net/HotWS",
                                GoldReward = 0.5,
                                IdentifyingPoints = new List<DetectionPoint>() { },
                                EndPoint = new DetectionPoint(106.65, 25.35, 5.11, 75),
                                EndCutsceneCount = 1
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = HonorOfTheWavesPathID.P1,
                                InstanceMapID = 71,
                                PathDisplayText = "P1",
                                GuideUrl = "http://gw2dungeons.net/HotW1",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint> { new DetectionPoint(-119.83, 159.85, 38.57, 35) },
                                EndPoint = new DetectionPoint(-67.77, -68.20, 18.95, 75),
                                EndCutsceneCount = 2
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = HonorOfTheWavesPathID.P2,
                                InstanceMapID = 71,
                                PathDisplayText = "P2",
                                GuideUrl = "http://gw2dungeons.net/HotW2",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint> { new DetectionPoint(-10.70, 4.73, 55.70, 35) },
                                EndPoint = new DetectionPoint(-158.60, 141.23, -36.51, 75),
                                EndCutsceneCount = 2
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = HonorOfTheWavesPathID.P3,
                                InstanceMapID = 71,
                                PathDisplayText = "P3",
                                GuideUrl = "http://gw2dungeons.net/HotW3",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint> { new DetectionPoint(-199.69, 197.26, 43.96, 35) },
                                EndPoint = new DetectionPoint(-100.96, 11.86, -18.29, 75),
                                EndCutsceneCount = 2
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Crucible of Eternity",
                ID = DungeonID.CrucibleOfEternity,
                WorldMapID = 39,
                MinimumLevel = 78,
                WaypointCode = "[&BEIFAAA=]",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = CrucibleOfEternityPathID.Story,
                                InstanceMapID = 81,
                                PathDisplayText = "S",
                                GuideUrl = "http://gw2dungeons.net/CoES",
                                GoldReward = 0.5,
                                IdentifyingPoints = new List<DetectionPoint>() { },
                                EndPoint = new DetectionPoint(34.60, 37.36, 115.80, 50),
                                EndCutsceneCount = 3
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = CrucibleOfEternityPathID.P1,
                                InstanceMapID = 82,
                                PathDisplayText = "P1",
                                GuideUrl = "http://gw2dungeons.net/CoE1",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint> { new DetectionPoint(170.98, -94.73, 176.74, 35) },
                                EndPoint = new DetectionPoint(131.17, 126.65, 188.48, 50),
                                EndCutsceneCount = 1
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = CrucibleOfEternityPathID.P2,
                                InstanceMapID = 82,
                                PathDisplayText = "P2",
                                GuideUrl = "http://gw2dungeons.net/CoE2",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint> { new DetectionPoint(21.56, 379.84, 225.42, 35) },
                                EndPoint = new DetectionPoint(-64.27, 356.45, 226.15, 40),
                                EndCutsceneCount = 1
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = CrucibleOfEternityPathID.P3,
                                InstanceMapID = 82,
                                PathDisplayText = "P3",
                                GuideUrl = "http://gw2dungeons.net/CoE3",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint> { new DetectionPoint(-137.85, 280.19, 228.39, 35) },
                                EndPoint = new DetectionPoint(76.29, 1.29, 233.99, 50),
                                EndCutsceneCount = 1
                            }
                        }
            });
            dTable.Dungeons.Add(new Dungeon()
            {
                Name = "Ruined City of Arah",
                ID = DungeonID.RuinedCityOfArah,
                WorldMapID = 62,
                MinimumLevel = 80,
                WaypointCode = "[&BCADAAA=]",
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = RuinedCityOfArahPathID.Story,
                                InstanceMapID = 111,
                                PathDisplayText = "S",
                                GuideUrl = "http://gw2dungeons.net/ArahS",
                                GoldReward = 0.5,
                                IdentifyingPoints = new List<DetectionPoint>() {},
                                EndPoint = new DetectionPoint(232.46, 380.78, 267.93, 100),
                                EndCutsceneCount = 1
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = RuinedCityOfArahPathID.P1,
                                InstanceMapID = 112,
                                PathDisplayText = "P1",
                                GuideUrl = "http://gw2dungeons.net/Arah1",
                                GoldReward = 3.05,
                                IdentifyingPoints = new List<DetectionPoint>
                                {
                                    new DetectionPoint(-403.28, 125.72, 3.90, 35),
                                    new DetectionPoint(-326.06, -433.39, 77.44, 50)
                                },
                                EndPoint = new DetectionPoint(-326.06, -433.39, 77.44, 100),
                                EndCutsceneCount = 1
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = RuinedCityOfArahPathID.P2,
                                InstanceMapID = 112,
                                PathDisplayText = "P2",
                                GuideUrl = "http://gw2dungeons.net/Arah2",
                                GoldReward = 3.05,
                                IdentifyingPoints = new List<DetectionPoint>
                                {
                                    new DetectionPoint(-233.35, 113.28, -0.30, 35.00),
                                    new DetectionPoint(-191.52, 182.03, -0.38, 35.00),
                                    new DetectionPoint(-166.94, 241.10, -0.09, 35.00),
                                    new DetectionPoint(-22.21, 384.75, 16.64, 50)
                                },
                                EndPoint = new DetectionPoint(-22.21, 384.75, 16.64, 100),
                                EndCutsceneCount = 2
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = RuinedCityOfArahPathID.P3,
                                InstanceMapID = 112,
                                PathDisplayText = "P3",
                                GuideUrl = "http://gw2dungeons.net/Arah3",
                                GoldReward = 1.05,
                                IdentifyingPoints = new List<DetectionPoint>
                                {
                                    new DetectionPoint(-400.14, -37.66, -0.18, 35),
                                    new DetectionPoint(-292.70, 347.14, 33.38, 50)
                                },
                                EndPoint = new DetectionPoint(-292.70, 347.14, 33.38, 100),
                                EndCutsceneCount = 2
                            },
                            new DungeonPath()
                            {
                                PathNumber = 4,
                                ID = RuinedCityOfArahPathID.P4,
                                InstanceMapID = 112,
                                PathDisplayText = "P4",
                                GuideUrl = "http://gw2dungeons.net/Arah4",
                                GoldReward = 3.05,
                                IdentifyingPoints = new List<DetectionPoint>
                                {
                                    new DetectionPoint(-209.09, -49.55, 0.45, 35.00),
                                    new DetectionPoint(-169.50, -157.64, -0.15, 35.00),
                                    new DetectionPoint(34.37, -536.42, 1.25, 35)
                                },
                                EndPoint = new DetectionPoint(34.37, -536.42, 1.25, 100),
                                EndCutsceneCount = 2
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
                Paths = new List<DungeonPath>() 
                        { 
                            new DungeonPath()
                            {
                                PathNumber = 0,
                                ID = FractalsOfTheMistsPathID.Tier0,
                                PathDisplayText = "10",
                                GoldReward = 0,
                                GuideUrl = "http://gw2dungeons.net/",
                            },
                            new DungeonPath()
                            {
                                PathNumber = 1,
                                ID = FractalsOfTheMistsPathID.Tier1,
                                PathDisplayText = "20",
                                GuideUrl = "http://gw2dungeons.net/",
                                GoldReward = 0
                            },
                            new DungeonPath()
                            {
                                PathNumber = 2,
                                ID = FractalsOfTheMistsPathID.Tier2,
                                PathDisplayText = "30",
                                GuideUrl = "http://gw2dungeons.net/",
                                GoldReward = 0
                            },
                            new DungeonPath()
                            {
                                PathNumber = 3,
                                ID = FractalsOfTheMistsPathID.Tier3,
                                PathDisplayText = "40",
                                GuideUrl = "http://gw2dungeons.net/",
                                GoldReward = 0
                            },
                            new DungeonPath()
                            {
                                PathNumber = 4,
                                ID = FractalsOfTheMistsPathID.Tier4,
                                PathDisplayText = "50",
                                GuideUrl = "http://gw2dungeons.net/",
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
