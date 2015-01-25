using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.API.Util;
using GW2PAO.Data.UserData;
using GW2PAO.Modules.Events.Interfaces;
using GW2PAO.Modules.Events.ViewModels;
using GW2PAO.Utility;
using NLog;

namespace GW2PAO.Modules.Events
{
    /// <summary>
    /// The Events controller. Handles refresh of events, including state and timer values
    /// Also handles notifications and notification states
    /// </summary>
    [Export(typeof(IEventsController))]
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
        /// Service responsible for Zone information
        /// </summary>
        private IZoneService zoneService;

        /// <summary>
        /// Service that provides player location information
        /// </summary>
        private IPlayerService playerService;

        /// <summary>
        /// The primary event refresh timer object
        /// </summary>
        private Timer eventRefreshTimer;

        /// <summary>
        /// Locking object for operations performed with the refresh timer
        /// </summary>
        private readonly object refreshTimerLock = new object();

        /// <summary>
        /// True if the controller's timers are no longer running, else false
        /// </summary>
        private bool isStopped;

        /// <summary>
        /// The user data
        /// </summary>
        private EventsUserData userData;

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
        /// The event tracker user data
        /// </summary>
        public EventsUserData UserData { get { return this.userData; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="eventsService">The events service</param>
        /// <param name="userData">The user settings</param>
        [ImportingConstructor]
        public EventsController(IEventsService eventsService, IZoneService zoneService, IPlayerService playerService, EventsUserData userData)
        {
            logger.Debug("Initializing Event Tracker Controller");
            this.eventsService = eventsService;
            this.zoneService = zoneService;
            this.playerService = playerService;
            this.isStopped = false;

            // Make sure the events service has loaded the appropriate table
            this.eventsService.LoadTable(userData.UseAdjustedTimeTable);

            this.userData = userData;

            // Initialize the refresh timer
            this.eventRefreshTimer = new Timer(this.RefreshEvents);
            this.EventRefreshInterval = 1000;

            // Initialize the start call count to 0
            this.startCallCount = 0;

            // Initialize the WorldEvents collection
            this.InitializeWorldEvents();

            // Set up handling of the event settings UseAdjustedTable property changed so that we can load the correct table when it changes
            this.UserData.PropertyChanged += UserData_PropertyChanged;

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
                    this.isStopped = false;
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
                    this.isStopped = true;
                    this.eventRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Forces a shutdown of the controller, including all running timers/threads
        /// </summary>
        public void Shutdown()
        {
            logger.Debug("Shutdown called");
            logger.Debug("Stopping refresh timers");
            lock (this.refreshTimerLock)
            {
                this.isStopped = true;
                this.eventRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Initializes the collection of world events
        /// </summary>
        private void InitializeWorldEvents()
        {
            lock (refreshTimerLock)
            {
                logger.Debug("Initializing world events");
                this.eventsService.LoadTable(this.UserData.UseAdjustedTimeTable);

                Threading.InvokeOnUI(() =>
                {
                    foreach (var worldEvent in this.eventsService.EventTimeTable.WorldEvents)
                    {
                        logger.Debug("Loading localized name for {0}", worldEvent.ID);
                        worldEvent.Name = this.eventsService.GetLocalizedName(worldEvent.ID);

                        logger.Debug("Loading localized zone location for {0}", worldEvent.ID);
                        worldEvent.MapName = this.zoneService.GetZoneName(worldEvent.MapID);

                        logger.Debug("Initializing view model for {0}", worldEvent.ID);
                        this.WorldEvents.Add(new EventViewModel(worldEvent, this.userData, this.EventNotifications));

                        // If the user data does not contain this event, add it to that collection as well
                        var ens = this.UserData.NotificationSettings.FirstOrDefault(ns => ns.EventID == worldEvent.ID);
                        if (ens == null)
                        {
                            this.UserData.NotificationSettings.Add(new EventNotificationSettings(worldEvent.ID)
                            {
                                EventName = worldEvent.Name
                            });
                        }
                        else
                        {
                            ens.EventName = worldEvent.Name;
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Refreshes all events within the events collection
        /// This is the primary function of the EventTrackerController
        /// </summary>
        private void RefreshEvents(object state = null)
        {
            lock (refreshTimerLock)
            {
                if (this.isStopped)
                    return; // Immediately return if we are supposed to be stopped

                // Refresh the state of all world events
                foreach (var worldEvent in this.WorldEvents)
                {
                    var newState = this.eventsService.GetState(worldEvent.EventModel);
                    Threading.BeginInvokeOnUI(() => worldEvent.State = newState);

                    var timeUntilActive = this.eventsService.GetTimeUntilActive(worldEvent.EventModel);
                    var timeSinceActive = this.eventsService.GetTimeSinceActive(worldEvent.EventModel);

                    Threading.BeginInvokeOnUI(() => worldEvent.TimeSinceActive = timeSinceActive);

                    if (newState == API.Data.Enums.EventState.Active)
                    {
                        Threading.BeginInvokeOnUI(() => worldEvent.TimerValue = timeSinceActive.Negate());
                    }
                    else
                    {
                        Threading.BeginInvokeOnUI(() => worldEvent.TimerValue = timeUntilActive);

                        // Check to see if we need to display a notification for this event
                        var ens = this.UserData.NotificationSettings.FirstOrDefault(ns => ns.EventID == worldEvent.EventId);
                        if (ens != null)
                        {
                            if (ens.IsNotificationEnabled
                                && timeUntilActive.CompareTo(ens.NotificationInterval) < 0)
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

                    // Check to see if the player is within range of any active events
                    // If so, automatically mark them as completed, if that feature is enabled
                    if (this.UserData.AutoDetectCompletion
                        && !worldEvent.IsTreasureObtained)
                    {
                        if (worldEvent.State == API.Data.Enums.EventState.Active
                            && this.playerService.HasValidMapId
                            && this.playerService.MapId == worldEvent.EventModel.MapID)
                        {
                            // Event is active and player is on the same map
                            // At this point, if the player is in range of the event for completion, mark it as completed
                            bool isInRange = false;
                            foreach (var location in worldEvent.EventModel.CompletionLocations)
                            {
                                isInRange = CalcUtil.IsInRadius(location, this.playerService.PlayerPosition, worldEvent.EventModel.CompletionRadius);
                                if (isInRange)
                                    break;
                            }
                            if (isInRange)
                            {
                                logger.Info("Auto-detected completion of \"{0}\"", worldEvent.EventName);
                                Threading.InvokeOnUI(() => worldEvent.IsTreasureObtained = true);
                            }
                        }
                    }
                }

                // Refresh state of daily treasures
                if (DateTime.UtcNow.Date.CompareTo(this.userData.LastResetDateTime.Date) != 0)
                {
                    logger.Info("Resetting daily treasures state");
                    this.userData.LastResetDateTime = DateTime.UtcNow;
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
            if (this.UserData.AreEventNotificationsEnabled)
            {
                Task.Factory.StartNew(() =>
                {
                    logger.Debug("Adding notification for \"{0}\"", eventData.EventName);
                    Threading.InvokeOnUI(() => this.EventNotifications.Add(eventData));

                    // For 10 seconds, loop and sleep, with checks to see if notifications have been disabled
                    for (int i = 0; i < 40; i++)
                    {
                        System.Threading.Thread.Sleep(250);
                        if (!this.UserData.AreEventNotificationsEnabled)
                        {
                            logger.Debug("Removing notification for \"{0}\"", eventData.EventName);
                            Threading.InvokeOnUI(() => this.EventNotifications.Remove(eventData));
                        }
                    }

                    logger.Debug("Removing notification for \"{0}\"", eventData.EventName);

                    // TODO: I hate having this here, but due to a limitation in WPF, there's no reasonable way around this at this time
                    // This makes it so that the notifications can fade out before they are removed from the notification window
                    Threading.InvokeOnUI(() => eventData.IsRemovingNotification = true);
                    System.Threading.Thread.Sleep(250);
                    Threading.InvokeOnUI(() =>
                    {
                        this.EventNotifications.Remove(eventData);
                        eventData.IsRemovingNotification = false;
                    });
                }, TaskCreationOptions.LongRunning);
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event for the EventSettings
        /// </summary>
        private void UserData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UseAdjustedTimeTable")
            {
                Task.Factory.StartNew(() =>
                {
                    // Load a different table
                    lock (refreshTimerLock)
                    {
                        this.eventsService.LoadTable(this.UserData.UseAdjustedTimeTable);
                        Threading.InvokeOnUI(() =>
                        {
                            foreach (var worldEvent in this.WorldEvents)
                            {
                                var newData = this.eventsService.EventTimeTable.WorldEvents.FirstOrDefault(evt => evt.ID == worldEvent.EventId);
                                worldEvent.EventModel.ActiveTimes = newData.ActiveTimes;
                                worldEvent.EventModel.Duration = newData.Duration;
                                worldEvent.EventModel.WarmupDuration = newData.WarmupDuration;
                            }
                        });
                    }
                });
            }
        }
    }
}
