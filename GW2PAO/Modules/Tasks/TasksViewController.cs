using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.Tasks.Interfaces;
using GW2PAO.Modules.Tasks.Views.TaskTracker;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Commands;
using NLog;

namespace GW2PAO.Modules.Tasks
{
    [Export(typeof(IPlayerTasksViewController))]
    public class TasksViewController : IPlayerTasksViewController
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
        /// The task tracker view
        /// </summary>
        private TaskTrackerView taskTrackerView;

        /// <summary>
        /// Displays all previously-opened windows and other windows
        /// that must be shown at startup
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing");

            logger.Debug("Registering hotkey commands");
            HotkeyCommands.ToggleTaskTrackerCommand.RegisterCommand(new DelegateCommand(this.ToggleTaskTracker));

            Threading.BeginInvokeOnUI(() =>
            {
                if (Properties.Settings.Default.IsTaskTrackerOpen && this.CanDisplayTaskTracker())
                    this.DisplayTaskTracker();
            });
        }

        /// <summary>
        /// Closes all windows and saves the "was previously opened" state for those windows.
        /// </summary>
        public void Shutdown()
        {
            logger.Debug("Shutting down");

            if (this.taskTrackerView != null)
            {
                Properties.Settings.Default.IsTaskTrackerOpen = this.taskTrackerView.IsVisible;
                Threading.InvokeOnUI(() => this.taskTrackerView.Close());
            }

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Displays the Zone Completion Assistant window, or, if already displayed,
        /// sets focus to the window
        /// </summary>
        public void DisplayTaskTracker()
        {
            if (this.taskTrackerView == null || !this.taskTrackerView.IsVisible)
            {
                this.taskTrackerView = new TaskTrackerView();
                this.Container.ComposeParts(this.taskTrackerView);
                this.taskTrackerView.Show();
            }
            else
            {
                this.taskTrackerView.Focus();
            }
        }

        /// <summary>
        /// Determines if the zone assistant can be displayed
        /// </summary>
        public bool CanDisplayTaskTracker()
        {
            return true;
        }

        /// <summary>
        /// Toggles whether or not the zone assistant is visible
        /// </summary>
        private void ToggleTaskTracker()
        {
            if (this.taskTrackerView == null || !this.taskTrackerView.IsVisible)
            {
                this.DisplayTaskTracker();
            }
            else
            {
                this.taskTrackerView.Close();
            }
        }
    }
}
