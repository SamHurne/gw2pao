using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.PresentationCore;
using GW2PAO.PresentationCore.Interfaces;
using GW2PAO.ViewModels.Interfaces;

namespace GW2PAO.ViewModels.TrayIcon
{
    /// <summary>
    /// Menu item view model containing details and behavior of a menu item
    /// </summary>
    public class MenuItemViewModel : NotifyPropertyChangedBase, IMenuViewModel
    {
        private string header;
        private DelegateCommand command;
        private ObservableCollection<IMenuViewModel> subMenuItems = new ObservableCollection<IMenuViewModel>();

        /// <summary>
        /// Collection of submenu objects
        /// </summary>
        public ObservableCollection<IMenuViewModel> SubMenuItems
        {
            get { return this.subMenuItems; }
        }

        /// <summary>
        /// Header text of the menu item
        /// </summary>
        public string Header
        {
            get { return this.header; }
            private set { SetField(ref this.header, value); }
        }

        /// <summary>
        /// The on-click command
        /// </summary>
        public IRefreshableCommand OnClickCommand
        {
            get { return this.command; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Header text for the menu item</param>
        /// <param name="action">On-click action to perform</param>
        public MenuItemViewModel(string header, Action action)
        {
            this.header = header;
            this.command = new DelegateCommand(action);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Header text for the menu item</param>
        /// <param name="action">On-click action to perform</param>
        /// <param name="canExcuteAction">CanExecute function</param>
        public MenuItemViewModel(string header, Action action, Func<bool> canExcuteAction)
        {
            this.header = header;
            this.command = new DelegateCommand(action, canExcuteAction);
        }
    }
}
