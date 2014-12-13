using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Infrastructure.ViewModels;
using GW2PAO.Modules.Dungeons.Interfaces;
using Microsoft.Practices.Prism.Commands;

namespace GW2PAO.Modules.Dungeons
{
    [Export(typeof(IMenuItem))]
    [ExportMetadata("Order", 3)]
    public class DungeonsMenu : IMenuItem
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
            get { return Properties.Resources.Dungeons; }
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
        public DungeonsMenu(IDungeonsViewController viewFactory)
        {
            this.SubMenuItems = new ObservableCollection<IMenuItem>();
            this.SubMenuItems.Add(new MenuItem(Properties.Resources.DungeonsTracker, viewFactory.DisplayDungeonTracker, viewFactory.CanDisplayDungeonTracker));
            this.SubMenuItems.Add(new MenuItem("Dungeons Timer", viewFactory.DisplayDungeonTimer, viewFactory.CanDisplayDungeonTimer));
        }
    }
}
