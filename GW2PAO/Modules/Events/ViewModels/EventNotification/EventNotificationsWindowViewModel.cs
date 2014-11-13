using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using GW2PAO.Modules.Events.Interfaces;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.Modules.Events.ViewModels.EventNotification
{
    [Export(typeof(EventNotificationsWindowViewModel))]
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
        [ImportingConstructor]
        public EventNotificationsWindowViewModel(IEventsController controller)
        {
            this.controller = controller;
        }
    }
}
