using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Models;
using GW2PAO.ViewModels.Interfaces;
using GW2PAO.ViewModels.ZoneCompletion;

namespace GW2PAO.Controllers.Interfaces
{
    /// <summary>
    /// Zone completion controller interface.
    /// Defines primary public functionality of the zone completion controller.
    /// </summary>
    public interface IZoneCompletionController
    {
        /// <summary>
        /// The collection of zone points in the current zone
        /// </summary>
        ObservableCollection<ZoneItemViewModel> ZoneItems { get; }

        /// <summary>
        /// The zone completion user settings
        /// </summary>
        ZoneCompletionSettings UserSettings { get; }

        /// <summary>
        /// The interval by which to refresh zone information (in ms)
        /// </summary>
        int ZoneRefreshInterval { get; set; }

        /// <summary>
        /// The interval by which to refresh zone point locations (in ms)
        /// </summary>
        int LocationsRefreshInterval { get; set; }

        /// <summary>
        /// The ID of the current map/zone
        /// </summary>
        int CurrentMapID { get; }

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
