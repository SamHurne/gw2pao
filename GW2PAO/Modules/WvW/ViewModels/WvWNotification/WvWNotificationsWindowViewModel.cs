using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Modules.WvW.Interfaces;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.Modules.WvW.ViewModels.WvWNotification
{
    [Export]
    public class WvWNotificationsWindowViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The events controller
        /// </summary>
        private IWvWController controller;

        /// <summary>
        /// Collection of active WvW notifications
        /// </summary>
        public ObservableCollection<WvWObjectiveViewModel> WvWNotifications { get { return this.controller.WvWNotifications; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="controller">The WvW controller</param>
        [ImportingConstructor]
        public WvWNotificationsWindowViewModel(IWvWController controller)
        {
            this.controller = controller;
        }
    }
}
