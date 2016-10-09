using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Modules.Map.Interfaces;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GW2PAO.Modules.Map
{
    [Export(typeof(IMenuItem))]
    [ExportMetadata("Order", 8)]
    public class MapMenu : IMenuItem
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
            get { return Properties.Resources.MapOverlay; }
        }

        /// <summary>
        /// Icon source string for the menu item, if any
        /// </summary>
        public string Icon
        {
            get { return "/Images/Title/map.png"; }
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
        public MapMenu(IMapViewController viewFactory)
        {
            // The only option is to open a map window, which is this menu item itself
            this.OnClickCommand = new DelegateCommand(viewFactory.OpenMap, viewFactory.CanOpenMap);
        }
    }
}
