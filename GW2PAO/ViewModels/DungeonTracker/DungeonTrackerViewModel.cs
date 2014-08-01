using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.Models;
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
        /// The Event Tracker controller
        /// </summary>
        private IDungeonsController controller;

        /// <summary>
        /// Collection of all World Events
        /// </summary>
        public ObservableCollection<DungeonViewModel> Dungeons { get { return this.controller.Dungeons; } }

        /// <summary>
        /// Command to reset all hidden events
        /// </summary>
        public DelegateCommand ResetHiddenDungeonsCommand { get { return new DelegateCommand(this.ResetHiddenDungeons); } }

        /// <summary>
        /// Command to reset all hidden events
        /// </summary>
        public DelegateCommand ResetCompletedPathsCommand { get { return new DelegateCommand(this.ResetCompletedPaths); } }

        /// <summary>
        /// Event Tracker user settings
        /// </summary>
        public DungeonSettings UserSettings { get { return this.controller.UserSettings; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dungeonsController">The dungeons controller</param>
        public DungeonTrackerViewModel(IDungeonsController dungeonsController)
        {
            this.controller = dungeonsController;
        }

        /// <summary>
        /// Resets all hidden events
        /// </summary>
        private void ResetHiddenDungeons()
        {
            logger.Debug("Resetting hidden dungeons");
            this.UserSettings.HiddenDungeons.Clear();
        }

        /// <summary>
        /// Resets all hidden events
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
