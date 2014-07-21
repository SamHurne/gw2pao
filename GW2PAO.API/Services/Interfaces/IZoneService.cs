using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;

namespace GW2PAO.API.Services.Interfaces
{
    public interface IZoneService
    {
        IEnumerable<ZoneItem> GetZoneItems(int mapId);
        string GetZoneName(int mapId);
    }
}
