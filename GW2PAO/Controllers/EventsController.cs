using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GW2PAO.API.Data;
using GW2PAO.API.Services;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.Models;
using GW2PAO.Utility;
using GW2PAO.ViewModels;
using GW2PAO.ViewModels.EventTracker;
using GW2PAO.ViewModels.Interfaces;
using GW2PAO.ViewModels.ZoneCompletion;
using NLog;

namespace GW2PAO.Controllers
{
    /// <summary>
    /// The Events controller. Handles refresh of events, including state and timer values
    /// Also handles notifications and notification states
    /// </summary>
    public class EventsController : IEventsController
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Service responsible for Event information
        /// </summary>
        private IEventsService eventsService;

        /// <summary>
        /// The primary event refresh timer object
        /// </summary>
        private Timer eventRefreshTimer;

        /// <summary>
        /// Locking object for operations performed with the refresh timer
        /// </summary>
        private readonly object refreshTimerLock = new object();

        /// <summary>
        /// The user settings
        /// </summary>
        private EventSettings userSettings;

        /// <summary>
        /// Keeps track of how many times Start() has been called in order
        /// to support reuse of a single object
        /// </summary>
        private int startCallCount;

        /// <summary>
        /// Backing store of the World Events collection
        /// </summary>
        private ObservableCollection<EventViewModel> worldEvents = new ObservableCollection<EventViewModel>();

        /// <summary>
        /// The collection of World Events
        /// </summary>
        public ObservableCollection<EventViewModel> WorldEvents { get { return this.worldEvents; } }

        /// <summary>
        /// Backing store of the Event Notifications collection
        /// </summary>
        private ObservableCollection<EventViewModel> eventNotifications = new ObservableCollection<EventViewModel>();

        /// <summary>
        /// The collection of events for event notifications
        /// </summary>
        public ObservableCollection<EventViewModel> EventNotifications { get { return this.eventNotifications; } }

        /// <summary>
        /// The interval by which to refresh events (in ms)
        /// </summary>
        public int EventRefreshInterval { get; set; }

        /// <summary>
        /// The event tracker user settings
        /// </summary>
        public EventSettings UserSettings { get { return this.userSettings; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="eventsService">The events service</param>
        /// <param name="userSettings">The user settings</param>
        public EventsController(IEventsService eventsService, EventSettings userSettings)
        {
            logger.Debug("Initializing Event Tracker Controller");
            this.eventsService = eventsService;

            this.userSettings = userSettings;

            // Initialize the refresh timer
            this.eventRefreshTimer = new Timer(this.RefreshEvents);
            this.EventRefreshInterval = 1000;

            // Initialize the start call count to 0
            this.startCallCount = 0;

            // Initialize the WorldEvents collection
            this.InitializeWorldEvents();

            logger.Info("Event Tracker Controller initialized");
        }

        /// <summary>
        /// Starts the automatic refresh
        /// </summary>
        public void Start()
        {
            logger.Debug("Start called");
            Task.Factory.StartNew(() =>
                {
                    // Start the timer if this is the first time that Start() has been called
                    if (this.startCallCount == 0)
                    {
                        logger.Debug("Starting refresh timers");
                        this.RefreshEvents();
                    }

                    this.startCallCount++;
                    logger.Debug("startCallCount = {0}", this.startCallCount);

                }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Stops the automatic refresh
        /// </summary>
        public void Stop()
        {
            this.startCallCount--;
            logger.Debug("Stop called - startCallCount = {0}", this.startCallCount);

            // Stop the refresh timer if all calls to Start() have had a matching call to Stop()
            if (this.startCallCount == 0)
            {
                logger.Debug("Stopping refresh timers");
                lock (refreshTimerLock)
                {
                    this.eventRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Initializes the collection of world events
        /// </summary>
        private void InitializeWorldEvents()
        {
            logger.Debug("Initializing world events");
            this.eventsService.LoadTable();

            Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    foreach (var worldEvent in this.eventsService.EventTimeTable.WorldEvents)
                    {
                        logger.Debug("Initializing view model for {0}", worldEvent.ID);
                        this.WorldEvents.Add(new EventViewModel(worldEvent, this.userSettings, this.EventNotifications));
                    }
                }));
        }

        /// <summary>
        /// Refreshes all events within the events collection
        /// This is the primary function of the EventTrackerController
        /// </summary>
        private void RefreshEvents(object state = null)
        {
            lock (refreshTimerLock)
            {
                // Refresh the state of all world events
                foreach (var worldEvent in this.WorldEvents)
                {
                    var newState = this.eventsService.GetState(worldEvent.EventModel);
                    Threading.BeginInvokeOnUI(() => worldEvent.State = newState);

                    if (newState == API.Data.Enums.EventState.Active)
                    {
                        var timeSinceActive = this.eventsService.GetTimeSinceActive(worldEvent.EventModel);
                        Threading.BeginInvokeOnUI(() => worldEvent.TimerValue = timeSinceActive.Negate());
                    }
                    else
                    {
                        var timeUntilActive = this.eventsService.GetTimeUntilActive(worldEvent.EventModel);
                        Threading.BeginInvokeOnUI(() => worldEvent.TimerValue = timeUntilActive);

                        // Check to see if we need to display a notification for this event
                        if (timeUntilActive.CompareTo(TimeSpan.FromMinutes(1)) < 0)
                        {
                            if (!worldEvent.IsNotificationShown)
                            {
                                worldEvent.IsNotificationShown = true;
                                this.DisplayEventNotification(worldEvent);
                            }
                        }
                        else
                        {
                            // Reset the IsNotificationShown state
                            worldEvent.IsNotificationShown = false;
                        }
                    }
                }

                // Refresh state of daily treasures
                if (DateTime.UtcNow.Date.CompareTo(this.userSettings.LastResetDateTime.Date) != 0)
                {
                    logger.Info("Resetting daily treasures state");
                    this.userSettings.LastResetDateTime = DateTime.UtcNow;
                    Threading.BeginInvokeOnUI(() =>
                    {
                        foreach (var worldEvent in WorldEvents)
                        {
                            worldEvent.IsTreasureObtained = false;
                        }
                    });
                }

                this.eventRefreshTimer.Change(this.EventRefreshInterval, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Adds an event to the event notifications collection, and then removes the event 10 seconds later
        /// </summary>
        private void DisplayEventNotification(EventViewModel eventData)
        {
            if (this.UserSettings.AreEventNotificationsEnabled)
            {
                Task.Factory.StartNew(() =>
                    {
                        logger.Debug("Adding notification for \"{0}\"", eventData.EventName);
                        Threading.InvokeOnUI(() => this.EventNotifications.Add(eventData));

                        // For 10 seconds, loop and sleep, with checks to see if notifications have been disabled
                        for (int i = 0; i < 40; i++)
                        {
                            System.Threading.Thread.Sleep(250);
                            if (!this.UserSettings.AreEventNotificationsEnabled)
                            {
                                logger.Debug("Removing notification for \"{0}\"", eventData.EventName);
                                Threading.InvokeOnUI(() => this.EventNotifications.Remove(eventData));
                            }
                        }

                        logger.Debug("Removing notification for \"{0}\"", eventData.EventName);
                        Threading.InvokeOnUI(() => this.EventNotifications.Remove(eventData));
                    }, TaskCreationOptions.LongRunning);
            }
        }
    }
}
