using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;
using GW2PAO.Data.UserData;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.Events.Interfaces;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using NLog;

namespace GW2PAO.Modules.Events
{
    [ModuleExport(typeof(EventsModule))]
    public class EventsModule : IModule
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
        /// Events controller
        /// </summary>
        private IEventsController eventsController;

        /// <summary>
        /// Factory object responsible for displaying views
        /// </summary>
        private IEventsViewController viewController;

        /// <summary>
        /// The dungeons user settings and data
        /// </summary>
        private EventsUserData userData;

        /// <summary>
        /// The dungeons user settings and data
        /// </summary>
        [Export(typeof(EventsUserData))]
        public EventsUserData UserData
        {
            get
            {
                if (this.userData == null)
                {
                    logger.Debug("Loading events user data");
                    this.userData = EventsUserData.LoadData(EventsUserData.Filename);
                    if (this.userData == null)
                        this.userData = new EventsUserData();
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
                    logger.Debug("Initializing Events Module");

                    this.eventsController = this.container.GetExportedValue<IEventsController>();
                    this.viewController = this.container.GetExportedValue<IEventsViewController>();

                    // Register for shutdown
                    Commands.ApplicationShutdownCommand.RegisterCommand(new DelegateCommand(this.Shutdown));

                    // Get the events controller started
                    this.eventsController.Start();

                    // Initialize the view controller
                    this.viewController.Initialize();

                    logger.Debug("Events Module initialized");
                });
        }

        /// <summary>
        /// Performs all neccesary shutdown activities for this module
        /// </summary>
        private void Shutdown()
        {
            logger.Debug("Shutting down Events Module");

            // Shut down the commerce controller
            this.eventsController.Shutdown();

            // Shutdown the view controller
            this.viewController.Shutdown();

            // Make sure we have saved all user data
            // Note that this is a little redundant given the AutoSave feature,
            // but it does help to make sure the user's data is really saved
            EventsUserData.SaveData(this.UserData, EventsUserData.Filename);
        }
    }
}
