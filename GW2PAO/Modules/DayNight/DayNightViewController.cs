using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Modules.DayNight.Interfaces;
using GW2PAO.Modules.DayNight.Views;
using GW2PAO.Utility;
using NLog;

namespace GW2PAO.Modules.DayNight
{
    [Export(typeof(IDayNightViewController))]
    public class DayNightViewController : IDayNightViewController
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
        /// The day-night timer view
        /// </summary>
        private DayNightTimerView dayNightTimerView;

        /// <summary>
        /// Displays all previously-opened windows and other windows
        /// that must be shown at startup
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing");

            //logger.Debug("Registering hotkey commands");
            //HotkeyCommands.ToggleMapOverlayCommand.RegisterCommand(new DelegateCommand(this.ToggleMap));

            //if (Properties.Settings.Default.IsMapOpen && this.CanOpenMap())
            //    this.OpenMap();
        }

        /// <summary>
        /// Closes all windows and saves the "was previously opened" state for those windows.
        /// </summary>
        public void Shutdown()
        {
            logger.Debug("Shutting down");

            if (this.dayNightTimerView != null)
            {
                //Properties.Settings.Default.IsMapOpen = this.mapView.IsVisible;
                Threading.InvokeOnUI(() => this.dayNightTimerView.Close());
            }

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Displays a new Day-Night timer window, or sets focus to the window if already visible
        /// </summary>
        public void OpenDayNightTimer()
        {
            if (this.dayNightTimerView == null || !this.dayNightTimerView.IsVisible)
            {
                this.dayNightTimerView = new DayNightTimerView();
                this.Container.ComposeParts(this.dayNightTimerView);
                this.dayNightTimerView.Show();
            }
            else
            {
                this.dayNightTimerView.Focus();
            }
        }

        /// <summary>
        /// Determines if a Day-Night timer window can be opened
        /// </summary>
        /// <returns>Always true</returns>
        public bool CanOpenDayNightTimer()
        {
            return true;
        }

        /// <summary>
        /// Toggles whether or not the Day-Night timer is visible
        /// </summary>
        private void ToggleDayNightTimer()
        {
            if (this.dayNightTimerView == null || !this.dayNightTimerView.IsVisible)
            {
                this.OpenDayNightTimer();
            }
            else
            {
                this.dayNightTimerView.Close();
            }
        }
    }
}
