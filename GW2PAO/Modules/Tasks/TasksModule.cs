using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.Tasks.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using NLog;

namespace GW2PAO.Modules.Tasks
{
    [ModuleExport(typeof(TasksModule))]
    public class TasksModule : IModule
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
        /// Player Tasks controller
        /// </summary>
        private IPlayerTasksController playerTasksController;

        /// <summary>
        /// Controller object responsible for displaying views
        /// </summary>
        private IPlayerTasksViewController viewController;

        /// <summary>
        /// The player tasks user settings and data
        /// </summary>
        private TasksUserData userData;

        /// <summary>
        /// The player tasks user settings and data
        /// </summary>
        [Export(typeof(TasksUserData))]
        public TasksUserData UserData
        {
            get
            {
                if (this.userData == null)
                {
                    logger.Debug("Loading player tasks user data");
                    this.userData = TasksUserData.LoadData(TasksUserData.Filename);
                    if (this.userData == null)
                        this.userData = new TasksUserData();
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
            logger.Debug("Initializing Player Tasks Module");

            this.playerTasksController = this.Container.GetExportedValue<IPlayerTasksController>();
            this.viewController = this.Container.GetExportedValue<IPlayerTasksViewController>();

            // Register for shutdown
            Commands.ApplicationShutdownCommand.RegisterCommand(new DelegateCommand(this.Shutdown));

            // Initialize the view controller
            this.viewController.Initialize();

            // Start the controller
            this.playerTasksController.Start();

            logger.Debug("Player Tasks Module initialized");
        }

        /// <summary>
        /// Performs all neccesary shutdown activities for this module
        /// </summary>
        private void Shutdown()
        {
            logger.Debug("Shutting down Player Tasks Module");

            // Shut down the player tasks controller
            this.playerTasksController.Shutdown();

            // Shutdown the view controller
            this.viewController.Shutdown();

            // Make sure we have saved all user data
            // Note that this is a little redundant given the AutoSave feature,
            // but it does help to make sure the user's data is really saved
            TasksUserData.SaveData(this.UserData, TasksUserData.Filename);
        }
    }
}
