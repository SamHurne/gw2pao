using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;

namespace GW2PAO.API.Services.Interfaces
{
    public interface IZoneService
    {
        /// <summary>
        /// Retrieves a collection of ZoneItems located in the zone with the given mapID
        /// </summary>
        /// <param name="mapId">The mapID of the zone to retrieve zone items for</param>
        /// <returns>a collection of ZoneItems located in the zone with the given mapID</returns>
        IEnumerable<ZoneItem> GetZoneItems(int mapId);

        /// <summary>
        /// Retrieves the name of the zone using the given mapID
        /// </summary>
        /// <param name="mapId">The mapID of the zone to retrieve the name for</param>
        /// <returns>the name of the zone</returns>
        string GetZoneName(int mapId);
    }
}
