using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.PresentationCore.Interfaces;

namespace GW2PAO.ViewModels.Interfaces
{
    /// <summary>
    /// Interface for a menu view model
    /// </summary>
    public interface IMenuViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Collection of submenu objects
        /// </summary>
        ObservableCollection<IMenuViewModel> SubMenuItems { get; }

        /// <summary>
        /// Header text of the menu item
        /// </summary>
        string Header { get; }

        /// <summary>
        /// True if the menu item is checkable, else false
        /// </summary>
        bool IsCheckable { get; }

        /// <summary>
        /// True if the menu item is checked, else false
        /// </summary>
        bool IsChecked { get; set; }

        /// <summary>
        /// True if the menu item does not close the menu on click, else false
        /// </summary>
        bool StaysOpen { get; }

        /// <summary>
        /// The on-click command
        /// </summary>
        IRefreshableCommand OnClickCommand { get; }
    }
}
