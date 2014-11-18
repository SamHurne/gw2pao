using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.ZoneCompletion.Interfaces;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using NLog;

namespace GW2PAO.Modules.ZoneCompletion
{
    [ModuleExport(typeof(ZoneCompletionModule))]
    public class ZoneCompletionModule : IModule
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
        /// Zone completion controller
        /// </summary>
        private IZoneCompletionController zoneCompletionController;

        /// <summary>
        /// Controller object responsible for displaying views
        /// </summary>
        private IZoneCompletionViewController viewController;

        /// <summary>
        /// The zone completion user settings and data
        /// </summary>
        private ZoneCompletionUserData userData;

        /// <summary>
        /// The dungeons user settings and data
        /// </summary>
        [Export(typeof(ZoneCompletionUserData))]
        public ZoneCompletionUserData UserData
        {
            get
            {
                if (this.userData == null)
                {
                    logger.Debug("Loading zone completion user data");
                    this.userData = ZoneCompletionUserData.LoadData(ZoneCompletionUserData.Filename);
                    if (this.userData == null)
                        this.userData = new ZoneCompletionUserData();
                }

                return this.userData;
            }
        }

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            Task.Factory.StartNew(() =>
                {
                    logger.Debug("Initializing Zone Completion Module");

                    this.zoneCompletionController = this.Container.GetExportedValue<IZoneCompletionController>();
                    this.viewController = this.Container.GetExportedValue<IZoneCompletionViewController>();

                    // Register for shutdown
                    Commands.ApplicationShutdownCommand.RegisterCommand(new DelegateCommand(this.Shutdown));

                    // Initialize the view controller
                    this.viewController.Initialize();

                    logger.Debug("Zone Completion Module initialized");
                });
        }

        /// <summary>
        /// Performs all neccesary shutdown activities for this module
        /// </summary>
        private void Shutdown()
        {
            logger.Debug("Shutting down Commerce Module");

            // Shut down the zone copmletion controller
            this.zoneCompletionController.Shutdown();

            // Shutdown the view controller
            this.viewController.Shutdown();

            // Make sure we have saved all user data
            // Note that this is a little redundant given the AutoSave feature,
            // but it does help to make sure the user's data is really saved
            ZoneCompletionUserData.SaveData(this.UserData, ZoneCompletionUserData.Filename);
        }
    }
}
