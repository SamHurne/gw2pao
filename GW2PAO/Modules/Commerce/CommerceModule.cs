using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.Commerce.Interfaces;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using NLog;

namespace GW2PAO.Modules.Commerce
{
    [ModuleExport(typeof(CommerceModule))]
    public class CommerceModule : IModule
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Composition container of composed parts
        /// </summary>
        [Import]
        private CompositionContainer container { get; set; }

        /// <summary>
        /// Commerce controller
        /// </summary>
        private ICommerceController commerceController;

        /// <summary>
        /// Controller object responsible for displaying views
        /// </summary>
        private ICommerceViewController viewController;

        /// <summary>
        /// The dungeons user settings and data
        /// </summary>
        private CommerceUserData userData;

        /// <summary>
        /// The dungeons user settings and data
        /// </summary>
        [Export(typeof(CommerceUserData))]
        public CommerceUserData UserData
        {
            get
            {
                if (this.userData == null)
                {
                    logger.Debug("Loading commerce user data");
                    this.userData = CommerceUserData.LoadData(CommerceUserData.Filename);
                    if (this.userData == null)
                        this.userData = new CommerceUserData();
                    this.userData.EnableAutoSave();
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
                    logger.Debug("Initializing Commerce Module");

                    this.commerceController = this.container.GetExportedValue<ICommerceController>();
                    this.viewController = this.container.GetExportedValue<ICommerceViewController>();

                    // Register for shutdown
                    Commands.ApplicationShutdownCommand.RegisterCommand(new DelegateCommand(this.Shutdown));

                    // Get the commerce controller started
                    this.commerceController.Start();

                    // Initialize the view controller
                    this.viewController.Initialize();

                    logger.Debug("Commerce Module initialized");
                });
        }

        /// <summary>
        /// Performs all neccesary shutdown activities for this module
        /// </summary>
        private void Shutdown()
        {
            logger.Debug("Shutting down Commerce Module");

            // Shut down the commerce controller
            this.commerceController.Shutdown();

            // Shutdown the view controller
            this.viewController.Shutdown();

            // Make sure we have saved all user data
            // Note that this is a little redundant given the AutoSave feature,
            // but it does help to make sure the user's data is really saved
            CommerceUserData.SaveData(this.UserData, CommerceUserData.Filename);
        }
    }
}
