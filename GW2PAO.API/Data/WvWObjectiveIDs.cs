
namespace GW2PAO.API.Data.Enums
{
    using GW2PAO.API.Data.Entities;

    public static class WvWObjectiveIds
    {
        // Eternal Battlegrounds:
        public static readonly WvWObjectiveId EB_Keep_Overlook      = new WvWObjectiveId(1, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Keep_Valley        = new WvWObjectiveId(2, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Keep_Lowlands      = new WvWObjectiveId(3, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Camp_Golanta       = new WvWObjectiveId(4, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Camp_Pangloss      = new WvWObjectiveId(5, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Camp_Speldan       = new WvWObjectiveId(6, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Camp_Danelon       = new WvWObjectiveId(7, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Camp_Umberglade    = new WvWObjectiveId(8, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Castle_Stonemist   = new WvWObjectiveId(9, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Camp_Rogues        = new WvWObjectiveId(10, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Tower_Aldons       = new WvWObjectiveId(11, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Tower_Wildcreek    = new WvWObjectiveId(12, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Tower_Jerrifers    = new WvWObjectiveId(13, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Tower_Klovan       = new WvWObjectiveId(14, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Tower_Langor       = new WvWObjectiveId(15, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Tower_Quentin      = new WvWObjectiveId(16, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Tower_Mendons      = new WvWObjectiveId(17, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Tower_Anzalias     = new WvWObjectiveId(18, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Tower_Ogrewatch    = new WvWObjectiveId(19, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Tower_Veloka       = new WvWObjectiveId(20, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Tower_Durios       = new WvWObjectiveId(21, WvWMap.EternalBattlegrounds);
        public static readonly WvWObjectiveId EB_Tower_Bravost      = new WvWObjectiveId(22, WvWMap.EternalBattlegrounds);

        // Blue Borderlands:
        public static readonly WvWObjectiveId BB_Keep_Hills         = new WvWObjectiveId(32, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Keep_Bay           = new WvWObjectiveId(33, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Camp_Orchard       = new WvWObjectiveId(34, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Tower_Redbriar     = new WvWObjectiveId(35, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Tower_Greenlake    = new WvWObjectiveId(36, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Keep_Garrison      = new WvWObjectiveId(37, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Tower_Dawns        = new WvWObjectiveId(38, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Camp_Spiritholme   = new WvWObjectiveId(39, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Tower_Woodhaven    = new WvWObjectiveId(40, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Camp_Greenwater    = new WvWObjectiveId(50, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Camp_Godslore      = new WvWObjectiveId(51, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Camp_Stargrove     = new WvWObjectiveId(52, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Camp_Redvale       = new WvWObjectiveId(53, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Temple             = new WvWObjectiveId(62, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Hollow             = new WvWObjectiveId(63, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Estate             = new WvWObjectiveId(64, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Orchard            = new WvWObjectiveId(65, WvWMap.BlueBorderlands);
        public static readonly WvWObjectiveId BB_Carvers            = new WvWObjectiveId(66, WvWMap.BlueBorderlands);

        // Red Borderlands:
        public static readonly WvWObjectiveId RB_Keep_Hills         = new WvWObjectiveId(32, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Keep_Bay           = new WvWObjectiveId(33, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Camp_Orchard       = new WvWObjectiveId(34, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Tower_Greenbriar   = new WvWObjectiveId(35, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Tower_Bluelake     = new WvWObjectiveId(36, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Keep_Garrison      = new WvWObjectiveId(37, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Tower_Longview     = new WvWObjectiveId(38, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Camp_Godsword      = new WvWObjectiveId(39, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Tower_Cliffside    = new WvWObjectiveId(40, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Camp_Bluewater     = new WvWObjectiveId(50, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Camp_Astralholme   = new WvWObjectiveId(51, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Camp_Arahs         = new WvWObjectiveId(52, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Camp_Greenvale     = new WvWObjectiveId(53, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Temple             = new WvWObjectiveId(62, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Hollow             = new WvWObjectiveId(63, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Estate             = new WvWObjectiveId(64, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Orchard            = new WvWObjectiveId(65, WvWMap.RedBorderlands);
        public static readonly WvWObjectiveId RB_Carvers            = new WvWObjectiveId(66, WvWMap.RedBorderlands);

        // Green Borderlands:
        public static readonly WvWObjectiveId GB_Keep_Hills         = new WvWObjectiveId(32, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Keep_Bay           = new WvWObjectiveId(33, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Camp_Orchard       = new WvWObjectiveId(34, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Tower_Bluebriar    = new WvWObjectiveId(35, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Tower_Redlake      = new WvWObjectiveId(36, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Keep_Garrison      = new WvWObjectiveId(37, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Tower_Sunnyhill    = new WvWObjectiveId(38, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Camp_Faithleap     = new WvWObjectiveId(39, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Tower_Cragtop      = new WvWObjectiveId(40, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Camp_Redwater      = new WvWObjectiveId(50, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Camp_Foghaven      = new WvWObjectiveId(51, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Camp_Titanpaw      = new WvWObjectiveId(52, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Camp_Bluevale      = new WvWObjectiveId(53, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Temple             = new WvWObjectiveId(62, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Hollow             = new WvWObjectiveId(63, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Estate             = new WvWObjectiveId(64, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Orchard            = new WvWObjectiveId(65, WvWMap.GreenBorderlands);
        public static readonly WvWObjectiveId GB_Carvers            = new WvWObjectiveId(66, WvWMap.GreenBorderlands);
    }
}
