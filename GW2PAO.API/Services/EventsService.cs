using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2DotNET;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;
using GW2PAO.API.Providers;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.API.Util;
using NLog;

namespace GW2PAO.API.Services
{
    /// <summary>
    /// Service class for event information
    /// </summary>
    [Export(typeof(IEventsService))]
    public class EventsService : IEventsService
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The GW2.NET API service objective
        /// </summary>
        private ServiceManager service = new ServiceManager();

        /// <summary>
        /// String provider for world event names
        /// </summary>
        private IStringProvider<Guid> worldEventStringProvider;

        /// <summary>
        /// Helper class for retrieving the current system time
        /// </summary>
        private ITimeProvider timeProvider;

        /// <summary>
        /// Default constructor
        /// </summary>
        public EventsService()
        {
            this.worldEventStringProvider = new WorldEventNamesProvider();
            this.timeProvider = new DefaultTimeProvider();
        }

        /// <summary>
        /// Alternate constructor
        /// </summary>
        /// <param name="currentTimeProvider">A time provider for determining the current name. If null, the EventsServer will use the DefaultTimeProvider</param>
        public EventsService(IStringProvider<Guid> worldEventNamesProvider, ITimeProvider currentTimeProvider)
        {
            this.worldEventStringProvider = worldEventNamesProvider;
            this.timeProvider = currentTimeProvider;
        }

        /// <summary>
        /// The World Events time table
        /// </summary>
        public MegaserverEventTimeTable EventTimeTable { get; private set; }

        /// <summary>
        /// Loads the events time table and initializes all cached event information
        /// </summary>
        public void LoadTable(bool isAdjustedTable)
        {
            logger.Info("Loading Event Time Table");
            try
            {
                this.EventTimeTable = MegaserverEventTimeTable.LoadTable(isAdjustedTable);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Info("Error loading Event Time Table, re-creating table");
                
                MegaserverEventTimeTable.CreateTable(isAdjustedTable);
                this.EventTimeTable = MegaserverEventTimeTable.LoadTable(isAdjustedTable);
            }
        }

        /// <summary>
        /// Returns the localized name for the given event
        /// </summary>
        /// <param name="id">ID of the event to return the name of</param>
        /// <returns>The localized name</returns>
        public string GetLocalizedName(Guid id)
        {
            string evtName = this.worldEventStringProvider.GetString(id);
            if (string.IsNullOrEmpty(evtName))
            {
                var allNames = this.service.GetDynamicEventNames(CultureInfo.CurrentUICulture);
                return allNames[id].Name;
            }
            return evtName;
        }

        /// <summary>
        /// Retrieves the current state of the given event
        /// </summary>
        /// <param name="id">The ID of the event to retrieve the state of</param>
        /// <returns>The current state of the input event</returns>
        public Data.Enums.EventState GetState(Guid id)
        {
            if (this.EventTimeTable.WorldEvents.Any(evt => evt.ID == id))
            {
                WorldEvent worldEvent = this.EventTimeTable.WorldEvents.First(evt => evt.ID == id);
                return this.GetState(worldEvent);
            }
            else
            {
                return Data.Enums.EventState.Unknown;
            }
        }

        /// <summary>
        /// Retrieves the current state of the given event
        /// </summary>
        /// <param name="evt">The event to retrieve the state of</param>
        /// <returns>The current state of the input event</returns>
        public Data.Enums.EventState GetState(WorldEvent evt)
        {
            var state = Data.Enums.EventState.Unknown;
            if (evt != null)
            {
                var timeUntilActive = this.GetTimeUntilActive(evt);
                var timeSinceActive = this.GetTimeSinceActive(evt);

                if (timeSinceActive >= TimeSpan.FromTicks(0)
                    && timeSinceActive < evt.Duration.Time)
                {
                    state = Data.Enums.EventState.Active;
                }
                else if (timeUntilActive >= TimeSpan.FromSeconds(0)
                            && timeUntilActive < evt.WarmupDuration.Time)
                {
                    state = Data.Enums.EventState.Warmup;
                }
                else
                {
                    state = Data.Enums.EventState.Inactive;
                }
            }

            return state;
        }

        /// <summary>
        /// Retrieves the amount of time until the next active time for the given event, using the megaserver timetables
        /// </summary>
        /// <param name="evt">The event to retrieve the time for</param>
        /// <returns>Timespan containing the amount of time until the event is next active</returns>
        public TimeSpan GetTimeUntilActive(WorldEvent evt)
        {
            TimeSpan timeUntilActive = TimeSpan.MinValue;
            if (evt != null)
            {
                // Find the next time
                var nextTime = evt.ActiveTimes.FirstOrDefault(activeTime => (activeTime.Time - this.timeProvider.CurrentTime.TimeOfDay) >= TimeSpan.FromSeconds(0));

                // If there is no next time, then take the first time
                if (nextTime == null)
                {
                    nextTime = evt.ActiveTimes.FirstOrDefault();
                    if (nextTime != null)
                        timeUntilActive = (nextTime.Time + TimeSpan.FromHours(24) - this.timeProvider.CurrentTime.TimeOfDay);
                }
                else
                {
                    // Calculate the number of seconds until the next time
                    timeUntilActive = nextTime.Time - this.timeProvider.CurrentTime.TimeOfDay;
                }
            }
            return timeUntilActive;
        }

        /// <summary>
        /// Retrieves the amount of time since the last active time for the given event, using the megaserver timetables
        /// </summary>
        /// <param name="evt">The event to retrieve the time for</param>
        /// <returns>Timespan containing the amount of time since the event was last active</returns>
        public TimeSpan GetTimeSinceActive(WorldEvent evt)
        {
            TimeSpan timeSinceActive = TimeSpan.MinValue;
            if (evt != null)
            {
                // Find the next time
                var lastTime = evt.ActiveTimes.LastOrDefault(activeTime => (this.timeProvider.CurrentTime.TimeOfDay - activeTime.Time) >= TimeSpan.FromSeconds(0));

                // If there is no next time, then take the first time
                if (lastTime == null)
                {
                    lastTime = evt.ActiveTimes.FirstOrDefault();
                    if (lastTime != null)
                        timeSinceActive = (this.timeProvider.CurrentTime.TimeOfDay - lastTime.Time) + TimeSpan.FromHours(24);
                }
                else
                {
                    // Calculate the number of seconds until the next time
                    timeSinceActive = this.timeProvider.CurrentTime.TimeOfDay - lastTime.Time;
                }
            }
            return timeSinceActive;
        }
    }
}
