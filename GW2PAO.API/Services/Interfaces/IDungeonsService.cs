using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;

namespace GW2PAO.API.Services.Interfaces
{
    public interface IDungeonsService
    {
        /// <summary>
        /// The Dungeons table containing dungeon information
        /// </summary>
        DungeonsTable DungeonsTable { get; }

        /// <summary>
        /// Loads the dungeons table and initializes all cached information
        /// </summary>
        void LoadTable();
    }
}
