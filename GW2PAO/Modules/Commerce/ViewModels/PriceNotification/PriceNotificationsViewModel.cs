using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Modules.Commerce.Interfaces;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.Modules.Commerce.ViewModels.PriceNotification
{
    [Export]
    public class PriceNotificationsViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The events controller
        /// </summary>
        private ICommerceController controller;

        /// <summary>
        /// Collection of active event notifications
        /// </summary>
        public ObservableCollection<PriceNotificationViewModel> PriceNotifications { get { return this.controller.PriceNotifications; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="controller">The commerce controller</param>
        [ImportingConstructor]
        public PriceNotificationsViewModel(ICommerceController controller)
        {
            this.controller = controller;
        }
    }
}
