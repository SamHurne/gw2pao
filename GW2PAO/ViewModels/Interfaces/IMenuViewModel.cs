using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.PresentationCore.Interfaces;

namespace GW2PAO.ViewModels.Interfaces
{
    /// <summary>
    /// Interface for a menu view model
    /// </summary>
    public interface IMenuViewModel
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
        /// The on-click command
        /// </summary>
        IRefreshableCommand OnClickCommand { get; }
    }
}
