using GW2PAO.Infrastructure;
using GW2PAO.Modules.Map.Interfaces;
using GW2PAO.Modules.Map.Views;
using GW2PAO.PresentationCore;
using GW2PAO.Utility;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.Map
{
    [Export(typeof(IMapViewController))]
    public class MapViewController : IMapViewController
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
        /// The map window, if open
        /// </summary>
        private MapView mapView;

        /// <summary>
        /// Displays all previously-opened windows and other windows
        /// that must be shown at startup
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing");

            logger.Debug("Registering hotkey commands");
            HotkeyCommands.ToggleMapOverlayCommand.RegisterCommand(new DelegateCommand(this.ToggleMap));

            // Delay a moment to allow the WPF Map Control library to initialize.
            // If we immediately open up the map, it will often fail to load the map tiles. This appears to be
            // an issue with the map control itself, so, as a work-around, we have a hard delay before automatically
            // opening up the map.
            Task.Delay(2000).ContinueWith(o =>
            {
                Threading.BeginInvokeOnUI(() =>
                {
                    if (Properties.Settings.Default.IsMapOpen && this.CanOpenMap())
                        this.OpenMap();
                });
            });
        }

        /// <summary>
        /// Closes all windows and saves the "was previously opened" state for those windows.
        /// </summary>
        public void Shutdown()
        {
            logger.Debug("Shutting down");

            if (this.mapView != null)
            {
                Properties.Settings.Default.IsMapOpen = this.mapView.IsVisible;
                Threading.InvokeOnUI(() => this.mapView.Close());
            }

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Displays a new Map Overlay window
        /// </summary>
        public void OpenMap()
        {
            if (this.mapView == null || !this.mapView.IsVisible)
            {
                this.mapView = new MapView();
                this.Container.ComposeParts(this.mapView);
                this.mapView.Show();
            }
            else
            {
                this.mapView.Focus();
            }
        }

        /// <summary>
        /// Determines if a Map Overlay window can be displayed
        /// </summary>
        /// <returns>Always true</returns>
        public bool CanOpenMap()
        {
            return true;
        }

        /// <summary>
        /// Toggles whether or not the map overlay window is visible
        /// </summary>
        private void ToggleMap()
        {
            if (this.mapView == null || !this.mapView.IsVisible)
            {
                this.OpenMap();
            }
            else
            {
                this.mapView.Close();
            }
        }
    }
}
