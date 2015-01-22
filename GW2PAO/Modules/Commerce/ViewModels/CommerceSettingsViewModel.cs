using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Modules.Commerce.Interfaces;
using GW2PAO.Properties;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Commerce.ViewModels
{
    [Export(typeof(CommerceSettingsViewModel))]
    public class CommerceSettingsViewModel : BindableBase, ISettingsViewModel
    {
        /// <summary>
        /// Settings header
        /// </summary>
        public string SettingsHeader
        {
            get { return Resources.Commerce; }
        }

        /// <summary>
        /// The commerce user data
        /// </summary>
        public CommerceUserData UserData
        {
            get;
            private set;
        }

        /// <summary>
        /// View Model for configuring watched items
        /// </summary>
        public MonitoredItemsConfigViewModel MonitoredItemsConfigViewModel
        {
            get;
            private set;
        }

        /// <summary>
        /// Command to kick off the item names database rebuild
        /// </summary>
        public DelegateCommand RebuildNamesDatabaseCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="userData">The commerce user data</param>
        [ImportingConstructor]
        public CommerceSettingsViewModel(ICommerceViewController viewController, CommerceUserData userData, MonitoredItemsConfigViewModel monitoredItemsConfigViewModel)
        {
            this.UserData = userData;
            this.MonitoredItemsConfigViewModel = monitoredItemsConfigViewModel;
            this.RebuildNamesDatabaseCommand = new DelegateCommand(viewController.DisplayRebuildItemNamesView, viewController.CanDisplayRebuildItemNamesView);
        }
    }
}
