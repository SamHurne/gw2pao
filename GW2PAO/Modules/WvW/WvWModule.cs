using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.WvW.Interfaces;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using NLog;

namespace GW2PAO.Modules.WvW
{
    [ModuleExport(typeof(WvWModule))]
    public class WvWModule : IModule
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
        /// WvW controller
        /// </summary>
        private IWvWController wvwController;

        /// <summary>
        /// Factory object responsible for displaying views
        /// </summary>
        private IWvWViewController viewController;

        /// <summary>
        /// The wvw user settings and data
        /// </summary>
        private WvWUserData userData;

        /// <summary>
        /// The dungeons user settings and data
        /// </summary>
        [Export(typeof(WvWUserData))]
        public WvWUserData UserData
        {
            get
            {
                if (this.userData == null)
                {
                    logger.Debug("Loading wvw user data");
                    this.userData = WvWUserData.LoadData(WvWUserData.Filename);
                    if (this.userData == null)
                        this.userData = new WvWUserData();
                }

                return this.userData;
            }
        }

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing WvW Module");

            this.wvwController = this.Container.GetExportedValue<IWvWController>();
            this.viewController = this.Container.GetExportedValue<IWvWViewController>();

            // Register for shutdown
            Commands.ApplicationShutdownCommand.RegisterCommand(new DelegateCommand(this.Shutdown));

            // Get the WvW controller started
            this.wvwController.Start();

            // Initialize the view controller
            this.viewController.Initialize();
        }

        /// <summary>
        /// Performs all neccesary shutdown activities for this module
        /// </summary>
        private void Shutdown()
        {
            logger.Debug("Shutting down WvW Module");

            // Shut down the WvW controller
            this.wvwController.Shutdown();

            // Shutdown the view controller
            this.viewController.Shutdown();

            // Make sure we have saved all user data
            // Note that this is a little redundant given the AutoSave feature,
            // but it does help to make sure the user's data is really saved
            WvWUserData.SaveData(this.UserData, WvWUserData.Filename);
        }
    }
}
