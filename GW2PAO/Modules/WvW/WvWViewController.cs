using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.WvW.Interfaces;
using GW2PAO.Modules.WvW.Views.WvWNotification;
using GW2PAO.Modules.WvW.Views.WvWTracker;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using NLog;

namespace GW2PAO.Modules.WvW
{
    [Export(typeof(IWvWViewController))]
    public class WvWViewController : IWvWViewController
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Composition container of composed parts
        /// </summary>
        [Import]
        private CompositionContainer Container { get; set; }

        /// <summary>
        /// The event aggregator
        /// </summary>
        [Import]
        private EventAggregator eventAggregator { get; set; }

        /// <summary>
        /// The WvW user data and settings
        /// </summary>
        [Import]
        private WvWUserData userData { get; set; }

        /// <summary>
        /// The wvw tracker view
        /// </summary>
        private WvWTrackerView wvwTrackerView;

        /// <summary>
        /// The wvw notifications window containing all wvw notifications
        /// </summary>
        private WvWNotificationWindow wvwNotificationsView;

        /// <summary>
        /// True if the player is in WvW, else false
        /// </summary>
        private bool isPlayerInWvW;

        /// <summary>
        /// Displays all previously-opened windows and other windows
        /// that must be shown at startup
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing");

            logger.Debug("Registering hotkey commands");
            HotkeyCommands.ToggleWvWTrackerCommmand.RegisterCommand(new DelegateCommand(this.ToggleWvWTracker));

            eventAggregator.GetEvent<PlayerEnteredWvW>().Subscribe(o => this.OnPlayerEnteredWvW());
            eventAggregator.GetEvent<PlayerEnteredPvE>().Subscribe(o => this.OnPlayerExitedWvW());
            eventAggregator.GetEvent<GW2ProcessClosed>().Subscribe(o => this.OnPlayerExitedWvW());

            Threading.BeginInvokeOnUI(() =>
            {
                if (Properties.Settings.Default.IsWvWTrackerOpen && this.CanDisplayWvWTracker())
                    this.DisplayWvWTracker();

                if (this.CanDisplayWvWNotificationsWindow())
                    this.DisplayWvWNotificationsWindow();
            });
        }

        /// <summary>
        /// Closes all windows and saves the "was previously opened" state for those windows.
        /// </summary>
        public void Shutdown()
        {
            logger.Debug("Shutting down");

            if (this.wvwTrackerView != null)
            {
                Properties.Settings.Default.IsWvWTrackerOpen = this.wvwTrackerView.IsVisible;
                Threading.InvokeOnUI(() => this.wvwTrackerView.Close());
            }

            if (this.wvwNotificationsView != null)
            {
                Threading.InvokeOnUI(() => this.wvwNotificationsView.Close());
            }

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Displays the WvW Tracker window, or, if already displayed, sets
        /// focus to the window
        /// </summary>
        public void DisplayWvWTracker()
        {
            if (this.wvwTrackerView == null || !this.wvwTrackerView.IsVisible)
            {
                this.wvwTrackerView = new WvWTrackerView();
                this.Container.ComposeParts(this.wvwTrackerView);
                this.wvwTrackerView.Show();
            }
            else
            {
                this.wvwTrackerView.Focus();
            }
        }

        /// <summary>
        /// Determines if the wvw tracker can be displayed
        /// </summary>
        /// <returns>Always true</returns>
        public bool CanDisplayWvWTracker()
        {
            return true;
        }

        /// <summary>
        /// Displays the WvW Notifications window
        /// </summary>
        public void DisplayWvWNotificationsWindow()
        {
            this.wvwNotificationsView = new WvWNotificationWindow();
            this.Container.ComposeParts(this.wvwNotificationsView);
            this.wvwNotificationsView.Show();
        }

        /// <summary>
        /// Determines if the WvW Notifications window can be displayed
        /// </summary>
        /// <returns>Always true</returns>
        public bool CanDisplayWvWNotificationsWindow()
        {
            return true;
        }

        /// <summary>
        /// Toggles whether or not the wvw tracker is visible
        /// </summary>
        private void ToggleWvWTracker()
        {
            if (this.wvwTrackerView == null || !this.wvwTrackerView.IsVisible)
            {
                this.DisplayWvWTracker();
            }
            else
            {
                this.wvwTrackerView.Close();
            }
        }

        /// <summary>
        /// Performs actions required when the player enters WvW
        /// </summary>
        private void OnPlayerEnteredWvW()
        {
            this.isPlayerInWvW = true;
            if (this.userData.AutoOpenCloseTracker)
            {
                Threading.InvokeOnUI(() =>
                {
                    this.DisplayWvWTracker();
                });
            }
        }

        /// <summary>
        /// Performs actions required when the player exits WvW
        /// </summary>
        private void OnPlayerExitedWvW()
        {
            if (this.userData.AutoOpenCloseTracker && this.isPlayerInWvW)
            {
                Threading.InvokeOnUI(() =>
                {
                    if (this.wvwTrackerView != null && this.wvwTrackerView.IsVisible)
                        this.wvwTrackerView.Close();
                });
            }
            this.isPlayerInWvW = false;
        }
    }
}
