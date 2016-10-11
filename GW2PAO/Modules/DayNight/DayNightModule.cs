using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.DayNight.Interfaces;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using NLog;

namespace GW2PAO.Modules.DayNight
{
    [ModuleExport(typeof(DayNightModule))]
    public class DayNightModule : IModule
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Composition container of composed parts
        /// </summary>
        [Import]
        private CompositionContainer container { get; set; }

        /// <summary>
        /// Factory object responsible for displaying views
        /// </summary>
        private IDayNightViewController viewController;

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing Day-Night Timer Module");

            this.viewController = this.container.GetExportedValue<IDayNightViewController>();

            // Register for shutdown
            Commands.ApplicationShutdownCommand.RegisterCommand(new DelegateCommand(this.Shutdown));

            // Initialize the view controller
            this.viewController.Initialize();

            logger.Debug("Day-Night Timer Module initialized");
        }

        /// <summary>
        /// Performs all neccesary shutdown activities for this module
        /// </summary>
        private void Shutdown()
        {
            logger.Debug("Shutting down Day-Night Timer Module");

            // Shutdown the view controller
            this.viewController.Shutdown();
        }
    }
}
