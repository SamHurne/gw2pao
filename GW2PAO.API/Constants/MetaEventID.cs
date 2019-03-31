using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Constants
{
    public class MetaEventID
    {
        public static readonly Guid TimberlineFalls = new Guid("32B1CE39-D2E0-4FCE-BDF3-6E00635FA398");
        public static readonly Guid IronMarches = new Guid("BAE5AD78-9540-4CEC-B1E5-AD57FB08A894");
        public static readonly Guid GendarranFields = new Guid("2A926CF0-AEF9-4BD2-BD01-7F03C7931D5B");
        public static readonly Guid DryTop = new Guid("F19EA7AF-7FFD-42A0-B20A-530C8F55DB32");
        public static readonly Guid VerdantBrink = new Guid("F65F53A9-9665-4049-8518-C04F4FAADA44");
        public static readonly Guid AuricBasin = new Guid("5544E617-3930-42C0-93D5-9FA0F938E5AD");
        public static readonly Guid TangledDepths = new Guid("7A2F2E16-CDE1-4C02-82BC-F37199301515");
        public static readonly Guid DragonsStand = new Guid("B03463F6-5CA6-4F0D-B614-B2DC1DFBDF2A");
        public static readonly Guid LakeDoric = new Guid("3B6E6919-934E-422F-8953-ECA63D66EA62");
        public static readonly Guid CrystalOasis = new Guid("E64A769A-A5AD-4F5D-93AF-CF2022BA88B3");
        public static readonly Guid DesertHighlands = new Guid("38A1C2C4-0BDC-4138-BA9E-CDBA7A088421");
        public static readonly Guid DomainOfVabbi = new Guid("FB30F672-8737-48B9-9FEE-A6A2A72EC9FF");
        public static readonly Guid DomainOfIstan = new Guid("6E705FEC-1C60-477B-B3DC-C4FBE35C3B97");
        public static readonly Guid JahaiBluffs = new Guid("99138F68-8016-45FF-B321-902DDCEAD962");
        public static readonly Guid ThunderheadPeaks = new Guid("6C33E8C9-959C-412F-93D8-CDF3EF19B6F2");
    }

    public class MetaEventStageID
    {
        // Core & LW2 Zones
        public static readonly Guid TimberlineFalls_LeyLine = new Guid("92B86E45-85CD-4AFA-8527-F9B300F3B79C");
        public static readonly Guid IronMarches_LeyLine = new Guid("D7C293F4-1B69-435F-A975-AEEE6167D542");
        public static readonly Guid GendarranFields_LeyLine = new Guid("4DA7CD29-BCCD-49F1-AD40-99807876070F");
        public static readonly Guid DryTop_CrashSite = new Guid("03E26C95-D7B6-4333-993C-1B2C1BB1CE10");
        public static readonly Guid DryTop_Sandstorm = new Guid("1C351A1B-3752-4758-B0C9-CAD397DAC3F8");

        // Heart of Thorns Zones
        public static readonly Guid VerdantBrink_NightBosses = new Guid("0C198B36-80D4-415D-836C-BFE02E6A1087");
        public static readonly Guid VerdantBrink_Daytime = new Guid("D766B894-11E2-4042-97F3-347DC14332E2");
        public static readonly Guid VerdantBrink_Night = new Guid("0E1D38EA-37BE-4C1B-8B28-B91727488B1B");
        public static readonly Guid AuricBasin_Challenges = new Guid("2F07D9A1-62F7-4F6D-BBDE-A4AE2CECC946");
        public static readonly Guid AuricBasin_Octovine = new Guid("2C3E6E9B-36E9-4EC0-A3E6-1088883FE92E");
        public static readonly Guid AuricBasin_Reset = new Guid("9CA89206-A892-4939-844A-02EFFA441494");
        public static readonly Guid AuricBasin_Pylons = new Guid("5D5886AE-6340-4B1F-A44F-52FA1C87ABD0");
        public static readonly Guid TangledDepths_Preparation = new Guid("8A9D1EDB-0F16-41F6-956B-1909E58D8293");
        public static readonly Guid TangledDepths_ChakGerent = new Guid("4D58ABE3-3506-4854-B28A-4860EBBA9615");
        public static readonly Guid TangledDepths_HelpOutposts = new Guid("8B298EE8-2C3C-491F-AA93-8F1A73D4D8CC");
        public static readonly Guid DragonsStand_MapActive = new Guid("79C4A416-50F1-4E0C-B7E7-DD096FFD6633");

        // LW3 Zones        
        public static readonly Guid LakeDoric_Noran = new Guid("B4D908D8-BB02-466A-B3F5-B2D0B970FC79");
        public static readonly Guid LakeDoric_Saidra = new Guid("0097E544-FF42-4097-A0E7-D34D261B8B47");
        public static readonly Guid LakeDoric_Loamhurst = new Guid("59E20379-DDEF-4609-BB19-BDC06AAB472A");

        // Path of Fire Zone
        public static readonly Guid CrystalOasis_CasinoBlitz = new Guid("308966B1-F0ED-42CA-B4D4-17CE6DC53981");
        public static readonly Guid DesertHighlands_Treasure = new Guid("C46AE04F-DAD1-4919-8F07-11C20D9C621A");
        public static readonly Guid DomainOfVabbi_Serpent = new Guid("BC211348-29D1-48BE-83AE-739A5F0E4274");

        // LW4 Zones
        public static readonly Guid DomainOfIstan_Palawadan = new Guid("1BB7E949-3FC9-455F-873C-9FD1178CD3B5");
        public static readonly Guid JahaiBluffs_DangerousPrey = new Guid("35D30F9D-C9AB-4A2E-A193-22CC76AFE387");
        public static readonly Guid ThunderheadPeaks_Keep = new Guid("9CD21738-2108-4E15-9102-F1F4B10001F5");
        public static readonly Guid ThunderheadPeaks_Oil = new Guid("CBA88373-C024-44A1-9202-6A4387BE949E");
    }
}
