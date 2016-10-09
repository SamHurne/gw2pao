using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Modules.WebBrowser.Interfaces;
using Microsoft.Practices.Prism.Commands;

namespace GW2PAO.Modules.WebBrowser
{
#if !NO_BROWSER
    [Export(typeof(IMenuItem))]
    [ExportMetadata("Order", 9)]
#endif
    public class WebBrowserMenu : IMenuItem
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
            get { return Properties.Resources.WebBrowser; }
        }

        /// <summary>
        /// Icon source string for the menu item, if any
        /// </summary>
        public string Icon
        {
            get { return "/Images/Title/browser.png"; }
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
        public WebBrowserMenu(IWebBrowserController controller)
        {
            // The only option is to open the web browser, which is this menu item itself
            this.OnClickCommand = new DelegateCommand(controller.OpenBrowser, () => { return true; });
        }
    }
}
