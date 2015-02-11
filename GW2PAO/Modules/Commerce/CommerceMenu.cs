using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.Infrastructure;
using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Infrastructure.ViewModels;
using GW2PAO.Modules.Commerce.Interfaces;
using NLog;

namespace GW2PAO.Modules.Commerce
{
    [Export(typeof(IMenuItem))]
    [ExportMetadata("Order", 3)]
    public class CommerceMenu : IMenuItem
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Collection of submenu objects
        /// </summary>
        public ObservableCollection<IMenuItem> SubMenuItems { get; private set; }

        /// <summary>
        /// Header text of the menu item
        /// </summary>
        public string Header
        {
            get { return Properties.Resources.Commerce; }
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
        public CommerceMenu(ICommerceViewController viewFactory, CommerceUserData userData)
        {
            logger.Debug("Initializing menu items");
            this.SubMenuItems = new ObservableCollection<IMenuItem>();

            // Build up the sub menu items
            this.SubMenuItems.Add(new MenuItem(Properties.Resources.Configure, () => Commands.OpenCommerceSettingsCommand.Execute(null)));
            this.SubMenuItems.Add(new MenuItem(Properties.Resources.PriceTracker, viewFactory.DisplayPriceTracker, viewFactory.CanDisplayPriceTracker));

            var notificationsMenu = new MenuItem(Properties.Resources.PriceNotifications);
            notificationsMenu.SubMenuItems.Add(new MenuItem(Properties.Resources.RebuildItemNamesDatabase, viewFactory.DisplayRebuildItemNamesView, viewFactory.CanDisplayRebuildItemNamesView));
            notificationsMenu.SubMenuItems.Add(null); // Null for a seperator
            notificationsMenu.SubMenuItems.Add(new CheckableMenuItem(Properties.Resources.BuyOrderPriceNotifications, true, () => userData.AreBuyOrderPriceNotificationsEnabled, userData));
            notificationsMenu.SubMenuItems.Add(new CheckableMenuItem(Properties.Resources.SellListingPriceNotifications, true, () => userData.AreSellListingPriceNotificationsEnabled, userData));

            this.SubMenuItems.Add(notificationsMenu);
            this.SubMenuItems.Add(null); // Null for a seperator
            this.SubMenuItems.Add(new MenuItem(Properties.Resources.TPCalculator, viewFactory.DisplayTPCalculator, viewFactory.CanDisplayTPCalculator));
        }
    }
}
