using GW2PAO.Infrastructure;
using GW2PAO.Modules.Map.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using NLog;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace GW2PAO.Modules.Map
{
    [ModuleExport(typeof(MapModule))]
    public class MapModule : IModule
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
        private IMapViewController viewController;

        /// <summary>
        /// The dungeons user settings and data
        /// </summary>
        private MapUserData userData;

        /// <summary>
        /// The map module's user settings and data
        /// </summary>
        [Export(typeof(MapUserData))]
        public MapUserData UserData
        {
            get
            {
                if (this.userData == null)
                {
                    logger.Debug("Loading maps user data");
                    this.userData = MapUserData.LoadData(MapUserData.Filename);
                    if (this.userData == null)
                        this.userData = new MapUserData();
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
            logger.Debug("Initializing Maps Module");

            this.viewController = this.container.GetExportedValue<IMapViewController>();

            // Register for shutdown
            Commands.ApplicationShutdownCommand.RegisterCommand(new DelegateCommand(this.Shutdown));

            // Initialize the view controller
            this.viewController.Initialize();

            logger.Debug("Maps Module initialized");
        }

        /// <summary>
        /// Performs all neccesary shutdown activities for this module
        /// </summary>
        private void Shutdown()
        {
            logger.Debug("Shutting down Events Module");

            // Shutdown the view controller
            this.viewController.Shutdown();

            // Make sure we have saved all user data
            // Note that this is a little redundant given the AutoSave feature,
            // but it does help to make sure the user's data is really saved
            MapUserData.SaveData(this.UserData, MapUserData.Filename);
        }
    }
}
