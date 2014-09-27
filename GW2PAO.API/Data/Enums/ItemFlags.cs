using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Data.Enums
{
    [Flags]
    public enum ItemFlags
    {
        None = 0,
        AccountBound = 1,
        HideSuffix = 2,
        NoMysticForge = 4,
        NoSalvage = 8,
        NoSell = 16,
        NotUpgradeable = 32,
        NoUnderwater = 64,
        SoulBindOnAcquire = 128,
        SoulBindOnUse = 256,
        Unique = 512,
        AccountBindOnUse = 1024,
        MonsterOnly = 2048
    }
}
