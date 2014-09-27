using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Data.Enums
{
    [Flags]
    public enum ItemRestrictions
    {
        None = 0,
        Asura = 1,
        Charr = 2,
        Human = 4,
        Norn = 8,
        Sylvari = 16,
        Elementalist = 32,
        Engineer = 64,
        Guardian = 128,
        Mesmer = 256,
        Necromancer = 512,
        Ranger = 1024,
        Thief = 2048,
        Warrior = 4096
    }
}
