using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Data.Enums
{
    public enum ItemRarity
    {
        Unknown = 0,
        Junk = 1,
        Basic = 2,
        Fine = 4,
        Masterwork = 8,
        Rare = 16,
        Exotic = 32,
        Ascended = 64,
        Legendary = 128
    }
}
