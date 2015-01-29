using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.WvW;
using Microsoft.Practices.Prism.PubSubEvents;

namespace GW2PAO.Utility
{
    /// <summary>
    /// Utility class that raises events when the user enters PvE or WvW, based on monitoring Map ID
    /// </summary>
    [Export]
    public class GameTypeMonitor
    {
        /// <summary>
        /// Raised when the player has entered a PvE map
        /// </summary>
        private EventAggregator eventAggregator;

        /// <summary>
        /// The map refresh timer object
        /// </summary>
        private Timer checkMapTimer;

        /// <summary>
        /// The current map ID for the player
        /// </summary>
        private int currentMapId;

        /// <summary>
        /// The player service
        /// </summary>
        private IPlayerService playerService;

        /// <summary>
        /// Default constructor
        /// </summary>
        [ImportingConstructor]
        public GameTypeMonitor(IPlayerService playerService, EventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.playerService = playerService;
            this.currentMapId = -1;
        }

        /// <summary>
        /// Starts the game type monitor
        /// </summary>
        public void Start()
        {
            this.checkMapTimer = new Timer(this.CheckMap, null, 500, 500);
        }

        /// <summary>
        /// Stops the game type monitor
        /// </summary>
        public void Stop()
        {
            if (this.checkMapTimer == null)
            {
                this.checkMapTimer.Change(Timeout.Infinite, Timeout.Infinite);
                this.checkMapTimer = null;
            }
        }

        /// <summary>
        /// Performs the actual map check
        /// </summary>
        private void CheckMap(object state = null)
        {
            if (this.playerService.HasValidMapId)
            {
                if (this.currentMapId != this.playerService.MapId)
                {
                    var newMap = this.playerService.MapId;

                    // Map changed
                    if (this.IsWvWMap(newMap) && !this.IsWvWMap(this.currentMapId))
                    {
                        // Player just entered WvW
                        this.eventAggregator.GetEvent<PlayerEnteredWvW>().Publish(null);
                    }
                    else if (!this.IsWvWMap(newMap) && this.IsWvWMap(this.currentMapId))
                    {
                        // Player just exited WvW
                        this.eventAggregator.GetEvent<PlayerEnteredPvE>().Publish(null);
                    }

                    this.currentMapId = this.playerService.MapId;
                }
            }
        }

        /// <summary>
        /// Determines if the given map ID is for one of the WvW maps
        /// </summary>
        /// <param name="mapId">The map ID to check</param>
        /// <returns>True if the given map ID is one of the WvW maps, else false</returns>
        private bool IsWvWMap(int mapId)
        {
            return mapId == WvWMapIDs.EternalBattlegrounds
                || mapId == WvWMapIDs.RedBorderlands
                || mapId == WvWMapIDs.GreenBorderlands
                || mapId == WvWMapIDs.BlueBorderlands;
        }
    }
}
