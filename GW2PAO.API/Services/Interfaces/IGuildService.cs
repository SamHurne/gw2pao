using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;

namespace GW2PAO.API.Services.Interfaces
{
    public interface IGuildService
    {
        /// <summary>
        /// Retrieves guild information using the given guild ID
        /// </summary>
        /// <param name="id">The guild's guild ID</param>
        /// <returns>the guild's information</returns>
        Guild GetGuild(Guid id);

        /// <summary>
        /// Retrieves guild information using the given guild name
        /// </summary>
        /// <param name="name">The guild's name</param>
        /// <returns>the guild's information</returns>
        Guild GetGuild(string name);
    }
}
