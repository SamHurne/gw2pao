using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Services.Interfaces;
using NLog;

namespace GW2PAO.API.Services
{
    /// <summary>
    /// Service class for dungeon information
    /// </summary>
    public class DungeonsService : IDungeonsService
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The Dungeons table
        /// </summary>
        public DungeonsTable DungeonsTable { get; private set; }

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
    }
}
