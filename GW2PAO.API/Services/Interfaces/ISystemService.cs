using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;

namespace GW2PAO.API.Services.Interfaces
{
    public interface ISystemService
    {
        /// <summary>
        /// The current primary screen's resolution
        /// </summary>
        Resolution ScreenResolution { get; }

        /// <summary>
        /// The center point of the primary screen
        /// </summary>
        Point ScreenCenter { get; }

        /// <summary>
        /// Retrieves the current GW2 Process, if running.
        /// If not running, returns NULL
        /// </summary>
        Process Gw2Process { get; }

        /// <summary>
        /// True if the GW2 Process is running, else false
        /// </summary>
        bool IsGw2Running { get; }

        /// <summary>
        /// True if the GW2 Process is the focused process, else false
        /// </summary>
        bool Gw2HasFocus { get; }

        /// <summary>
        /// True if the current application (GW2 PAO) Process is the focused process, else false
        /// </summary>
        bool MyAppHasFocus { get; }
    }
}
