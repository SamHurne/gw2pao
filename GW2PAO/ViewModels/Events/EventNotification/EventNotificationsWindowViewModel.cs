using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.ViewModels.Events.EventNotification
{
    public class EventNotificationsWindowViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The events controller
        /// </summary>
        private IEventsController controller;

        /// <summary>
        /// Collection of active event notifications
        /// </summary>
        public ObservableCollection<EventViewModel> EventNotifications { get { return this.controller.EventNotifications; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="controller">The events controller</param>
        public EventNotificationsWindowViewModel(IEventsController controller)
        {
            this.controller = controller;
        }
    }
}
