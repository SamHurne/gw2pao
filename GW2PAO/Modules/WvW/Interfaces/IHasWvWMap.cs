using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.Modules.WvW.Interfaces
{
    public interface IHasWvWMap
    {
        WvWMap Map { get; set; }
    }
}
