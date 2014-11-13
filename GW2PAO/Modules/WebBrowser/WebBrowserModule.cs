using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Awesomium.Core;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.WebBrowser.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using NLog;

namespace GW2PAO.Modules.WebBrowser
{
    [ModuleExport(typeof(WebBrowserModule))]
    public class WebBrowserModule : IModule
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
        /// Web Browser controller
        /// </summary>
        private IWebBrowserController webBrowserController;

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing Web Browser Module");

            this.webBrowserController = this.Container.GetExportedValue<IWebBrowserController>();

            // Register for shutdown
            Commands.ApplicationShutdownCommand.RegisterCommand(new DelegateCommand(this.Shutdown));

#if !NO_BROWSER
            // Initialize the WebCore for the web browser
            logger.Debug("Initializing Awesomium WebCore");
            if (!WebCore.IsInitialized)
            {
                WebCore.Initialize(new WebConfig()
                {
                    HomeURL = "http://wiki.guildwars2.com/".ToUri(),
                });
            }
#endif
        }

        /// <summary>
        /// Performs all neccesary shutdown activities for this module
        /// </summary>
        private void Shutdown()
        {
            logger.Debug("Shutting down Web Browser Module");

            // Close the browser window, if it's open
            this.webBrowserController.CloseBrowser();

#if !NO_BROWSER
            logger.Debug("Cleaning up Awesomium WebCore");
            if (WebCore.IsInitialized)
            {
                WebCore.Shutdown();
            }
#endif
        }
    }
}
