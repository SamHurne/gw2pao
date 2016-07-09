using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using GW2PAO.Modules.Events.Interfaces;
using GW2PAO.Modules.Events.ViewModels.WorldBossTimers;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Events.ViewModels.EventNotification
{
    [Export(typeof(EventNotificationsWindowViewModel))]
    public class EventNotificationsWindowViewModel : BindableBase
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
        public ObservableCollection<WorldBossEventViewModel> WorldBossEventNotifications { get { return this.controller.WorldBossEventNotifications; } }

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
