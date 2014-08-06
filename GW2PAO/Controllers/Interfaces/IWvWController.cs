using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Models;
using GW2PAO.ViewModels;

namespace GW2PAO.Controllers.Interfaces
{
    public interface IWvWController
    {
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
        /// The collection of All WvW Objectives
        /// </summary>
        ObservableCollection<WvWObjectiveViewModel> AllObjectives { get; }

        /// <summary>
        /// The collection of Blue Borderlands WvW Objectives
        /// </summary>
        ObservableCollection<WvWObjectiveViewModel> BlueBorderlandsObjectives { get; }

        /// <summary>
        /// The collection of Green Borderlands WvW Objectives
        /// </summary>
        ObservableCollection<WvWObjectiveViewModel> GreenBorderlandsObjectives { get; }

        /// <summary>
        /// The collection of Red Borderlands WvW Objectives
        /// </summary>
        ObservableCollection<WvWObjectiveViewModel> RedBorderlandsObjectives { get; }

        /// <summary>
        /// The collection of Eternal Battlegrounds WvW Objectives
        /// </summary>
        ObservableCollection<WvWObjectiveViewModel> EternalBattlegroundsObjectives { get; }

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
