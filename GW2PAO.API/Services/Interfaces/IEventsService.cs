using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.API.Services.Interfaces
{
    public interface IEventsService
    {
        /// <summary>
        /// The World Events time table
        /// </summary>
        MegaserverEventTimeTable EventTimeTable { get; }

        /// <summary>
        /// Loads the events time table and initializes all cached event information
        /// </summary>
        void LoadTable(bool isAdjustedTable);

        /// <summary>
        /// Returns the localized name for the given event
        /// </summary>
        /// <param name="id">ID of the event to return the name of</param>
        /// <returns>The localized name</returns>
        string GetLocalizedName(Guid id);

        /// <summary>
        /// Retrieves the current state of the given event
        /// </summary>
        /// <param name="id">The ID of the event to retrieve the state of</param>
        /// <returns>The current state of the input event</returns>
        Data.Enums.EventState GetState(Guid id);

        /// <summary>
        /// Retrieves the current state of the given event
        /// </summary>
        /// <param name="evt">The event to retrieve the state of</param>
        /// <returns>The current state of the input event</returns>
        Data.Enums.EventState GetState(WorldEvent evt);

        /// <summary>
        /// Retrieves the amount of time until the next active time for the given event, using the megaserver timetables
        /// </summary>
        /// <param name="evt">The event to retrieve the time for</param>
        /// <returns>Timespan containing the amount of time until the event is next active</returns>
        TimeSpan GetTimeUntilActive(WorldEvent evt);

        /// <summary>
        /// Retrieves the amount of time since the last active time for the given event, using the megaserver timetables
        /// </summary>
        /// <param name="evt">The event to retrieve the time for</param>
        /// <returns>Timespan containing the amount of time since the event was last active</returns>
        TimeSpan GetTimeSinceActive(WorldEvent evt);
    }
}
