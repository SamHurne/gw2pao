using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.Data;
using GW2PAO.Data.UserData;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.ViewModels.DungeonTracker
{
    public class DungeonTrackerViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The Dungeons Tracker controller
        /// </summary>
        private IDungeonsController controller;

        /// <summary>
        /// Collection of all Dungeons
        /// </summary>
        public ObservableCollection<DungeonViewModel> Dungeons { get { return this.controller.Dungeons; } }

        /// <summary>
        /// Command to reset all hidden dungeons
        /// </summary>
        public DelegateCommand ResetHiddenDungeonsCommand { get { return new DelegateCommand(this.ResetHiddenDungeons); } }

        /// <summary>
        /// Command to reset all completed dungeon paths
        /// </summary>
        public DelegateCommand ResetCompletedPathsCommand { get { return new DelegateCommand(this.ResetCompletedPaths); } }

        /// <summary>
        /// Dungeon user settings
        /// </summary>
        public DungeonUserData UserData { get { return this.controller.UserData; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dungeonsController">The dungeons controller</param>
        public DungeonTrackerViewModel(IDungeonsController dungeonsController)
        {
            this.controller = dungeonsController;
        }

        /// <summary>
        /// Resets all hidden dungeons
        /// </summary>
        private void ResetHiddenDungeons()
        {
            logger.Debug("Resetting hidden dungeons");
            this.UserData.HiddenDungeons.Clear();
        }

        /// <summary>
        /// Resets all completed dungeon paths
        /// </summary>
        private void ResetCompletedPaths()
        {
            logger.Debug("Resetting completed paths");
            foreach (var dungeon in this.Dungeons)
            {
                foreach (var path in dungeon.Paths)
                {
                    path.IsCompleted = false;
                }
            }
        }
    }
}
