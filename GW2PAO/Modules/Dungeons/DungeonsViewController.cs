using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.Dungeons.Interfaces;
using GW2PAO.Modules.Dungeons.Views;
using GW2PAO.Modules.Dungeons.Views.DungeonTimer;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Commands;
using NLog;

namespace GW2PAO.Modules.Dungeons
{
    [Export(typeof(IDungeonsViewController))]
    public class DungeonsViewController : IDungeonsViewController
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
        /// The dungeons tracker view
        /// </summary>
        private DungeonTrackerView dungeonTrackerView;

        /// <summary>
        /// The dungeons timer view
        /// </summary>
        private DungeonTimerView dungeonTimerView;

        /// <summary>
        /// Displays all previously-opened windows and other windows
        /// that must be shown at startup
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing");

            logger.Debug("Registering hotkey commands");
            HotkeyCommands.ToggleDungeonsTrackerCommand.RegisterCommand(new DelegateCommand(this.ToggleDungeonTracker));

            Threading.BeginInvokeOnUI(() =>
            {
                if (Properties.Settings.Default.IsDungeonTrackerOpen && this.CanDisplayDungeonTracker())
                    this.DisplayDungeonTracker();

                if (Properties.Settings.Default.IsDungeonTimerOpen && this.CanDisplayDungeonTimer())
                    this.DisplayDungeonTimer();
            });
        }

        /// <summary>
        /// Closes all windows and saves the "was previously opened" state for those windows.
        /// </summary>
        public void Shutdown()
        {
            logger.Debug("Shutting down");

            if (this.dungeonTrackerView != null)
            {
                Properties.Settings.Default.IsDungeonTrackerOpen = this.dungeonTrackerView.IsVisible;
                Threading.InvokeOnUI(() => this.dungeonTrackerView.Close());
            }

            if (this.dungeonTimerView != null)
            {
                Properties.Settings.Default.IsDungeonTimerOpen = this.dungeonTimerView.IsVisible;
                Threading.InvokeOnUI(() => this.dungeonTimerView.Close());
            }

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Displays the Dungeon Tracker window, or, if already displayed,
        /// sets focus to the window
        /// </summary>
        public void DisplayDungeonTracker()
        {
            if (this.dungeonTrackerView == null || !this.dungeonTrackerView.IsVisible)
            {
                this.dungeonTrackerView = new DungeonTrackerView();
                this.Container.ComposeParts(this.dungeonTrackerView);
                this.dungeonTrackerView.Show();
            }
            else
            {
                this.dungeonTrackerView.Focus();
            }
        }

        /// <summary>
        /// Determines if the dungeon tracker can be displayed
        /// </summary>
        /// <returns></returns>
        public bool CanDisplayDungeonTracker()
        {
            return true;
        }

        /// <summary>
        /// Displays the Dungeon Timer window, or, if already displayed,
        /// sets focus to the window
        /// </summary>
        public void DisplayDungeonTimer()
        {
            if (this.dungeonTimerView == null || !this.dungeonTimerView.IsVisible)
            {
                this.dungeonTimerView = new DungeonTimerView();
                this.Container.ComposeParts(this.dungeonTimerView);
                this.dungeonTimerView.Show();
            }
            else
            {
                this.dungeonTimerView.Focus();
            }
        }

        /// <summary>
        /// Determines if the dungeon timer can be displayed
        /// </summary>
        /// <returns></returns>
        public bool CanDisplayDungeonTimer()
        {
            return true;
        }

        /// <summary>
        /// Toggles whether or not the dungeon tracker is visible
        /// </summary>
        private void ToggleDungeonTracker()
        {
            if (this.dungeonTrackerView == null || !this.dungeonTrackerView.IsVisible)
            {
                this.DisplayDungeonTracker();
            }
            else
            {
                this.dungeonTrackerView.Close();
            }
        }
    }
}
