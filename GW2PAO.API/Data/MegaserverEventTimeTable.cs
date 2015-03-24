using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.API.Constants;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;

namespace GW2PAO.API.Data
{
    /// <summary>
    /// The Megaserver event time table
    /// </summary>
    public class MegaserverEventTimeTable
    {
        /// <summary>
        /// File name for the standard time table
        /// </summary>
        public static readonly string StandardFilename = "StandardEventTimeTable.xml";

        /// <summary>
        /// File name for the adjusted time table
        /// </summary>
        public static readonly string AdjustedFilename = "AdjustedEventTimeTable.xml";

        /// <summary>
        /// List of world events and their details
        /// </summary>
        public List<WorldEvent> WorldEvents { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MegaserverEventTimeTable()
        {
            this.WorldEvents = new List<WorldEvent>();
        }

        /// <summary>
        /// Loads the world events time table file
        /// </summary>
        /// <returns>The loaded event time table data</returns>
        public static MegaserverEventTimeTable LoadTable(bool adjustedTimes)
        {
            string filename;
            if (adjustedTimes)
                filename = AdjustedFilename;
            else
                filename = StandardFilename;

            XmlSerializer deserializer = new XmlSerializer(typeof(MegaserverEventTimeTable));
            TextReader reader = new StreamReader(filename);
            MegaserverEventTimeTable loadedData = null;
            try
            {
                object obj = deserializer.Deserialize(reader);
                loadedData = (MegaserverEventTimeTable)obj;
            }
            finally
            {
                reader.Close();
            }

            return loadedData;
        }

        /// <summary>
        /// Creates the world events time table file
        /// </summary>
        /// <returns></returns>
        public static void CreateTable(bool adjustedTimes)
        {
            string filename;

            MegaserverEventTimeTable tt = new MegaserverEventTimeTable();
            tt.WorldEvents = new List<WorldEvent>();

            var megadestroyer = new WorldEvent()
                {
                    Name = "Megadestroyer",
                    ID = WorldEventID.Megadestroyer,
                    MapID = 39,
                    WaypointCode = "[&BM0CAAA=]",
                    CompletionLocations = new List<Point>() { new Point(-317.3, -271.8, 15.4185) },
                    CompletionRadius = 50
                };
            var tequatl = new WorldEvent()
                {
                    Name = "Tequatl",
                    ID = WorldEventID.Tequatl,
                    MapID = 53,
                    WaypointCode = "[&BNABAAA=]",
                    CompletionLocations = new List<Point>() { new Point(-562.4, -907.4, 0.3) },
                    CompletionRadius = 50
                };
            var karkaQueen = new WorldEvent()
                {
                    Name = "Karka Queen",
                    ID = WorldEventID.KarkaQueen,
                    MapID = 873,
                    WaypointCode = "[&BNcGAAA=]",
                    CompletionLocations = new List<Point>() { new Point(605.74, 313.42, 2.24), new Point(490.17, 121.94, 28.21), new Point(92.86, -5.77, 43.5), new Point(20.34, -311.43, 21.48) },
                    CompletionRadius = 75
                };
            var evolvedJungleWurm = new WorldEvent()
                {
                    Name = "Evolved Jungle Wurm",
                    ID = WorldEventID.EvolvedJungleWurm,
                    MapID = 73,
                    WaypointCode = "[&BKoBAAA=]",
                    CompletionLocations = new List<Point>() { new Point(217.04, -430.25, 16.22), new Point(-263.30, -861.60, 0.31), new Point(657.93, -599.51, 0.5) },
                    CompletionRadius = 75
                };
            var shatterer = new WorldEvent()
                {
                    Name = "Shatterer",
                    ID = WorldEventID.Shatterer,
                    MapID = 20,
                    WaypointCode = "[&BE4DAAA=]",
                    CompletionLocations = new List<Point>() { new Point(-269.9, 928.2, 3.2) },
                    CompletionRadius = 50
                };
            var clawOfJormag = new WorldEvent()
                {
                    Name = "Claw of Jormag",
                    ID = WorldEventID.ClawOfJormag,
                    MapID = 30,
                    WaypointCode = "[&BHoCAAA=]",
                    CompletionLocations = new List<Point>() { new Point(449.9, 468.4, 23.2) },
                    CompletionRadius = 75
                };
            var modniirUlgoth = new WorldEvent()
                {
                    Name = "Modniir Ulgoth",
                    ID = WorldEventID.ModniirUlgoth,
                    MapID = 17,
                    WaypointCode = "[&BLEAAAA=]",
                    CompletionLocations = new List<Point>() { new Point(803.02, 663.3, 143.8) },
                    CompletionRadius = 50
                };
            var inquestGolemMarkII = new WorldEvent()
                {
                    Name = "Inquest Golem Mark II",
                    ID = WorldEventID.InquestGolemMarkII,
                    MapID = 39,
                    WaypointCode = "[&BNQCAAA=]",
                    CompletionLocations = new List<Point>() { new Point(926.17, 51.20, 10.72) },
                    CompletionRadius = 50
                };
            var taidhaCovington = new WorldEvent()
                {
                    Name = "Taidha Covington",
                    ID = WorldEventID.TaidhaCovington,
                    MapID = 73,
                    WaypointCode = "[&BKgBAAA=]",
                    CompletionLocations = new List<Point>() { new Point(-178.4, 229.5, 18.5) },
                    CompletionRadius = 50
                };
            var jungleWurm = new WorldEvent()
                {
                    Name = "Jungle Wurm",
                    ID = WorldEventID.JungleWurm,
                    MapID = 34,
                    WaypointCode = "[&BEEFAAA=]",
                    CompletionLocations = new List<Point>() { new Point(-401.6, 841.7, 0) },
                    CompletionRadius = 50
                };
            var shadowBehemoth = new WorldEvent()
                {
                    Name = "Shadow Behemoth",
                    ID = WorldEventID.ShadowBehemoth,
                    MapID = 15,
                    WaypointCode = "[&BPwAAAA=]",
                    CompletionLocations = new List<Point>() { new Point(261.9, -432.2, -0.75) },
                    CompletionRadius = 50
                };
            var fireElemental = new WorldEvent()
                {
                    Name = "Fire Elemental",
                    ID = WorldEventID.FireElemental,
                    MapID = 35,
                    WaypointCode = "[&BEYAAAA=]",
                    CompletionLocations = new List<Point>() { new Point(-371.9, 885.8, 21.63) },
                    CompletionRadius = 50
                };
            var frozenMaw = new WorldEvent()
                {
                    Name = "Frozen Maw",
                    ID = WorldEventID.FrozenMaw,
                    MapID = 28,
                    WaypointCode = "[&BH4BAAA=]",
                    CompletionLocations = new List<Point>() { new Point(390.7, 345.2, 71.9) },
                    CompletionRadius = 50
                };

            if (adjustedTimes)
            {
                megadestroyer.ActiveTimes = new List<EventTimespan>() { new EventTimespan(0, 35, 0), new EventTimespan(3, 35, 0), new EventTimespan(6, 35, 0), new EventTimespan(9, 35, 0), new EventTimespan(12, 35, 0), new EventTimespan(15, 35, 0), new EventTimespan(18, 35, 0), new EventTimespan(21, 35, 0) };
                megadestroyer.Duration = new EventTimespan(0, 10, 0);
                megadestroyer.WarmupDuration = new EventTimespan(0, 7, 0);

                tequatl.ActiveTimes = new List<EventTimespan>() { new EventTimespan(0, 0, 0), new EventTimespan(3, 0, 0), new EventTimespan(7, 0, 0), new EventTimespan(11, 30, 0), new EventTimespan(16, 0, 0), new EventTimespan(19, 0, 0) };
                tequatl.Duration = new EventTimespan(0, 15, 0);
                tequatl.WarmupDuration = new EventTimespan(0, 0, 0);

                karkaQueen.ActiveTimes = new List<EventTimespan>() { new EventTimespan(2, 3, 0), new EventTimespan(6, 3, 0), new EventTimespan(10, 33, 0), new EventTimespan(15, 3, 0), new EventTimespan(18, 3, 0), new EventTimespan(23, 3, 0) };
                karkaQueen.Duration = new EventTimespan(0, 5, 0);
                karkaQueen.WarmupDuration = new EventTimespan(0, 5, 0);

                evolvedJungleWurm.ActiveTimes = new List<EventTimespan>() { new EventTimespan(1, 0, 0), new EventTimespan(4, 0, 0), new EventTimespan(8, 0, 0), new EventTimespan(12, 30, 0), new EventTimespan(17, 0, 0), new EventTimespan(20, 0, 0) };
                evolvedJungleWurm.Duration = new EventTimespan(0, 15, 0);
                evolvedJungleWurm.WarmupDuration = new EventTimespan(0, 10, 0);

                shatterer.ActiveTimes = new List<EventTimespan>() { new EventTimespan(1, 8, 0), new EventTimespan(4, 8, 0), new EventTimespan(7, 8, 0), new EventTimespan(10, 8, 0), new EventTimespan(13, 8, 0), new EventTimespan(16, 8, 0), new EventTimespan(19, 8, 0), new EventTimespan(22, 8, 0) };
                shatterer.Duration = new EventTimespan(0, 7, 0);
                shatterer.WarmupDuration = new EventTimespan(0, 6, 0);

                clawOfJormag.ActiveTimes = new List<EventTimespan>() { new EventTimespan(2, 33, 0), new EventTimespan(5, 33, 0), new EventTimespan(8, 33, 0), new EventTimespan(11, 33, 0), new EventTimespan(14, 33, 0), new EventTimespan(17, 33, 0), new EventTimespan(20, 33, 0), new EventTimespan(23, 33, 0) };
                clawOfJormag.Duration = new EventTimespan(0, 17, 0);
                clawOfJormag.WarmupDuration = new EventTimespan(0, 3, 0);

                modniirUlgoth.ActiveTimes = new List<EventTimespan>() { new EventTimespan(1, 30, 0), new EventTimespan(4, 30, 0), new EventTimespan(7, 30, 0), new EventTimespan(10, 30, 0), new EventTimespan(13, 30, 0), new EventTimespan(16, 30, 0), new EventTimespan(19, 30, 0), new EventTimespan(22, 30, 0) };
                modniirUlgoth.Duration = new EventTimespan(0, 4, 0);
                modniirUlgoth.WarmupDuration = new EventTimespan(0, 15, 0);

                inquestGolemMarkII.ActiveTimes = new List<EventTimespan>() { new EventTimespan(2, 4, 0), new EventTimespan(5, 4, 0), new EventTimespan(8, 4, 0), new EventTimespan(11, 4, 0), new EventTimespan(14, 4, 0), new EventTimespan(17, 4, 0), new EventTimespan(20, 4, 0), new EventTimespan(23, 4, 0) };
                inquestGolemMarkII.Duration = new EventTimespan(0, 6, 0);
                inquestGolemMarkII.WarmupDuration = new EventTimespan(0, 2, 0);

                taidhaCovington.ActiveTimes = new List<EventTimespan>() { new EventTimespan(0, 10, 0), new EventTimespan(3, 10, 0), new EventTimespan(6, 10, 0), new EventTimespan(9, 10, 0), new EventTimespan(12, 10, 0), new EventTimespan(15, 10, 0), new EventTimespan(18, 10, 0), new EventTimespan(21, 10, 0) };
                taidhaCovington.Duration = new EventTimespan(0, 10, 0);
                taidhaCovington.WarmupDuration = new EventTimespan(0, 10, 0);

                jungleWurm.ActiveTimes = new List<EventTimespan>() { new EventTimespan(1, 15, 0), new EventTimespan(3, 15, 0), new EventTimespan(5, 15, 0), new EventTimespan(7, 15, 0), new EventTimespan(9, 15, 0), new EventTimespan(11, 15, 0), new EventTimespan(13, 15, 0), new EventTimespan(15, 15, 0), new EventTimespan(17, 15, 0), new EventTimespan(19, 15, 0), new EventTimespan(21, 15, 0), new EventTimespan(23, 15, 0) };
                jungleWurm.Duration = new EventTimespan(0, 5, 0);
                jungleWurm.WarmupDuration = new EventTimespan(0, 2, 0);

                shadowBehemoth.ActiveTimes = new List<EventTimespan>() { new EventTimespan(1, 46, 0), new EventTimespan(3, 46, 0), new EventTimespan(5, 46, 0), new EventTimespan(7, 46, 0), new EventTimespan(9, 46, 0), new EventTimespan(11, 46, 0), new EventTimespan(13, 46, 0), new EventTimespan(15, 46, 0), new EventTimespan(17, 46, 0), new EventTimespan(19, 46, 0), new EventTimespan(21, 46, 0), new EventTimespan(23, 46, 0) };
                shadowBehemoth.Duration = new EventTimespan(0, 9, 0);
                shadowBehemoth.WarmupDuration = new EventTimespan(0, 1, 0);

                fireElemental.ActiveTimes = new List<EventTimespan>() { new EventTimespan(0, 57, 0), new EventTimespan(2, 57, 0), new EventTimespan(4, 57, 0), new EventTimespan(6, 57, 0), new EventTimespan(8, 57, 0), new EventTimespan(10, 57, 0), new EventTimespan(12, 57, 0), new EventTimespan(14, 57, 0), new EventTimespan(16, 57, 0), new EventTimespan(18, 57, 0), new EventTimespan(20, 57, 0), new EventTimespan(22, 57, 0) };
                fireElemental.Duration = new EventTimespan(0, 3, 0);
                fireElemental.WarmupDuration = new EventTimespan(0, 12, 0);

                frozenMaw.ActiveTimes = new List<EventTimespan>() { new EventTimespan(0, 17, 0), new EventTimespan(2, 17, 0), new EventTimespan(4, 17, 0), new EventTimespan(6, 17, 0), new EventTimespan(8, 17, 0), new EventTimespan(10, 17, 0), new EventTimespan(12, 17, 0), new EventTimespan(14, 17, 0), new EventTimespan(16, 17, 0), new EventTimespan(18, 17, 0), new EventTimespan(20, 17, 0), new EventTimespan(22, 17, 0) };
                frozenMaw.Duration = new EventTimespan(0, 3, 30);
                frozenMaw.WarmupDuration = new EventTimespan(0, 2, 0);

                filename = AdjustedFilename;
            }
            else
            {
                megadestroyer.ActiveTimes = new List<EventTimespan>() { new EventTimespan(0, 30, 0), new EventTimespan(3, 30, 0), new EventTimespan(6, 30, 0), new EventTimespan(9, 30, 0), new EventTimespan(12, 30, 0), new EventTimespan(15, 30, 0), new EventTimespan(18, 30, 0), new EventTimespan(21, 30, 0) };
                megadestroyer.Duration = new EventTimespan(0, 12, 0);
                megadestroyer.WarmupDuration = new EventTimespan(0, 0, 0);

                tequatl.ActiveTimes = new List<EventTimespan>() { new EventTimespan(0, 0, 0), new EventTimespan(3, 0, 0), new EventTimespan(7, 0, 0), new EventTimespan(11, 30, 0), new EventTimespan(16, 0, 0), new EventTimespan(19, 0, 0) };
                tequatl.Duration = new EventTimespan(0, 15, 0);
                tequatl.WarmupDuration = new EventTimespan(0, 0, 0);

                karkaQueen.ActiveTimes = new List<EventTimespan>() { new EventTimespan(2, 0, 0), new EventTimespan(6, 0, 0), new EventTimespan(10, 30, 0), new EventTimespan(15, 0, 0), new EventTimespan(18, 0, 0), new EventTimespan(23, 0, 0) };
                karkaQueen.Duration = new EventTimespan(0, 5, 0);
                karkaQueen.WarmupDuration = new EventTimespan(0, 0, 0);

                evolvedJungleWurm.ActiveTimes = new List<EventTimespan>() { new EventTimespan(1, 0, 0), new EventTimespan(4, 0, 0), new EventTimespan(8, 0, 0), new EventTimespan(12, 30, 0), new EventTimespan(17, 0, 0), new EventTimespan(20, 0, 0) };
                evolvedJungleWurm.Duration = new EventTimespan(0, 15, 0);
                evolvedJungleWurm.WarmupDuration = new EventTimespan(0, 0, 0);

                shatterer.ActiveTimes = new List<EventTimespan>() { new EventTimespan(1, 0, 0), new EventTimespan(4, 0, 0), new EventTimespan(7, 0, 0), new EventTimespan(10, 0, 0), new EventTimespan(13, 0, 0), new EventTimespan(16, 0, 0), new EventTimespan(19, 0, 0), new EventTimespan(22, 0, 0) };
                shatterer.Duration = new EventTimespan(0, 7, 0);
                shatterer.WarmupDuration = new EventTimespan(0, 0, 0);

                clawOfJormag.ActiveTimes = new List<EventTimespan>() { new EventTimespan(2, 30, 0), new EventTimespan(5, 30, 0), new EventTimespan(8, 30, 0), new EventTimespan(11, 30, 0), new EventTimespan(14, 30, 0), new EventTimespan(17, 30, 0), new EventTimespan(20, 30, 0), new EventTimespan(23, 30, 0) };
                clawOfJormag.Duration = new EventTimespan(0, 17, 0);
                clawOfJormag.WarmupDuration = new EventTimespan(0, 0, 0);

                modniirUlgoth.ActiveTimes = new List<EventTimespan>() { new EventTimespan(1, 30, 0), new EventTimespan(4, 30, 0), new EventTimespan(7, 30, 0), new EventTimespan(10, 30, 0), new EventTimespan(13, 30, 0), new EventTimespan(16, 30, 0), new EventTimespan(19, 30, 0), new EventTimespan(22, 30, 0) };
                modniirUlgoth.Duration = new EventTimespan(0, 8, 0);
                modniirUlgoth.WarmupDuration = new EventTimespan(0, 0, 0);

                inquestGolemMarkII.ActiveTimes = new List<EventTimespan>() { new EventTimespan(2, 0, 0), new EventTimespan(5, 0, 0), new EventTimespan(8, 0, 0), new EventTimespan(11, 0, 0), new EventTimespan(14, 0, 0), new EventTimespan(17, 0, 0), new EventTimespan(20, 0, 0), new EventTimespan(23, 0, 0) };
                inquestGolemMarkII.Duration = new EventTimespan(0, 7, 0);
                inquestGolemMarkII.WarmupDuration = new EventTimespan(0, 0, 0);

                taidhaCovington.ActiveTimes = new List<EventTimespan>() { new EventTimespan(0, 0, 0), new EventTimespan(3, 0, 0), new EventTimespan(6, 0, 0), new EventTimespan(9, 0, 0), new EventTimespan(12, 0, 0), new EventTimespan(15, 0, 0), new EventTimespan(18, 0, 0), new EventTimespan(21, 0, 0) };
                taidhaCovington.Duration = new EventTimespan(0, 10, 0);
                taidhaCovington.WarmupDuration = new EventTimespan(0, 0, 0);

                jungleWurm.ActiveTimes = new List<EventTimespan>() { new EventTimespan(1, 15, 0), new EventTimespan(3, 15, 0), new EventTimespan(5, 15, 0), new EventTimespan(7, 15, 0), new EventTimespan(9, 15, 0), new EventTimespan(11, 15, 0), new EventTimespan(13, 15, 0), new EventTimespan(15, 15, 0), new EventTimespan(17, 15, 0), new EventTimespan(19, 15, 0), new EventTimespan(21, 15, 0), new EventTimespan(23, 15, 0) };
                jungleWurm.Duration = new EventTimespan(0, 8, 0);
                jungleWurm.WarmupDuration = new EventTimespan(0, 0, 0);

                shadowBehemoth.ActiveTimes = new List<EventTimespan>() { new EventTimespan(1, 45, 0), new EventTimespan(3, 45, 0), new EventTimespan(5, 45, 0), new EventTimespan(7, 45, 0), new EventTimespan(9, 45, 0), new EventTimespan(11, 45, 0), new EventTimespan(13, 45, 0), new EventTimespan(15, 45, 0), new EventTimespan(17, 45, 0), new EventTimespan(19, 45, 0), new EventTimespan(21, 45, 0), new EventTimespan(23, 45, 0) };
                shadowBehemoth.Duration = new EventTimespan(0, 10, 0);
                shadowBehemoth.WarmupDuration = new EventTimespan(0, 0, 0);

                fireElemental.ActiveTimes = new List<EventTimespan>() { new EventTimespan(0, 45, 0), new EventTimespan(2, 45, 0), new EventTimespan(4, 45, 0), new EventTimespan(6, 45, 0), new EventTimespan(8, 45, 0), new EventTimespan(10, 45, 0), new EventTimespan(12, 45, 0), new EventTimespan(14, 45, 0), new EventTimespan(16, 45, 0), new EventTimespan(18, 45, 0), new EventTimespan(20, 45, 0), new EventTimespan(22, 45, 0) };
                fireElemental.Duration = new EventTimespan(0, 8, 0);
                fireElemental.WarmupDuration = new EventTimespan(0, 0, 0);

                frozenMaw.ActiveTimes = new List<EventTimespan>() { new EventTimespan(0, 15, 0), new EventTimespan(2, 15, 0), new EventTimespan(4, 15, 0), new EventTimespan(6, 15, 0), new EventTimespan(8, 15, 0), new EventTimespan(10, 15, 0), new EventTimespan(12, 15, 0), new EventTimespan(14, 15, 0), new EventTimespan(16, 15, 0), new EventTimespan(18, 15, 0), new EventTimespan(20, 15, 0), new EventTimespan(22, 15, 0) };
                frozenMaw.Duration = new EventTimespan(0, 5, 0);
                frozenMaw.WarmupDuration = new EventTimespan(0, 0, 0);

                filename = StandardFilename;
            }

            tt.WorldEvents.Add(megadestroyer);
            tt.WorldEvents.Add(tequatl);
            tt.WorldEvents.Add(karkaQueen);
            tt.WorldEvents.Add(evolvedJungleWurm);
            tt.WorldEvents.Add(shatterer);
            tt.WorldEvents.Add(clawOfJormag);
            tt.WorldEvents.Add(modniirUlgoth);
            tt.WorldEvents.Add(inquestGolemMarkII);
            tt.WorldEvents.Add(taidhaCovington);
            tt.WorldEvents.Add(jungleWurm);
            tt.WorldEvents.Add(shadowBehemoth);
            tt.WorldEvents.Add(fireElemental);
            tt.WorldEvents.Add(frozenMaw);

            XmlSerializer serializer = new XmlSerializer(typeof(MegaserverEventTimeTable));
            TextWriter textWriter = new StreamWriter(filename);
            serializer.Serialize(textWriter, tt);
            textWriter.Close();
        }
    }
}
