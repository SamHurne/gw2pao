using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;

namespace GW2PAO.API.Services.Interfaces
{
    public interface ISystemService
    {
        Resolution ScreenResolution { get; }
        Point ScreenCenter { get; }
        Process Gw2Process { get; }
        bool IsGw2Running { get; }
        bool Gw2HasFocus { get; }
    }
}
