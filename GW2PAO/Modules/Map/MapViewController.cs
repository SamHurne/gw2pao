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
        /// Collection of open map windows
        /// </summary>
        private List<MapView> openMapViews = new List<MapView>();

        /// <summary>
        /// Displays all previously-opened windows and other windows
        /// that must be shown at startup
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing");

            logger.Debug("Registering hotkey commands");
            //HotkeyCommands.ToggleEventTrackerCommand.RegisterCommand(new DelegateCommand(this.ToggleEventsTracker));

            //Threading.BeginInvokeOnUI(() =>
            //{
            //    if (Properties.Settings.Default.IsEventTrackerOpen && this.CanDisplayEventsTracker())
            //        this.DisplayEventsTracker();

            //    if (this.CanDisplayEventNotificationsWindow())
            //        this.DisplayEventNotificationsWindow();
            //});
        }

        /// <summary>
        /// Closes all windows and saves the "was previously opened" state for those windows.
        /// </summary>
        public void Shutdown()
        {
            logger.Debug("Shutting down");

            foreach (var mapView in this.openMapViews)
            {
                Threading.InvokeOnUI(() => mapView.Close());
            }

            //if (this.eventTrackerView != null)
            //{
            //    Properties.Settings.Default.IsEventTrackerOpen = this.eventTrackerView.IsVisible;
            //    Threading.InvokeOnUI(() => this.eventTrackerView.Close());
            //}
            //
            //Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Displays a new Map Overlay window
        /// </summary>
        public void OpenMap()
        {
            var mapView = new MapView();
            this.Container.ComposeParts(mapView);
            mapView.Show();
            openMapViews.Add(mapView);
            mapView.Closed += (o, e) =>
            {
                if (this.openMapViews.Contains(mapView))
                    this.openMapViews.Remove(mapView);
            };
            //if (this.eventTrackerView == null || !this.eventTrackerView.IsVisible)
            //{
            //    this.eventTrackerView = new EventTrackerView();
            //    this.Container.ComposeParts(this.eventTrackerView);
            //    this.eventTrackerView.Show();
            //}
            //else
            //{
            //    this.eventTrackerView.Focus();
            //}
        }

        /// <summary>
        /// Determines if a Map Overlay window can be displayed
        /// </summary>
        /// <returns>Always true</returns>
        public bool CanOpenMap()
        {
            return true;
        }
    }
}
