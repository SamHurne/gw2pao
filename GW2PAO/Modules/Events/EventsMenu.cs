using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.Data.UserData;
using GW2PAO.Infrastructure;
using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Infrastructure.ViewModels;
using GW2PAO.Modules.Events.Interfaces;
using Microsoft.Practices.Prism.Commands;

namespace GW2PAO.Modules.Events
{
    [Export(typeof(IMenuItem))]
    [ExportMetadata("Order", 1)]
    public class EventsMenu : IMenuItem
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
            get { return Properties.Resources.Events; }
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
        public EventsMenu(IEventsViewController viewFactory, EventsUserData userData)
        {
            this.SubMenuItems = new ObservableCollection<IMenuItem>();
            this.SubMenuItems.Add(new MenuItem(Properties.Resources.Configure, () => Commands.OpenEventSettingsCommand.Execute(null)));
            this.SubMenuItems.Add(new MenuItem(Properties.Resources.OpenEventsTracker, viewFactory.DisplayEventsTracker, viewFactory.CanDisplayEventsTracker));
            this.SubMenuItems.Add(new CheckableMenuItem(Properties.Resources.EventNotifications, false, () => userData.AreEventNotificationsEnabled, userData));
        }
    }
}
