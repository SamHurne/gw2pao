using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Enums;
using GW2PAO.Models;
using GW2PAO.ViewModels;

namespace GW2PAO.Controllers.Interfaces
{
    public interface IWvWController
    {
        /// <summary>
        /// Map with which to override the player map.
        /// To disable the override, set this to Unknown
        /// </summary>
        WvWMap MapOverride { get; set; }

        /// <summary>
        /// The interval by which to refresh the objectives state
        /// </summary>
        int ObjectivesRefreshInterval { get; set; }

        /// <summary>
        /// The WvW user settings
        /// </summary>
        WvWSettings UserSettings { get; }

        /// <summary>
        /// The collection of WvW Teams
        /// </summary>
        ObservableCollection<WvWTeamViewModel> Teams { get; }

        /// <summary>
        /// The collection of all WvW Objectives
        /// </summary>
        ObservableCollection<WvWObjectiveViewModel> AllObjectives { get; }

        /// <summary>
        /// The collection of current WvW Objectives
        /// </summary>
        ObservableCollection<WvWObjectiveViewModel> CurrentObjectives { get; }

        /// <summary>
        /// The collection of WvW Objective Notifications
        /// </summary>
        ObservableCollection<WvWObjectiveViewModel> WvWNotifications { get; }

        /// <summary>
        /// Starts the automatic refresh
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the automatic refresh
        /// </summary>
        void Stop();
    }
}
