using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Infrastructure;
using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Infrastructure.ViewModels;
using GW2PAO.Modules.WvW.Interfaces;

namespace GW2PAO.Modules.WvW
{
    [Export(typeof(IMenuItem))]
    [ExportMetadata("Order", 2)]
    public class WvWMenu : IMenuItem
    {
        /// <summary>
        /// Collection of submenu objects
        /// </summary>
        public ObservableCollection<IMenuItem> SubMenuItems { get; private set; }

        /// <summary>
        /// Header text of the menu item
        /// </summary>
        public string Header
        {
            get { return Properties.Resources.WorldvsWorld; }
        }

        /// <summary>
        /// True if the menu item is checkable, else false
        /// </summary>
        public bool IsCheckable
        {
            get { return false; }
        }

        /// <summary>
        /// True if the menu item is checked, else false
        /// </summary>
        public bool IsChecked
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// True if the menu item does not close the menu on click, else false
        /// </summary>
        public bool StaysOpen
        {
            get { return false; }
        }

        /// <summary>
        /// The on-click command
        /// </summary>
        public ICommand OnClickCommand { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        [ImportingConstructor]
        public WvWMenu(IWvWViewController viewFactory, IWvWService wvwService, WvWUserData userData)
        {
            // Build up the sub menu items
            this.SubMenuItems = new ObservableCollection<IMenuItem>();

            // WvW Tracker
            this.SubMenuItems.Add(new MenuItem(Properties.Resources.OpenWvWTracker, viewFactory.DisplayWvWTracker, viewFactory.CanDisplayWvWTracker));
            this.SubMenuItems.Add(new CheckableMenuItem(Properties.Resources.WvWNotifications, false, () => userData.AreNotificationsEnabled, userData));
            this.SubMenuItems.Add(new MenuItem(Properties.Resources.Configure, () => Commands.OpenWvWSettingsCommand.Execute(null)));
        }
    }
}
