using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GW2PAO.Infrastructure;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.ViewModels
{
    [Export(typeof(RestartPromptViewModel))]
    public class RestartPromptViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Command to restart application
        /// </summary>
        public ICommand RestartApplicationCommand { get { return new DelegateCommand(this.RestartApplication); } }

        /// <summary>
        /// Performs the actual application restart
        /// </summary>
        private void RestartApplication()
        {
            // Perform shutdown operations
            Commands.ApplicationShutdownCommand.Execute(null);

            // Start up the new process
            Process.Start(Application.ResourceAssembly.Location);

            // Shutdown the currect process
            Application.Current.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Normal);
        }
    }
}
