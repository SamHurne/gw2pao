using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Data;
using GW2PAO.Data.UserData;
using GW2PAO.Modules.ZoneCompletion.ViewModels;

namespace GW2PAO.Modules.ZoneCompletion.Interfaces
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
        /// The current character's name
        /// </summary>
        string CharacterName { get; }

        /// <summary>
        /// The zone completion user data
        /// </summary>
        ZoneCompletionUserData UserData { get; }

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

        /// <summary>
        /// Forces a shutdown of the controller, including all running timers/threads
        /// </summary>
        void Shutdown();
    }
}
