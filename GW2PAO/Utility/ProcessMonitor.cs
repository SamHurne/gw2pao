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
using NLog;

namespace GW2PAO.Utility
{
    /// <summary>
    /// Helper class that monitors the GW2 Process and raises events based on it's state
    /// </summary>
    [Export]
    public class ProcessMonitor : IDisposable
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Refresh interval for the refresh timer
        /// </summary>
        private const int REFRESH_INTERVAL = 500; //ms

        private bool disposed;
        private ISystemService systemService;
        private IPlayerService playerService;
        private bool isAdminRightsErrorShown;
        private EventAggregator eventAggregator;

        /// <summary>
        /// Timer responsible for refreshing
        /// </summary>
        private Timer refreshTimer;

        /// <summary>
        /// True if GW2 is running, else false
        /// </summary>
        private bool isGw2Running;

        /// <summary>
        /// True if GW2 has focus, else false
        /// </summary>
        private bool doesGw2HaveFocus;

        /// <summary>
        /// The current map ID for the player
        /// </summary>
        private int currentMapId;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="systemService"></param>
        [ImportingConstructor]
        public ProcessMonitor(ISystemService systemService, IPlayerService playerService, EventAggregator eventAggregator)
        {
            this.systemService = systemService;
            this.playerService = playerService;
            this.isGw2Running = false;
            this.doesGw2HaveFocus = false;
            this.currentMapId = -1;
            this.isAdminRightsErrorShown = false;
            this.eventAggregator = eventAggregator;
            
        }

        /// <summary>
        /// Starts the game type monitor
        /// </summary>
        public void Start()
        {
            this.refreshTimer = new Timer(this.Refresh, null, REFRESH_INTERVAL, REFRESH_INTERVAL);
        }

        /// <summary>
        /// Stops the game type monitor
        /// </summary>
        public void Stop()
        {
            if (this.refreshTimer == null)
            {
                this.refreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                this.refreshTimer = null;
            }
        }

        /// <summary>
        /// Main functionality of the Process Monitor, refreshes monitor state of the gw2 process
        /// </summary>
        private void Refresh(object state = null)
        {
            try
            {
                var newGW2RunningState = this.systemService.IsGw2Running;
                if (this.isGw2Running != newGW2RunningState)
                {
                    if (newGW2RunningState) // Game just started
                        this.eventAggregator.GetEvent<GW2ProcessStarted>().Publish(null);
                    else // Game just closed
                        this.eventAggregator.GetEvent<GW2ProcessClosed>().Publish(null);
                }
                this.isGw2Running = newGW2RunningState;

                if (this.isGw2Running)
                {
                    var newFocusState = this.systemService.Gw2HasFocus;
                    if (this.doesGw2HaveFocus != newFocusState)
                    {
                        if (newFocusState) // Game gained focus
                            this.eventAggregator.GetEvent<GW2ProcessFocused>().Publish(null);
                        else // Game lost focus
                            this.eventAggregator.GetEvent<GW2ProcessLostFocus>().Publish(null);
                    }
                    this.doesGw2HaveFocus = newFocusState;

                    // No exception thrown, reset bool that keeps track of admin error
                    isAdminRightsErrorShown = false;

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
                    else
                    {
                        this.currentMapId = -1;
                    }
                }
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                // An exception can happen if GW2 is running as admin
                // If that occurs, display a notification
                if (ex.NativeErrorCode == 5)
                {
                    if (!isAdminRightsErrorShown)
                    {
                        this.eventAggregator.GetEvent<InsufficientPrivilegesEvent>().Publish(null);
                        logger.Warn(ex);
                        isAdminRightsErrorShown = true;
                    }
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

        #region IDisposable Implementation

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (refreshTimer != null)
                        refreshTimer.Dispose();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
