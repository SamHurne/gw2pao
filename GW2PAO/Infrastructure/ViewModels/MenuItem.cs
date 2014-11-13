using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Infrastructure.ViewModels
{
    /// <summary>
    /// Basic menu item that can have sub-menu items
    /// </summary>
    public class MenuItem : BindableBase, IMenuItem
    {
        /// <summary>
        /// Collection of Sub-menu items
        /// </summary>
        public ObservableCollection<IMenuItem> SubMenuItems { get; private set; }

        /// <summary>
        /// Header text of the menu item
        /// </summary>
        public string Header { get; private set; }

        /// <summary>
        /// The on-click command
        /// </summary>
        public ICommand OnClickCommand { get; private set; }

        /// <summary>
        /// True if the menu item is checkable, else false
        /// Always false.
        /// </summary>
        public bool IsCheckable { get { return false; } }

        /// <summary>
        /// True if the menu item does not close the menu on click, else false
        /// Always false.
        /// </summary>
        public bool StaysOpen { get { return false; } }

        /// <summary>
        /// True if the menu item is checked, else false
        /// Always false.
        /// </summary>
        public bool IsChecked
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="header">Header text to use for the menu item</param>
        /// <param name="action">Action to perform when clicking the menu item, if any</param>
        /// <param name="canClick">Function used to determine if the menu item can be clicked or not</param>
        public MenuItem(string header, Action action = null, Func<bool> canClick = null)
        {
            this.SubMenuItems = new ObservableCollection<IMenuItem>();
            this.Header = header;

            if (action != null)
            {
                if (canClick == null)
                    this.OnClickCommand = new DelegateCommand(action);
                else
                    this.OnClickCommand = new DelegateCommand(action, canClick);
            }
        }
    }
}
