using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.Models;
using GW2PAO.ViewModels;
using GW2PAO.ViewModels.EventTracker;

namespace GW2PAO.Controllers.Interfaces
{
    /// <summary>
    /// Event tracker controller interface.
    /// Defines primary public functionality of the event tracker controller.
    /// </summary>
    public interface IEventTrackerController
    {
        /// <summary>
        /// The collection of World Events
        /// </summary>
        ObservableCollection<EventViewModel> WorldEvents { get; }

        /// <summary>
        /// The interval by which to refresh events (in ms)
        /// </summary>
        int EventRefreshInterval { get; set; }

        /// <summary>
        /// The event tracker user settings
        /// </summary>
        EventTrackerSettings UserSettings { get; }

        /// <summary>
        /// Starts the controller
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the controller
        /// </summary>
        void Stop();
    }
}
