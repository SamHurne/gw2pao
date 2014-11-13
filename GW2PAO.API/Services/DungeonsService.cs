using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Providers;
using GW2PAO.API.Services.Interfaces;
using NLog;

namespace GW2PAO.API.Services
{
    /// <summary>
    /// Service class for dungeon information
    /// </summary>
    [Export(typeof(IDungeonsService))]
    public class DungeonsService : IDungeonsService
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The Dungeons table containing dungeon information
        /// </summary>
        public DungeonsTable DungeonsTable { get; private set; }

        /// <summary>
        /// String provider for dungeon and dungeon path names
        /// </summary>
        private IStringProvider<Guid> dungeonsStringProvider;

        /// <summary>
        /// Default constructor
        /// </summary>
        public DungeonsService()
        {
            this.dungeonsStringProvider = new DungeonNamesProvider();
        }

        /// <summary>
        /// Alternate constructor
        /// </summary>
        public DungeonsService(IStringProvider<Guid> dungeonNamesProvider)
        {
            this.dungeonsStringProvider = dungeonNamesProvider;
        }

        /// <summary>
        /// Loads the dungeons table and initializes all cached information
        /// </summary>
        public void LoadTable()
        {
            logger.Info("Loading Dungeons Table");
            try
            {
                this.DungeonsTable = DungeonsTable.LoadTable();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Info("Error loading Dungeons Table, re-creating table");
                DungeonsTable.CreateTable();
                this.DungeonsTable = DungeonsTable.LoadTable();
            }
        }

        /// <summary>
        /// Returns the localized name for the given dungeon or dungeon path
        /// </summary>
        /// <param name="id">ID of the dungeon or dungeon path to return the name of</param>
        /// <returns>The localized name</returns>
        public string GetLocalizedName(Guid id)
        {
            return this.dungeonsStringProvider.GetString(id);
        }
    }
}
