using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.API.Util;
using GW2PAO.Modules.Events.Interfaces;
using GW2PAO.Modules.Events.ViewModels.EventNotification;
using GW2PAO.Modules.Events.ViewModels.MetaEventTimers;
using GW2PAO.Modules.Events.ViewModels.WorldBossTimers;
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
        /// Backing store of the Meta Events collection
        /// </summary>
        private ObservableCollection<MetaEventViewModel> metaEvents = new ObservableCollection<MetaEventViewModel>();

        /// <summary>
        /// The collection of Meta Events
        /// </summary>
        public ObservableCollection<MetaEventViewModel> MetaEvents { get { return this.metaEvents; } }

        /// <summary>
        /// Backing store of the World Boss Events collection
        /// </summary>
        private ObservableCollection<WorldBossEventViewModel> worldEvents = new ObservableCollection<WorldBossEventViewModel>();

        /// <summary>
        /// The collection of World Boss Events
        /// </summary>
        public ObservableCollection<WorldBossEventViewModel> WorldBossEvents { get { return this.worldEvents; } }

        /// <summary>
        /// Dictionary containing a mapping of what events have 'armed' notifications
        /// An event is considered 'armed' if it's notification has not already been shown
        /// </summary>
        private Dictionary<Guid, bool> armedEventNotifications = new Dictionary<Guid, bool>();

        /// <summary>
        /// Backing store of the collection of notifications for world boss and meta event notifications
        /// </summary>
        private ObservableCollection<IEventNotification> eventNotifications = new ObservableCollection<IEventNotification>();

        /// <summary>
        /// The collection of notifications for world boss and meta event notifications
        /// </summary>
        public ObservableCollection<IEventNotification> EventNotifications { get { return this.eventNotifications; } }

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

            this.userData = userData;

            // Initialize the refresh timer
            this.eventRefreshTimer = new Timer(this.RefreshEvents);
            this.EventRefreshInterval = 1000;

            // Initialize the start call count to 0
            this.startCallCount = 0;

            // Set up handling of the event settings UseAdjustedTable property changed so that we can load the correct table when it changes
            this.UserData.PropertyChanged += UserData_PropertyChanged;

            // Initialize the WorldEvents collection
            this.InitializeEvents();
            this.InitializeEventZoneNames();
            this.InitializeNotifications();
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
        /// Initializes the collection of world boss events and meta events
        /// </summary>
        private void InitializeEvents()
        {
            lock (refreshTimerLock)
            {
                logger.Debug("Initializing local event data caches");
                this.eventsService.LoadTables(this.UserData.UseAdjustedTimeTable);

                logger.Debug("Initializing World Boss events");
                Threading.InvokeOnUI(() =>
                {
                    foreach (var worldEvent in this.eventsService.WorldBossEventTimeTable.WorldEvents)
                    {
                        logger.Debug("Loading localized name for {0}", worldEvent.ID);
                        worldEvent.Name = this.eventsService.GetLocalizedName(worldEvent.ID);

                        logger.Debug("Initializing view model for {0}", worldEvent.ID);
                        this.WorldBossEvents.Add(new WorldBossEventViewModel(worldEvent, this.userData));
                    }
                });

                logger.Debug("Initializing Meta Events");
                Threading.InvokeOnUI(() =>
                {
                    foreach (var metaEvent in this.eventsService.MetaEventsTable.MetaEvents)
                    {
                        logger.Debug("Loading localized stage names for {0}", metaEvent.ID);
                        foreach (var stage in metaEvent.Stages)
                        {
                            stage.Name = this.eventsService.GetLocalizedName(stage.ID);
                        }

                        logger.Debug("Initializing view models for {0}", metaEvent.ID);
                        this.MetaEvents.Add(new MetaEventViewModel(metaEvent));
                    }
                });
            }
        }

        /// <summary>
        /// Initialized all event zone/map names
        /// </summary>
        private void InitializeEventZoneNames()
        {
            this.zoneService.Initialize();
            foreach (var worldEvent in this.eventsService.WorldBossEventTimeTable.WorldEvents)
            {
                logger.Debug("Loading localized zone location for {0}", worldEvent.ID);
                var name = this.zoneService.GetZoneName(worldEvent.MapID);
                Threading.BeginInvokeOnUI(() =>
                {
                    worldEvent.MapName = name;
                });
            }

            foreach (var metaEvent in this.MetaEvents)
            {
                logger.Debug("Loading localized zone location for {0}", metaEvent.MapID);
                var name = this.zoneService.GetZoneName(metaEvent.MapID);
                Threading.BeginInvokeOnUI(() =>
                {
                    metaEvent.MapName = name;
                });
            }
        }

        /// <summary>
        /// Initializes all data for event notifications
        /// </summary>
        private void InitializeNotifications()
        {
            logger.Debug("Initializing World Boss Event Notifications");
            Threading.InvokeOnUI(() =>
            {
                foreach (var worldEvent in this.WorldBossEvents)
                {
                    // If the user data does not contain this event, add it to that collection as well
                    var ens = this.UserData.NotificationSettings.FirstOrDefault(ns => ns.EventID == worldEvent.EventId);
                    if (ens == null)
                    {
                        this.UserData.NotificationSettings.Add(new EventNotificationSettings(worldEvent.EventId)
                        {
                            EventName = worldEvent.EventName
                        });
                    }
                    else
                    {
                        ens.EventName = worldEvent.EventName;
                    }

                    this.armedEventNotifications.Add(worldEvent.EventId, true);
                }
            });

            logger.Debug("Initializing Meta Event Notifications");
            Threading.InvokeOnUI(() =>
            {
                foreach (var metaEvent in this.MetaEvents)
                {
                    // If the user data does not contain this event, add it to that collection as well
                    var ens = this.UserData.NotificationSettings.FirstOrDefault(ns => ns.EventID == metaEvent.EventId);
                    if (ens == null)
                    {
                        this.UserData.NotificationSettings.Add(new EventNotificationSettings(metaEvent.EventId)
                        {
                            EventName = metaEvent.MapName
                        });
                    }
                    else
                    {
                        ens.EventName = metaEvent.MapName;
                    }

                    this.armedEventNotifications.Add(metaEvent.EventId, true);
                }
            });
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
                foreach (var worldEvent in this.WorldBossEvents)
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
                                var notification = new WorldBossEventNotificationViewModel(worldEvent, this.EventNotifications);
                                this.DisplayEventNotification(notification, this.EventNotifications);
                                this.armedEventNotifications[worldEvent.EventId] = false;
                            }
                            else
                            {
                                this.armedEventNotifications[worldEvent.EventId] = true;
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
                        foreach (var worldEvent in WorldBossEvents)
                        {
                            worldEvent.IsTreasureObtained = false;
                        }
                    });
                }

                // Update meta events
                foreach (var metaEvent in this.MetaEvents)
                {
                    // Refresh the timer and stages for the event
                    metaEvent.Update(DateTime.UtcNow.TimeOfDay);

                    // Check to see if we need to display a notification for this event
                    var ens = this.UserData.NotificationSettings.FirstOrDefault(ns => ns.EventID == metaEvent.EventId);
                    if (ens != null)
                    {
                        if (ens.IsNotificationEnabled
                            && metaEvent.TimeUntilNextStage.CompareTo(ens.NotificationInterval) < 0)
                        {
                            var notification = new MetaEventNotificationViewModel(metaEvent, this.EventNotifications);
                            this.DisplayEventNotification(notification, this.EventNotifications);
                            this.armedEventNotifications[metaEvent.EventId] = false;
                        }
                        else
                        {
                            this.armedEventNotifications[metaEvent.EventId] = true;
                        }
                    }
                }

                this.eventRefreshTimer.Change(this.EventRefreshInterval, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Displays an event notification, and then removes the event some time later (user-configurable)
        /// </summary>
        private void DisplayEventNotification(IEventNotification notification, ICollection<IEventNotification> notificationCollection)
        {
            const int SLEEP_TIME = 250;

            if (this.UserData.AreEventNotificationsEnabled
                && (this.armedEventNotifications[notification.EventId] == true)
                && !notificationCollection.Any((n) => n.EventId == notification.EventId))
            {
                Task.Factory.StartNew(() =>
                {
                    logger.Info("Displaying notification for \"{0}\"", notification.EventName);
                    Threading.BeginInvokeOnUI(() =>
                    {
                        notificationCollection.Add(notification);
                    });

                    if (this.UserData.NotificationDuration > 0)
                    {
                        // For X seconds, loop and sleep, with checks to see if notifications have been disabled
                        for (int i = 0; i < (this.UserData.NotificationDuration * 1000 / SLEEP_TIME); i++)
                        {
                            System.Threading.Thread.Sleep(SLEEP_TIME);
                            if (!this.UserData.AreEventNotificationsEnabled)
                            {
                                logger.Debug("Removing notification for \"{0}\"", notification.EventName);
                                Threading.BeginInvokeOnUI(() => notificationCollection.Remove(notification));
                            }
                        }

                        logger.Debug("Removing notification for \"{0}\"", notification.EventName);

                        // TODO: I hate having this here, but due to a limitation in WPF, there's no reasonable way around this at this time
                        // This makes it so that the notifications can fade out before they are removed from the notification window
                        Threading.BeginInvokeOnUI(() => notification.IsRemovingNotification = true);
                        System.Threading.Thread.Sleep(SLEEP_TIME);
                        Threading.BeginInvokeOnUI(() =>
                        {
                            notificationCollection.Remove(notification);
                        });
                    }
                });
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
                        this.eventsService.LoadTables(this.UserData.UseAdjustedTimeTable);
                        Threading.InvokeOnUI(() =>
                        {
                            foreach (var worldBossEvent in this.WorldBossEvents)
                            {
                                var newData = this.eventsService.WorldBossEventTimeTable.WorldEvents.FirstOrDefault(evt => evt.ID == worldBossEvent.EventId);
                                worldBossEvent.EventModel.ActiveTimes = newData.ActiveTimes;
                                worldBossEvent.EventModel.Duration = newData.Duration;
                                worldBossEvent.EventModel.WarmupDuration = newData.WarmupDuration;
                            }
                        });
                    }
                });
            }
        }
    }
}
