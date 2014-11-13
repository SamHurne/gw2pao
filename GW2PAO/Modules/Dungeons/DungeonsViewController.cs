using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Modules.Dungeons.Interfaces;
using GW2PAO.Modules.Dungeons.Views;
using GW2PAO.Utility;
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
        /// Displays all previously-opened windows and other windows
        /// that must be shown at startup
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing");
            Threading.BeginInvokeOnUI(() =>
            {
                if (Properties.Settings.Default.IsDungeonTrackerOpen && this.CanDisplayDungeonTracker())
                    this.DisplayDungeonTracker();
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
    }
}
