using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.Modules.Events.ViewModels.EventNotification;
using GW2PAO.Modules.Events.ViewModels.MetaEventTimers;
using GW2PAO.Modules.Events.ViewModels.WorldBossTimers;

namespace GW2PAO.Modules.Events.Interfaces
{
    public interface IEventsController
    {
        /// <summary>
        /// The collection of World Events
        /// </summary>
        ObservableCollection<MetaEventViewModel> MetaEvents { get; }

        /// <summary>
        /// The collection of World Events
        /// </summary>
        ObservableCollection<WorldBossEventViewModel> WorldBossEvents { get; }

        /// <summary>
        /// The collection of notifications for world boss and meta event notifications
        /// </summary>
        ObservableCollection<IEventNotification> EventNotifications { get; }

        /// <summary>
        /// The interval by which to refresh events (in ms)
        /// </summary>
        int EventRefreshInterval { get; set; }

        /// <summary>
        /// The event tracker user data
        /// </summary>
        EventsUserData UserData { get; }

        /// <summary>
        /// Starts the controller
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the controller
        /// </summary>
        void Stop();

        /// <summary>
        /// Forces a shutdown of the controller, including all running timers/threads
        /// </summary>
        void Shutdown();
    }
}
