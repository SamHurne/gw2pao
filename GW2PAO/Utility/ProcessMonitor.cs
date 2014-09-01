using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Utility.Interfaces;
using NLog;

namespace GW2PAO.Utility
{
    /// <summary>
    /// Helper class that monitors the GW2 Process and raises events based on it's state
    /// </summary>
    public class ProcessMonitor : IProcessMonitor
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
        private bool isAdminRightsErrorShown;

        /// <summary>
        /// Timer responsible for refreshing
        /// </summary>
        private Timer refreshTimer;

        /// <summary>
        /// True if GW2 has focus, else false
        /// </summary>
        private bool doesGw2HaveFocus;

        /// <summary>
        /// Raised when the GW2 Process gains focus
        /// </summary>
        public event EventHandler GW2Focused;

        /// <summary>
        /// Raised when the GW2 Process loses focus
        /// </summary>
        public event EventHandler GW2LostFocus;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="systemService"></param>
        public ProcessMonitor(ISystemService systemService)
        {
            this.systemService = systemService;
            this.doesGw2HaveFocus = false;
            this.isAdminRightsErrorShown = false;
            this.refreshTimer = new Timer(this.Refresh, null, REFRESH_INTERVAL, REFRESH_INTERVAL);
        }

        /// <summary>
        /// Main functionality of the Process Monitor, refreshes monitor state of the gw2 process
        /// </summary>
        private void Refresh(object state = null)
        {
            try
            {
                var newFocusState = this.systemService.Gw2HasFocus;
                if (this.doesGw2HaveFocus != newFocusState)
                {
                    // Focus state changed

                    if (newFocusState)
                    {
                        this.RaiseGW2Focused();
                    }
                    else
                    {
                        this.RaiseGW2LostFocus();
                    }
                    this.doesGw2HaveFocus = newFocusState;
                }

                // No exception thrown, reset bool that keeps track of admin error
                isAdminRightsErrorShown = false;
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                // An exception can happen if GW2 is running as admin
                // If that occurs, display a notification
                if (ex.NativeErrorCode == 5)
                {
                    if (!isAdminRightsErrorShown)
                    {
                        App.TrayIcon.DisplayNotification("Warning", "Some features cannot be started because GW2 is running as administrator.", GW2PAO.TrayIcon.TrayInfoMessageType.Warning);
                        logger.Warn(ex);
                        isAdminRightsErrorShown = true;
                    }
                }
            }
        }

        /// <summary>
        /// Raises the GW2Focused event
        /// </summary>
        private void RaiseGW2Focused()
        {
            if (this.GW2Focused != null)
                this.GW2Focused(this, new EventArgs());
        }

        /// <summary>
        /// Raises the GW2LostFocus event
        /// </summary>
        private void RaiseGW2LostFocus()
        {
            if (this.GW2LostFocus != null)
                this.GW2LostFocus(this, new EventArgs());
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
