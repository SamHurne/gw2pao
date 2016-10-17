using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.Map.Interfaces
{
    public interface IHasMapLocation
    {
        MapControl.Location Location { get; set; }
    }
}
