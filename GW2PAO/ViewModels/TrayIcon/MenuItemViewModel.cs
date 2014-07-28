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
        private Func<bool> getIsChecked;
        private Action<bool> setIsChecked;
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
        /// True if the menu item is checkable, else false
        /// </summary>
        public bool IsCheckable { get; private set; }

        /// <summary>
        /// True if the menu item is checked, else false
        /// </summary>
        public bool IsChecked
        {
            get
            {
                if (this.getIsChecked != null)
                    return this.getIsChecked.Invoke();
                else
                    return false;
            }
            set
            {
                if (this.setIsChecked != null)
                    this.setIsChecked(value);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Header text for the menu item</param>
        /// <param name="action">On-click action to perform</param>
        /// <param name="isCheckable">True if the menu item is checkable, else false</param>
        /// <param name="getIsChecked">Getter function used to get the value of the IsChecked property</param>
        /// <param name="setIsChecked">Setter action used to set the value of the IsChecked property</param>
        public MenuItemViewModel(string header, Action action, bool isCheckable = false, Func<bool> getIsChecked = null, Action<bool> setIsChecked = null)
        {
            this.header = header;
            this.command = new DelegateCommand(action);
            this.IsCheckable = isCheckable;
            this.getIsChecked = getIsChecked;
            this.setIsChecked = setIsChecked;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Header text for the menu item</param>
        /// <param name="action">On-click action to perform</param>
        /// <param name="canExcuteAction">CanExecute function</param>
        /// <param name="isCheckable">True if the menu item is checkable, else false</param>
        /// <param name="getIsChecked">Getter function used to get the value of the IsChecked property</param>
        /// <param name="setIsChecked">Setter action used to set the value of the IsChecked property</param>
        public MenuItemViewModel(string header, Action action, Func<bool> canExcuteAction, bool isCheckable = false, Func<bool> getIsChecked = null, Action<bool> setIsChecked = null)
        {
            this.header = header;
            this.command = new DelegateCommand(action, canExcuteAction);
            this.IsCheckable = isCheckable;
            this.getIsChecked = getIsChecked;
            this.setIsChecked = setIsChecked;
        }
    }
}
