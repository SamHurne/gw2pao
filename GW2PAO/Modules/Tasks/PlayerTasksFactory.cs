using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Modules.Tasks.Interfaces;
using GW2PAO.Modules.Tasks.Models;
using GW2PAO.Modules.Tasks.ViewModels;
using NLog;

namespace GW2PAO.Modules.Tasks
{
    [Export(typeof(PlayerTasksFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class PlayerTasksFactory
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IZoneService zoneService;
        private IPlayerTasksController tasksController;
        private CompositionContainer container;


        [ImportingConstructor]
        public PlayerTasksFactory(IZoneService zoneService, IPlayerTasksController controller, CompositionContainer container)
        {
            this.zoneService = zoneService;
            this.tasksController = controller;
            this.container = container;
        }

        public PlayerTask GetPlayerTask()
        {
            return new PlayerTask();
        }

        public PlayerTaskViewModel GetPlayerTaskWithViewModel()
        {
            return new PlayerTaskViewModel(new PlayerTask(), this.zoneService, this.tasksController, this.container);
        }

        public PlayerTaskViewModel GetPlayerTaskViewModel(PlayerTask task)
        {
            return new PlayerTaskViewModel(task, this.zoneService, this.tasksController, this.container);
        }
    }
}
