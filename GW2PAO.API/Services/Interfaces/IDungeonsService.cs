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

        /// <summary>
        /// Returns the localized name for the given dungeon or dungeon path
        /// </summary>
        /// <param name="id">ID of the dungeon or dungeon path to return the name of</param>
        /// <returns>The localized name</returns>
        string GetLocalizedName(Guid id);
    }
}
