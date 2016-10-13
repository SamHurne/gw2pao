using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Modules.DayNight.Interfaces;
using GW2PAO.PresentationCore;

namespace GW2PAO.Modules.DayNight
{
    [Export(typeof(IMenuItem))]
    [ExportMetadata("Order", 9)]
    public class DayNightMenu : IMenuItem
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
            get { return Properties.Resources.DayNightTimer; }
        }

        /// <summary>
        /// Icon source string for the menu item, if any
        /// </summary>
        public string Icon
        {
            get { return "/Images/Title/daynight.png"; }
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
        public DayNightMenu(IDayNightViewController viewFactory)
        {
            // The only option is to open the day-night timer window, which is this menu item itself
            this.OnClickCommand = new DelegateCommand(viewFactory.OpenDayNightTimer, viewFactory.CanOpenDayNightTimer);
        }
    }
}
