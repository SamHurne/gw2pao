using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;
using GW2PAO.Modules.Events;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2PAO.Modules.Events.ViewModels
{
    /// <summary>
    /// View model for an event shown by the event tracker
    /// </summary>
    public class EventViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private EventState state;
        private TimeSpan timeSinceActive;
        private TimeSpan timerValue;
        private bool isVisible;
        private bool isNotificationShown;
        private bool isRemovingNotification;
        private ICollection<EventViewModel> displayedNotifications;
        private EventsUserData userData;

        /// <summary>
        /// The primary model object containing the event information
        /// </summary>
        public WorldEvent EventModel { get; private set; }

        /// <summary>
        /// The event's ID
        /// </summary>
        public Guid EventId { get { return this.EventModel.ID; } }

        /// <summary>
        /// The event's name
        /// </summary>
        public string EventName { get { return this.EventModel.Name; } }

        /// <summary>
        /// Name of the zone in which the event occurs
        /// </summary>
        public string ZoneName { get { return this.EventModel.MapName; } }

        /// <summary>
        /// Current state of the event
        /// </summary>
        public EventState State
        {
            get { return this.state; }
            set { if (SetProperty(ref this.state, value)) this.RefreshVisibility(); }
        }

        /// <summary>
        /// Depending on the state of the event, contains the
        /// 'Time Until Active' or the 'Time Since Active'
        /// </summary>
        public TimeSpan TimerValue
        {
            get { return this.timerValue; }
            set { SetProperty(ref this.timerValue, value); }
        }

        /// <summary>
        /// Time since the event was last active
        /// </summary>
        public TimeSpan TimeSinceActive
        {
            get { return this.timeSinceActive; }
            set { SetProperty(ref this.timeSinceActive, value); }
        }

        /// <summary>
        /// Visibility of the event
        /// Visibility is based on multiple properties, including:
        ///     - EventState and the user configuration for what states are shown
        ///     - IsTreasureObtained and whether or not treasure-obtained events are shown
        ///     - Whether or not the event is user-configured as hidden
        /// </summary>
        public bool IsVisible
        {
            get { return this.isVisible; }
            set { SetProperty(ref this.isVisible, value); }
        }

        /// <summary>
        /// Daily treasure obtained state
        /// Resets at UTC midnight
        /// </summary>
        public bool IsTreasureObtained
        {
            get { return this.userData.EventsWithTreasureObtained.Contains(this.EventModel.ID); }
            set
            {
                if (value && !this.userData.EventsWithTreasureObtained.Contains(this.EventModel.ID))
                {
                    logger.Debug("Adding \"{0}\" to EventsWithTreasureObtained", this.EventName);
                    this.userData.EventsWithTreasureObtained.Add(this.EventModel.ID);
                    this.OnPropertyChanged(() => this.IsTreasureObtained);
                }
                else
                {
                    logger.Debug("Removing \"{0}\" from EventsWithTreasureObtained", this.EventName);
                    if (this.userData.EventsWithTreasureObtained.Remove(this.EventModel.ID))
                        this.OnPropertyChanged(() => this.IsTreasureObtained);
                }
            }
        }

        /// <summary>
        /// True if the notification for this event has already been shown, else false
        /// </summary>
        public bool IsNotificationShown
        {
            get { return this.isNotificationShown; }
            set { SetProperty(ref this.isNotificationShown, value); }
        }

        /// <summary>
        /// True if the notification for this event is about to be removed, else false
        /// TODO: I hate having this here, but due to a limitation in WPF, there's no reasonable way around this at this time
        /// </summary>
        public bool IsRemovingNotification
        {
            get { return this.isRemovingNotification; }
            set { SetProperty(ref this.isRemovingNotification, value); }
        }

        /// <summary>
        /// Command to hide the event
        /// </summary>
        public DelegateCommand HideCommand { get { return new DelegateCommand(this.AddToHiddenEvents); } }

        /// <summary>
        /// Command to copy the nearest waypoint's chat code to the clipboard
        /// </summary>
        public DelegateCommand CopyWaypointCommand { get { return new DelegateCommand(this.CopyWaypointCode); } }

        /// <summary>
        /// Command to copy the information about the event to the clipboard
        /// </summary>
        public DelegateCommand CopyDataCommand { get { return new DelegateCommand(this.CopyEventData); } }

        /// <summary>
        /// Closes the displayed notification
        /// </summary>
        public DelegateCommand CloseNotificationCommand { get { return new DelegateCommand(this.CloseNotification); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="eventData">The event's details/data</param>
        /// <param name="userData">Event tracker user data</param>
        /// <param name="displayedNotificationsCollection">Collection of displayed event notifications</param>
        public EventViewModel(WorldEvent eventData, EventsUserData userData, ICollection<EventViewModel> displayedNotificationsCollection)
        {
            this.EventModel = eventData;
            this.userData = userData;
            this.displayedNotifications = displayedNotificationsCollection;
            this.IsVisible = true;
            this.IsNotificationShown = false;
            this.IsRemovingNotification = false;

            this.State = EventState.Unknown;
            this.TimerValue = TimeSpan.Zero;
            this.userData.PropertyChanged += (o, e) => this.RefreshVisibility();
            this.userData.HiddenEvents.CollectionChanged += (o, e) => this.RefreshVisibility();
        }

        /// <summary>
        /// Adds the event to the list of hidden events
        /// </summary>
        private void AddToHiddenEvents()
        {
            logger.Debug("Adding \"{0}\" to hidden events", this.EventName);
            this.userData.HiddenEvents.Add(this.EventModel.ID);
        }

        /// <summary>
        /// Refreshes the visibility of the event
        /// </summary>
        private void RefreshVisibility()
        {
            logger.Trace("Refreshing visibility of \"{0}\"", this.EventName);
            if (this.userData.HiddenEvents.Any(id => id == this.EventId))
            {
                this.IsVisible = false;
            }
            else if (!this.userData.AreInactiveEventsVisible
                    && this.State == EventState.Inactive)
            {
                this.IsVisible = false;
            }
            else
            {
                this.IsVisible = true;
            }
            logger.Trace("IsVisible = {0}", this.IsVisible);
        }

        /// <summary>
        /// Copies the nearest waypoint's chat code to the clipboard
        /// </summary>
        private void CopyWaypointCode()
        {
            logger.Debug("Copying waypoint code of \"{0}\" as \"{1}\"", this.EventName, this.EventModel.WaypointCode);
            System.Windows.Clipboard.SetText(this.EventModel.WaypointCode);
        }

        /// <summary>
        /// Removes this event from the collection of displayed notifications
        /// </summary>
        private void CloseNotification()
        {
            this.displayedNotifications.Remove(this);
        }

        /// <summary>
        /// Places a string of data on the clipboard for pasting into the game
        /// Contains the event name, status, time until active, waypoint code, etc
        /// </summary>
        private void CopyEventData()
        {
            string fullText;
            if (this.State == EventState.Active)
            {
                fullText = string.Format("{0} - {1}",
                    this.EventName,
                    this.EventModel.WaypointCode);
            }
            else
            {
                fullText = string.Format("{0} - {1} {2} - {3}",
                    this.EventName,
                    GW2PAO.Properties.Resources.ActiveIn, this.TimerValue.ToString("hh\\:mm\\:ss"),
                    this.EventModel.WaypointCode);
            }

            logger.Debug("Copying \"{0}\" to clipboard", fullText);
            System.Windows.Clipboard.SetText(fullText);
        }

    }
}
