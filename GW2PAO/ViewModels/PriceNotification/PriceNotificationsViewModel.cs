using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.ViewModels.PriceNotification
{
    public class PriceNotificationsViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
        public PriceNotificationsViewModel(ICommerceController controller)
        {
            this.controller = controller;
        }
    }
}
