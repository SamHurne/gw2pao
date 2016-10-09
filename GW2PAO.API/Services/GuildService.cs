using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2NET;
using GW2NET.Guilds;
using GW2PAO.API.Data;
using GW2PAO.API.Services.Interfaces;
using NLog;
using Guild = GW2PAO.API.Data.Entities.Guild;

namespace GW2PAO.API.Services
{
    [Export(typeof(IGuildService))]
    public class GuildService : IGuildService
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The GW2.NET API service object
        /// </summary>
        private IGuildRepository guildService = GW2.V1.Guilds;

        /// <summary>
        /// Retrieves guild information using the given guild ID
        /// </summary>
        /// <param name="id">The guild's guild ID</param>
        /// <returns>the guild's information</returns>
        public Guild GetGuild(Guid id)
        {
            logger.Debug("Retrieving guild information for {0}", id);

            Guild guild = null;

            try
            {

                var details = this.guildService.Find(id);
                if (details != null)
                {
                    guild = new Guild(id)
                    {
                        Name = details.Name,
                        Tag = details.Tag
                    };
                }
            }
            catch (Exception ex) when (ex is AggregateException || ex is GW2NET.Common.ServiceException || ex is System.Runtime.Serialization.SerializationException)
            {
                // Don't crash on a service exception... just log it
                logger.Warn(ex);
            }

            return guild;
        }

        /// <summary>
        /// Retrieves guild information using the given guild name
        /// </summary>
        /// <param name="name">The guild's name</param>
        /// <returns>the guild's information</returns>
        public Guild GetGuild(string name)
        {
            logger.Debug("Retrieving guild information for \"{0}\"", name);

            Guild guild = null;

            var details = this.guildService.FindByName(name);
            if (details != null)
            {
                guild = new Guild(details.GuildId)
                {
                    Name = details.Name,
                    Tag = details.Tag
                };
            }

            return guild;
        }
    }
}
