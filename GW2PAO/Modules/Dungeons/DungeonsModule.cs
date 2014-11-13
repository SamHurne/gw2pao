using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.Dungeons.Interfaces;
using GW2PAO.Modules.Dungeons.Views;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using NLog;

namespace GW2PAO.Modules.Dungeons
{
    [ModuleExport(typeof(DungeonsModule))]
    public class DungeonsModule : IModule
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Composition container of composed parts
        /// </summary>
        [Import]
        private CompositionContainer Container { get; set; }

        /// <summary>
        /// Dungeons controller
        /// </summary>
        private IDungeonsController dungeonsController;

        /// <summary>
        /// Factory object responsible for displaying views
        /// </summary>
        private IDungeonsViewController viewController;

        /// <summary>
        /// The dungeons user settings and data
        /// </summary>
        private DungeonsUserData userData;

        /// <summary>
        /// The dungeons user settings and data
        /// </summary>
        [Export(typeof(DungeonsUserData))]
        public DungeonsUserData UserData
        {
            get
            {
                if (this.userData == null)
                {
                    logger.Debug("Loading dungeon user data");
                    this.userData = DungeonsUserData.LoadData(DungeonsUserData.Filename);
                    if (this.userData == null)
                        this.userData = new DungeonsUserData();
                }

                return this.userData;
            }
        }

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing Dungeons Module");

            this.dungeonsController = this.Container.GetExportedValue<IDungeonsController>();
            this.viewController = this.Container.GetExportedValue<IDungeonsViewController>();

            // Register for shutdown
            Commands.ApplicationShutdownCommand.RegisterCommand(new DelegateCommand(this.Shutdown));

            // Get the dungeons controller started
            this.dungeonsController.Start();

            // Initialize the view controller
            this.viewController.Initialize();
        }

        /// <summary>
        /// Performs all neccesary shutdown activities for this module
        /// </summary>
        private void Shutdown()
        {
            logger.Debug("Shutting down Dungeons Module");

            // Shut down the commerce controller
            this.dungeonsController.Shutdown();

            // Shutdown the view controller
            this.viewController.Shutdown();

            // Make sure we have saved all user data
            // Note that this is a little redundant given the AutoSave feature,
            // but it does help to make sure the user's data is really saved
            DungeonsUserData.SaveData(this.UserData, DungeonsUserData.Filename);
        }
    }
}
