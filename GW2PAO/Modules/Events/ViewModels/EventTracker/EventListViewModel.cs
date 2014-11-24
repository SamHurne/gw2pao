using GW2PAO.Modules.Events.Interfaces;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

namespace GW2PAO.Modules.Events.ViewModels.EventTracker
{
    /// <summary>
    /// Primary Event Tracker view model class
    /// </summary>
    [Export(typeof(EventTrackerViewModel))]
    public class EventTrackerViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The Event Tracker controller
        /// </summary>
        private IEventsController controller;

        /// <summary>
        /// Collection of all World Events
        /// </summary>
        public ObservableCollection<EventViewModel> WorldEvents { get { return this.controller.WorldEvents; } }

        /// <summary>
        /// Command to reset all hidden events
        /// </summary>
        public DelegateCommand ResetHiddenEventsCommand { get { return new DelegateCommand(this.ResetHiddenEvents); } }

        /// <summary>
        /// Event Tracker user data
        /// </summary>
        public EventsUserData UserData { get { return this.controller.UserData; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="eventTrackerController">The event tracker controller</param>
        [ImportingConstructor]
        public EventTrackerViewModel(IEventsController eventTrackerController)
        {
            this.controller = eventTrackerController;
        }

        /// <summary>
        /// Resets all hidden events
        /// </summary>
        private void ResetHiddenEvents()
        {
            logger.Debug("Resetting hidden events");
            this.UserData.HiddenEvents.Clear();
        }
    }
}
