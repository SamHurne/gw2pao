using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Data.Enums
{
    [Flags]
    public enum ItemGameTypes
    {
        None = 0,
        Activity = 1,
        Dungeon = 2,
        PvE = 4,
        PvP = 8,
        PvPLobby = 16,
        WvW = 32
    }
}
