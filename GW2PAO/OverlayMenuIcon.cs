using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.PresentationCore;
using GW2PAO.Properties;
using GW2PAO.ViewModels.TrayIcon;
using GW2PAO.Views;

namespace GW2PAO
{
    /// <summary>
    /// Class that provides show/hide functionality for the Overlay Menu Icon
    /// This is more like a controller than a view model, but has aspects of both
    /// </summary>
    public class OverlayMenuIcon : NotifyPropertyChangedBase
    {
        private TrayIconViewModel trayIconViewModel;
        private OverlayMenuIconView menuIconView;

        private bool isVisible;
        public bool IsVisible
        {
            get { return this.isVisible; }
            set
            {
                if (SetField(ref this.isVisible, value))
                {
                    Settings.Default.IsOverlayIconVisible = value;
                    Settings.Default.Save();

                    if (value && menuIconView == null || !menuIconView.IsVisible)
                    {
                        // Showing
                        GW2PAO.Utility.Threading.InvokeOnUI(() =>
                        {
                            menuIconView = new Views.OverlayMenuIconView(trayIconViewModel, Properties.Settings.Default.FirstTimeRun);
                            menuIconView.Show();
                        });
                    }
                    else if (menuIconView != null && menuIconView.IsVisible)
                    {
                        // Closing
                        GW2PAO.Utility.Threading.InvokeOnUI(() =>
                        {
                            menuIconView.Close();
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="vm">The tray icon view model containing the main menu</param>
        public OverlayMenuIcon(TrayIconViewModel vm)
        {
            this.trayIconViewModel = vm;
            this.IsVisible = Settings.Default.IsOverlayIconVisible;
        }

        public void Shutdown()
        {
            if (menuIconView != null && menuIconView.IsVisible)
            {
                // Closing
                GW2PAO.Utility.Threading.InvokeOnUI(() =>
                    {
                        menuIconView.Close();
                    });
            }
        }
    }
}
