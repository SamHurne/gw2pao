using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Modules.WvW.Interfaces;
using GW2PAO.Modules.WvW.Views.WvWNotification;
using GW2PAO.Modules.WvW.Views.WvWTracker;
using GW2PAO.Utility;
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
        /// The wvw tracker view
        /// </summary>
        private WvWTrackerView wvwTrackerView;

        /// <summary>
        /// The wvw notifications window containing all wvw notifications
        /// </summary>
        private WvWNotificationWindow wvwNotificationsView;

        /// <summary>
        /// Displays all previously-opened windows and other windows
        /// that must be shown at startup
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing");
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

            // TODO: Also close down the notifications window

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
    }
}
