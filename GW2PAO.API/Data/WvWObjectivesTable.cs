using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.API.Data
{
    /// <summary>
    /// The WvW Objectives Table containing all information about the various objectives in WvW
    /// </summary>
    public class WvWObjectivesTable
    {
        /// <summary>
        /// File name for the table
        /// </summary>
        public static readonly string FileName = "WvWObjectives.xml";

        /// <summary>
        /// List of dungeons and their details
        /// </summary>
        public List<Objective> Objectives { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public WvWObjectivesTable()
        {
            this.Objectives = new List<Objective>();
        }

        /// <summary>
        /// Loads the WvW objectives table file
        /// </summary>
        /// <returns>The loaded event time table data</returns>
        public static WvWObjectivesTable LoadTable()
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(WvWObjectivesTable));
            TextReader reader = new StreamReader(FileName);
            object obj = deserializer.Deserialize(reader);
            WvWObjectivesTable loadedData = (WvWObjectivesTable)obj;
            reader.Close();

            return loadedData;
        }

        /// <summary>
        /// Loads the WvW objectives table file
        /// </summary>
        /// <returns>The loaded event time table data</returns>
        public static void CreateTable()
        {
            WvWObjectivesTable table = new WvWObjectivesTable();
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Keep_Overlook, ChatCode = @"[&BOsDAAA=]", Type = ObjectiveType.Keep, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(4548, 17078.4) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Keep_Valley, ChatCode = @"[&BOIDAAA=]", Type = ObjectiveType.Keep, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(25620, -20251.2) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Keep_Lowlands, ChatCode = @"[&BPMDAAA=]", Type = ObjectiveType.Keep, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(-19802.4, -17690.4) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Camp_Golanta, ChatCode = @"[&BPUDAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(-7497.6, -25468.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Camp_Pangloss, ChatCode = @"[&BPADAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(18393.6, 13939.2) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Camp_Speldan, ChatCode = @"[&BO0DAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(-16848.72, 19048.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Camp_Danelon, ChatCode = @"[&BOcDAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(13310.4, -28867.2) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Camp_Umberglade, ChatCode = @"[&BOQDAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(25802.4, -2716.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Castle_Stonemist, ChatCode = @"[&BN4DAAA=]", Type = ObjectiveType.Castle, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(2690.4, -5174.4) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Camp_Rogues, ChatCode = @"[&BPgDAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(-22010.16, -2349.6) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Tower_Aldons, ChatCode = @"[&BPcDAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(-26110.56, -12943.2) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Tower_Wildcreek, ChatCode = @"[&BPkDAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(-12559.92, -5227.2) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Tower_Jerrifers, ChatCode = @"[&BPYDAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(15682.08, -25161.6) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Tower_Klovan, ChatCode = @"[&BPQDAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(-5013.6, -17596.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Tower_Langor, ChatCode = @"[&BOUDAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(21544.8, -27280.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Tower_Quentin, ChatCode = @"[&BOgDAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(10137.6, -21256.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Tower_Mendons, ChatCode = @"[&BO8DAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(-5599.2, 22768.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Tower_Anzalias, ChatCode = @"[&BOwDAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(-6684, 7200) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Tower_Ogrewatch, ChatCode = @"[&BPEDAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(12072, 10622.4) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Tower_Veloka, ChatCode = @"[&BO4DAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(13173.6, 2058) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Tower_Durios, ChatCode = @"[&BOMDAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(17412, -5109.6) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.EB_Tower_Bravost, ChatCode = @"[&BOYDAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.EternalBattlegrounds, MapLocation = new Point(29232, -11200.8) });

            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Keep_Garrison, ChatCode = @"[&BCQFAAA=]", Type = ObjectiveType.Keep, Map = WvWMap.BlueBorderlands, MapLocation = new Point(-441.6, 9160.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Camp_Orchard, ChatCode = @"[&BCUFAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.BlueBorderlands, MapLocation = new Point(-648, -32762.4) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Tower_Redbriar, ChatCode = @"[&BCoFAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.BlueBorderlands, MapLocation = new Point(-7545.6, -17548.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Tower_Greenlake, ChatCode = @"[&BCYFAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.BlueBorderlands, MapLocation = new Point(11155.2, -16648.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Keep_Bay, ChatCode = @"[&BCgFAAA=]", Type = ObjectiveType.Keep, Map = WvWMap.BlueBorderlands, MapLocation = new Point(-22857.6, -6789.6) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Tower_Dawns, ChatCode = @"[&BC0FAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.BlueBorderlands, MapLocation = new Point(12969.6, 12840) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Camp_Spiritholme, ChatCode = @"[&BB8FAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.BlueBorderlands, MapLocation = new Point(232.8, 34180.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Tower_Woodhaven, ChatCode = @"[&BCwFAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.BlueBorderlands, MapLocation = new Point(-14152.8, 14712) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Keep_Hills, ChatCode = @"[&BCAFAAA=]", Type = ObjectiveType.Keep, Map = WvWMap.BlueBorderlands, MapLocation = new Point(22420.8, -5016) });

            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Keep_Hills, ChatCode = @"[&BA8FAAA=]", Type = ObjectiveType.Keep, Map = WvWMap.RedBorderlands, MapLocation = new Point(22420.8, -5016) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Keep_Bay, ChatCode = @"[&BBMFAAA=]", Type = ObjectiveType.Keep, Map = WvWMap.RedBorderlands, MapLocation = new Point(-22857.36, -6789.6) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Camp_Orchard, ChatCode = @"[&BBgFAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.RedBorderlands, MapLocation = new Point(-648, -32762.4) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Tower_Greenbriar, ChatCode = @"[&BB4FAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.RedBorderlands, MapLocation = new Point(-7545.6, -17548.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Tower_Bluelake, ChatCode = @"[&BBkFAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.RedBorderlands, MapLocation = new Point(11155.2, -16648.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Keep_Garrison, ChatCode = @"[&BBYFAAA=]", Type = ObjectiveType.Keep, Map = WvWMap.RedBorderlands, MapLocation = new Point(-441.6, 9160.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Tower_Longview, ChatCode = @"[&BBoFAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.RedBorderlands, MapLocation = new Point(-14153.28, 14712) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Camp_Godsword, ChatCode = @"[&BBIFAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.RedBorderlands, MapLocation = new Point(232.8, 34180.08) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Tower_Cliffside, ChatCode = @"[&BBsFAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.RedBorderlands, MapLocation = new Point(12969.6, 12840) });

            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Keep_Hills, ChatCode = @"[&BDAFAAA=]", Type = ObjectiveType.Keep, Map = WvWMap.GreenBorderlands, MapLocation = new Point(22421.04, -5016) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Tower_Redlake, ChatCode = @"[&BD4FAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.GreenBorderlands, MapLocation = new Point(11154.72, -16648.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Camp_Orchard, ChatCode = @"[&BDkFAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.GreenBorderlands, MapLocation = new Point(-648, -32762.4) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Keep_Bay, ChatCode = @"[&BC8FAAA=]", Type = ObjectiveType.Keep, Map = WvWMap.GreenBorderlands, MapLocation = new Point(-22857.36, -6789.6) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Tower_Bluebriar, ChatCode = @"[&BD0FAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.GreenBorderlands, MapLocation = new Point(-7544.64, -17548.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Keep_Garrison, ChatCode = @"[&BDIFAAA=]", Type = ObjectiveType.Keep, Map = WvWMap.GreenBorderlands, MapLocation = new Point(-441.6, 9160.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Tower_Sunnyhill, ChatCode = @"[&BDYFAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.GreenBorderlands, MapLocation = new Point(-14153.52, 14712) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Camp_Faithleap, ChatCode = @"[&BDwFAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.GreenBorderlands, MapLocation = new Point(-21083.28, 11462.4) });

            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Camp_Bluevale, ChatCode = @"[&BDgFAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.GreenBorderlands, MapLocation = new Point(-19053.12, -18657.6) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Camp_Bluewater, ChatCode = @"[&BBcFAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.RedBorderlands, MapLocation = new Point(22442.4, -19444.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Camp_Astralholme, ChatCode = @"[&BBQFAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.RedBorderlands, MapLocation = new Point(23253.6, 12273.6) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Camp_Arahs, ChatCode = @"[&BB0FAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.RedBorderlands, MapLocation = new Point(-21083.28, 11462.4) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Camp_Greenvale, ChatCode = @"[&BBwFAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.RedBorderlands, MapLocation = new Point(-19053.12, -18657.6) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Camp_Foghaven, ChatCode = @"[&BDMFAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.GreenBorderlands, MapLocation = new Point(23253.36, 12273.6) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Camp_Redwater, ChatCode = @"[&BDoFAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.GreenBorderlands, MapLocation = new Point(22442.16, -19444.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Camp_Titanpaw, ChatCode = @"[&BDEFAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.GreenBorderlands, MapLocation = new Point(232.8, 34180.8) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Tower_Cragtop, ChatCode = @"[&BDcFAAA=]", Type = ObjectiveType.Tower, Map = WvWMap.GreenBorderlands, MapLocation = new Point(12969.6, 12840) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Camp_Godslore, ChatCode = @"[&BCkFAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.BlueBorderlands, MapLocation = new Point(-21084, 11462.4) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Camp_Redvale, ChatCode = @"[&BC4FAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.BlueBorderlands, MapLocation = new Point(-19053.6, -18657.6) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Camp_Stargrove, ChatCode = @"[&BCEFAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.BlueBorderlands, MapLocation = new Point(23253.6, 12273.6) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Camp_Greenwater, ChatCode = @"[&BCYFAAA=]", Type = ObjectiveType.Camp, Map = WvWMap.BlueBorderlands, MapLocation = new Point(22442.4, -19444.8) });

            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Temple, ChatCode = @"", Type = ObjectiveType.TempleofLostPrayers, Map = WvWMap.RedBorderlands, MapLocation = new Point(-279.59, -16015.255) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Hollow, ChatCode = @"", Type = ObjectiveType.BattlesHollow, Map = WvWMap.RedBorderlands, MapLocation = new Point(-7939.68, -9885.6) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Estate, ChatCode = @"", Type = ObjectiveType.BauersEstate, Map = WvWMap.RedBorderlands, MapLocation = new Point(-5251.525, -783.006) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Orchard, ChatCode = @"", Type = ObjectiveType.OrchardOverlook, Map = WvWMap.RedBorderlands, MapLocation = new Point(5983.413, -2110.027) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.RB_Carvers, ChatCode = @"", Type = ObjectiveType.CarversAscent, Map = WvWMap.RedBorderlands, MapLocation = new Point(6802.815, -10643.904) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Carvers, ChatCode = @"", Type = ObjectiveType.CarversAscent, Map = WvWMap.BlueBorderlands, MapLocation = new Point(6802.815, -10643.904) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Orchard, ChatCode = @"", Type = ObjectiveType.OrchardOverlook, Map = WvWMap.BlueBorderlands, MapLocation = new Point(5983.413, -2110.027) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Estate, ChatCode = @"", Type = ObjectiveType.BauersEstate, Map = WvWMap.BlueBorderlands, MapLocation = new Point(-5251.525, -783.006) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Hollow, ChatCode = @"", Type = ObjectiveType.BattlesHollow, Map = WvWMap.BlueBorderlands, MapLocation = new Point(-7939.68, -9885.6) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.BB_Temple, ChatCode = @"", Type = ObjectiveType.TempleofLostPrayers, Map = WvWMap.BlueBorderlands, MapLocation = new Point(-279.59, -16015.255) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Carvers, ChatCode = @"", Type = ObjectiveType.CarversAscent, Map = WvWMap.GreenBorderlands, MapLocation = new Point(6802.815, -10643.904) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Orchard, ChatCode = @"", Type = ObjectiveType.OrchardOverlook, Map = WvWMap.GreenBorderlands, MapLocation = new Point(5983.413, -2110.027) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Estate, ChatCode = @"", Type = ObjectiveType.BauersEstate, Map = WvWMap.GreenBorderlands, MapLocation = new Point(-5251.525, -783.006) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Hollow, ChatCode = @"", Type = ObjectiveType.BattlesHollow, Map = WvWMap.GreenBorderlands, MapLocation = new Point(-7939.68, -9885.6) });
            table.Objectives.Add(new Objective() { ID = WvWObjectiveIds.GB_Temple, ChatCode = @"", Type = ObjectiveType.TempleofLostPrayers, Map = WvWMap.GreenBorderlands, MapLocation = new Point(-279.59, -16015.255) });

            XmlSerializer serializer = new XmlSerializer(typeof(WvWObjectivesTable));
            TextWriter textWriter = new StreamWriter(FileName);
            serializer.Serialize(textWriter, table);
            textWriter.Close();
        }

        public class Objective
        {
            public WvWObjectiveId ID { get; set; }
            public string ChatCode { get; set; }
            public Point MapLocation { get; set; }
            public ObjectiveType Type { get; set; }
            public WvWMap Map { get; set; }
        }
    }
}
