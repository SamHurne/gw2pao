using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Infrastructure.ViewModels;
using GW2PAO.Modules.WvW.Interfaces;

namespace GW2PAO.Modules.WvW
{
    [Export(typeof(IMenuItem))]
    [ExportMetadata("Order", 4)]
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

            // World Selection menu
            var worldSelectionMenu = new MenuItem(Properties.Resources.WorldSelection);

            // Make sure the service has loaded the world names
            if (wvwService.Worlds == null)
                wvwService.LoadTable();

            var naWorlds = new MenuItem("NA");
            foreach (var world in wvwService.Worlds.Worlds.Where(wld => wld.ID < 2000))
            {
                var worldMenuItem = new CheckableMenuItem(world.Name, false,
                    (selected) => { if (selected) userData.WorldSelection = world; },
                    () => { return userData.WorldSelection.ID == world.ID; },
                    userData,
                    () => userData.WorldSelection);

                naWorlds.SubMenuItems.Add(worldMenuItem);
            }
            worldSelectionMenu.SubMenuItems.Add(naWorlds);

            var euWorlds = new MenuItem("EU");
            foreach (var world in wvwService.Worlds.Worlds.Where(wld => wld.ID > 2000))
            {
                var worldMenuItem = new CheckableMenuItem(world.Name, false,
                    (selected) => { if (selected) userData.WorldSelection = world; },
                    () => { return userData.WorldSelection.ID == world.ID; },
                    userData,
                    () => userData.WorldSelection);

                euWorlds.SubMenuItems.Add(worldMenuItem);
            }
            worldSelectionMenu.SubMenuItems.Add(euWorlds);

            this.SubMenuItems.Add(worldSelectionMenu);

            // WvW Tracker
            this.SubMenuItems.Add(new MenuItem(Properties.Resources.OpenWvWTracker, viewFactory.DisplayWvWTracker, viewFactory.CanDisplayWvWTracker));

            // WvW Notifications
            var notificationsMenu = new MenuItem(Properties.Resources.WvWNotifications);
            notificationsMenu.SubMenuItems.Add(new MenuItem(Properties.Resources.EnableAll, () =>
            {
                userData.NotifyWhenHomeTakesObjective = true;
                userData.NotifyWhenHomeLosesObjective = true;
                userData.NotifyWhenOtherTakesOtherObjective = true;
            }));
            notificationsMenu.SubMenuItems.Add(new MenuItem(Properties.Resources.DisableAll, () =>
            {
                userData.NotifyWhenHomeTakesObjective = false;
                userData.NotifyWhenHomeLosesObjective = false;
                userData.NotifyWhenOtherTakesOtherObjective = false;
            }));
            notificationsMenu.SubMenuItems.Add(null); // Null for a seperator.
            notificationsMenu.SubMenuItems.Add(new CheckableMenuItem(Properties.Resources.WhenHomeWorldTakesObjective, true, () => userData.NotifyWhenHomeTakesObjective, userData));
            notificationsMenu.SubMenuItems.Add(new CheckableMenuItem(Properties.Resources.WhenHomeWorldLosesObjective, true, () => userData.NotifyWhenHomeLosesObjective, userData));
            notificationsMenu.SubMenuItems.Add(new CheckableMenuItem(Properties.Resources.WhenOtherWorldTakesOtherWorldsObjective, true, () => userData.NotifyWhenOtherTakesOtherObjective, userData));
            notificationsMenu.SubMenuItems.Add(null); // Null for a seperator

            var notificationsMapMenu = new MenuItem(Properties.Resources.Maps);
            notificationsMapMenu.SubMenuItems.Add(new MenuItem(Properties.Resources.EnableAll, () =>
            {
                userData.AreEternalBattlegroundsNotificationsEnabled = true;
                userData.AreBlueBorderlandsNotificationsEnabled = true;
                userData.AreGreenBorderlandsNotificationsEnabled = true;
                userData.AreRedBorderlandsNotificationsEnabled = true;
            }));
            notificationsMapMenu.SubMenuItems.Add(new MenuItem(Properties.Resources.DisableAll, () =>
            {
                userData.AreEternalBattlegroundsNotificationsEnabled = false;
                userData.AreBlueBorderlandsNotificationsEnabled = false;
                userData.AreGreenBorderlandsNotificationsEnabled = false;
                userData.AreRedBorderlandsNotificationsEnabled = false;
            }));
            notificationsMapMenu.SubMenuItems.Add(null); // Null for a seperator
            notificationsMapMenu.SubMenuItems.Add(new CheckableMenuItem(Properties.Resources.EternalBattlegrounds, true, () => userData.AreEternalBattlegroundsNotificationsEnabled, userData));
            notificationsMapMenu.SubMenuItems.Add(new CheckableMenuItem(Properties.Resources.BlueBorderlands, true, () => userData.AreBlueBorderlandsNotificationsEnabled, userData));
            notificationsMapMenu.SubMenuItems.Add(new CheckableMenuItem(Properties.Resources.GreenBorderlands, true, () => userData.AreGreenBorderlandsNotificationsEnabled, userData));
            notificationsMapMenu.SubMenuItems.Add(new CheckableMenuItem(Properties.Resources.RedBorderlands, true, () => userData.AreRedBorderlandsNotificationsEnabled, userData));

            notificationsMenu.SubMenuItems.Add(notificationsMapMenu);

            this.SubMenuItems.Add(notificationsMenu);
        }
    }
}
