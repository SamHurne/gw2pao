using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using GW2PAO.ViewModels.Interfaces;
using NLog;

namespace GW2PAO.ViewModels.TrayIcon
{
    /// <summary>
    /// Tray icon view model
    /// </summary>
    public class TrayIconViewModel
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Timer that refreshes the CanExecute state of all menu items for the tray icon
        /// </summary>
        private DispatcherTimer canExecuteTimer;

        /// <summary>
        /// Collection of menu items used in the context menu of the tray icon
        /// </summary>
        public ObservableCollection<IMenuViewModel> MenuItems { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TrayIconViewModel()
        {
            this.MenuItems = new ObservableCollection<IMenuViewModel>();

            // Start a timer that will keep CanExecutes up-to-date
            this.canExecuteTimer = new DispatcherTimer();
            this.canExecuteTimer.Interval = TimeSpan.FromMilliseconds(1000);
            this.canExecuteTimer.Tick += (o, e) =>
            {
                foreach (var menuItem in this.MenuItems)
                    this.RefreshCanExecute(menuItem);
            };
            this.canExecuteTimer.Start();
        }

        /// <summary>
        /// Refreshes the CanExecute state of a menu item
        /// </summary>
        /// <param name="menuItem">The menu item of which to refresh the CanExecute state</param>
        private void RefreshCanExecute(IMenuViewModel menuItem)
        {
            if (menuItem != null)
            {
                foreach (var item in menuItem.SubMenuItems)
                    this.RefreshCanExecute(item);

                menuItem.OnClickCommand.RefreshCanExecuteChanged();
            }
        }
    }
}
