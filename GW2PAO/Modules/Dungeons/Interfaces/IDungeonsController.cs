using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Data;
using GW2PAO.Data.UserData;
using GW2PAO.Modules.Dungeons.ViewModels;

namespace GW2PAO.Modules.Dungeons.Interfaces
{
    public interface IDungeonsController
    {
        /// <summary>
        /// The collection of Dungeons
        /// </summary>
        ObservableCollection<DungeonViewModel> Dungeons { get; }

        /// <summary>
        /// The interval by which to refresh events (in ms)
        /// </summary>
        int RefreshInterval { get; set; }

        /// <summary>
        /// The event tracker user data
        /// </summary>
        DungeonsUserData UserData { get; }

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

        /// <summary>
        /// Sets the active path as the path with the given ID
        /// </summary>
        /// <param name="pathId">The ID of the path to set as the active path</param>
        void SetActivePath(Guid pathId);
    }
}
