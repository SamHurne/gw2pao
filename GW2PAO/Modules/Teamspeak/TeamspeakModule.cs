using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.Teamspeak.Interfaces;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using NLog;

namespace GW2PAO.Modules.Teamspeak
{
    [ModuleExport(typeof(TeamspeakModule))]
    public class TeamspeakModule : IModule
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
        /// The teamspeak user settings and data
        /// </summary>
        private TeamspeakUserData userData;

        /// <summary>
        /// Factory object responsible for displaying views
        /// </summary>
        private ITeamspeakViewController viewController;

        /// <summary>
        /// The teamspeak user settings and data
        /// </summary>
        [Export(typeof(TeamspeakUserData))]
        public TeamspeakUserData UserData
        {
            get
            {
                if (this.userData == null)
                {
                    logger.Debug("Loading teamspeak user data");
                    this.userData = TeamspeakUserData.LoadData(TeamspeakUserData.Filename);
                    if (this.userData == null)
                        this.userData = new TeamspeakUserData();
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
            logger.Debug("Initializing Teamspeak Module");

            this.viewController = this.Container.GetExportedValue<ITeamspeakViewController>();

            // Register for shutdown
            Commands.ApplicationShutdownCommand.RegisterCommand(new DelegateCommand(this.Shutdown));

            // Initialize the view controller
            this.viewController.Initialize();

            logger.Debug("Teamspeak Module initialized");
        }

        /// <summary>
        /// Performs all neccesary shutdown activities for this module
        /// </summary>
        private void Shutdown()
        {
            logger.Debug("Shutting down Teamspeak Module");

            // Shutdown the view controller
            this.viewController.Shutdown();

            // Make sure we have saved all user data
            // Note that this is a little redundant given the AutoSave feature,
            // but it does help to make sure the user's data is really saved
            TeamspeakUserData.SaveData(this.UserData, TeamspeakUserData.Filename);
        }
    }
}
