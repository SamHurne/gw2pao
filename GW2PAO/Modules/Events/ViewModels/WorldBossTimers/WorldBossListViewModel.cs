using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.Events.Interfaces;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Events.ViewModels.WorldBossTimers
{
    /// <summary>
    /// Primary Event Tracker view model class
    /// </summary>
    [Export(typeof(WorldBossListViewModel))]
    public class WorldBossListViewModel : BindableBase
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
        public ObservableCollection<WorldBossEventViewModel> WorldBossEvents
        {
            get { return this.controller.WorldBossEvents; }
        }

        /// <summary>
        /// Command to reset all hidden events
        /// </summary>
        public DelegateCommand ResetHiddenEventsCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Command to open the settings for the Events Tracker
        /// </summary>
        public DelegateCommand SettingsCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Event Tracker user data
        /// </summary>
        public EventsUserData UserData { get { return this.controller.UserData; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="eventTrackerController">The event tracker controller</param>
        [ImportingConstructor]
        public WorldBossListViewModel(IEventsController eventTrackerController)
        {
            this.controller = eventTrackerController;
            this.ResetHiddenEventsCommand = new DelegateCommand(this.ResetHiddenEvents);
            this.SettingsCommand = new DelegateCommand(() => Commands.OpenEventSettingsCommand.Execute(null));
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
