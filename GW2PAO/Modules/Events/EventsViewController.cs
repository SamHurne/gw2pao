using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.Events.Interfaces;
using GW2PAO.Modules.Events.Views.EventNotification;
using GW2PAO.Modules.Events.Views.MetaEventTimers;
using GW2PAO.Modules.Events.Views.WorldBossTimers;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Commands;
using NLog;

namespace GW2PAO.Modules.Events
{
    [Export(typeof(IEventsViewController))]
    public class EventsViewController : IEventsViewController
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
        /// The world boss timers view
        /// </summary>
        private WorldBossTimersView worldBossTimersView;

        /// <summary>
        /// The meta event timers view
        /// </summary>
        private MetaEventTimersView metaEventTimersView;

        /// <summary>
        /// The event notifications window containing all event notifications
        /// </summary>
        private EventNotificationWindow eventNotificationsView;

        /// <summary>
        /// Displays all previously-opened windows and other windows
        /// that must be shown at startup
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing");

            logger.Debug("Registering hotkey commands");
            HotkeyCommands.ToggleWorldBossTimersCommand.RegisterCommand(new DelegateCommand(this.ToggleWorldBossTimers));
            HotkeyCommands.ToggleMetaEventTimersCommand.RegisterCommand(new DelegateCommand(this.ToggleMetaEventTimers));

            Threading.BeginInvokeOnUI(() =>
            {
                if (Properties.Settings.Default.IsEventTrackerOpen && this.CanDisplayWorldBossTimers())
                    this.DisplayWorldBossTimers();

                if (Properties.Settings.Default.IsMetaEventsTimersOpen && this.CanDisplayMetaEventTimers())
                    this.DisplayMetaEventTimers();

                if (this.CanDisplayEventNotificationsWindow())
                    this.DisplayEventNotificationsWindow();
            });
        }

        /// <summary>
        /// Closes all windows and saves the "was previously opened" state for those windows.
        /// </summary>
        public void Shutdown()
        {
            logger.Debug("Shutting down");

            if (this.worldBossTimersView != null)
            {
                Properties.Settings.Default.IsEventTrackerOpen = this.worldBossTimersView.IsVisible;
                Threading.InvokeOnUI(() => this.worldBossTimersView.Close());
            }

            if (this.metaEventTimersView != null)
            {
                Properties.Settings.Default.IsMetaEventsTimersOpen = this.metaEventTimersView.IsVisible;
                Threading.InvokeOnUI(() => this.metaEventTimersView.Close());
            }

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Displays the World Boss Timers window, or, if already displayed, sets
        /// focus to the window
        /// </summary>
        public void DisplayWorldBossTimers()
        {
            if (this.worldBossTimersView == null || !this.worldBossTimersView.IsVisible)
            {
                this.worldBossTimersView = new WorldBossTimersView();
                this.Container.ComposeParts(this.worldBossTimersView);
                this.worldBossTimersView.Show();
            }
            else
            {
                this.worldBossTimersView.Focus();
            }
        }

        /// <summary>
        /// Determines if the world boss timers window can be displayed
        /// </summary>
        /// <returns>Always true</returns>
        public bool CanDisplayWorldBossTimers()
        {
            return true;
        }

        /// <summary>
        /// Displays the Event Notifications window
        /// </summary>
        public void DisplayEventNotificationsWindow()
        {
            this.eventNotificationsView = new EventNotificationWindow();
            this.Container.ComposeParts(this.eventNotificationsView);
            this.eventNotificationsView.Show();
        }

        /// <summary>
        /// Determines if the Event Notifications window can be displayed
        /// </summary>
        /// <returns>Always true</returns>
        public bool CanDisplayEventNotificationsWindow()
        {
            return true;
        }

        /// <summary>
        /// Toggles whether or not the world boss timers window is visible
        /// </summary>
        private void ToggleWorldBossTimers()
        {
            if (this.worldBossTimersView == null || !this.worldBossTimersView.IsVisible)
            {
                this.DisplayWorldBossTimers();
            }
            else
            {
                this.worldBossTimersView.Close();
            }
        }

        /// <summary>
        /// Displays the Meta Event Timers window, or, if already displayed, sets
        /// focus to the window
        /// </summary>
        public void DisplayMetaEventTimers()
        {
            if (this.metaEventTimersView == null || !this.metaEventTimersView.IsVisible)
            {
                this.metaEventTimersView = new MetaEventTimersView();
                this.Container.ComposeParts(this.metaEventTimersView);
                this.metaEventTimersView.Show();
            }
            else
            {
                this.metaEventTimersView.Focus();
            }
        }

        /// <summary>
        /// Determines if the Meta Event Timers window can be displayed
        /// </summary>
        /// <returns>Always true</returns>
        public bool CanDisplayMetaEventTimers()
        {
            return true;
        }

        /// <summary>
        /// Toggles whether or not the meta event timers window is visible
        /// </summary>
        private void ToggleMetaEventTimers()
        {
            if (this.metaEventTimersView == null || !this.metaEventTimersView.IsVisible)
            {
                this.DisplayMetaEventTimers();
            }
            else
            {
                this.metaEventTimersView.Close();
            }
        }
    }
}
