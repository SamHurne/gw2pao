using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Models;
using GW2PAO.ViewModels.DungeonTracker;

namespace GW2PAO.Controllers.Interfaces
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
        /// The event tracker user settings
        /// </summary>
        DungeonSettings UserSettings { get; }

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
