using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Constants
{
    public class MetaEventID
    {
        public static readonly Guid DryTop = new Guid("F19EA7AF-7FFD-42A0-B20A-530C8F55DB32");
        public static readonly Guid VerdantBrink = new Guid("F65F53A9-9665-4049-8518-C04F4FAADA44");
        public static readonly Guid AuricBasin = new Guid("5544E617-3930-42C0-93D5-9FA0F938E5AD");
        public static readonly Guid TangledDepths = new Guid("7A2F2E16-CDE1-4C02-82BC-F37199301515");
        public static readonly Guid DragonsStand = new Guid("B03463F6-5CA6-4F0D-B614-B2DC1DFBDF2A");
    }

    public class MetaEventStageID
    {
        public static readonly Guid DryTop_CrashSite = new Guid("03E26C95-D7B6-4333-993C-1B2C1BB1CE10");
        public static readonly Guid DryTop_Sandstorm = new Guid("1C351A1B-3752-4758-B0C9-CAD397DAC3F8");
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
    }
}
