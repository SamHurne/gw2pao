using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.ViewModels
{
    /// <summary>
    /// View Model for the About window
    /// </summary>
    public class AboutViewModel
    {
        /// <summary>
        /// Version information for the application
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// True if logging to file is enabled, else false
        /// </summary>
        public bool IsLoggingEnabled
        {
            get { return Properties.Settings.Default.IsLoggingEnabled; }
            set
            {
                Properties.Settings.Default.IsLoggingEnabled = value;

                if (!Properties.Settings.Default.IsLoggingEnabled)
                    LogManager.GlobalThreshold = NLog.LogLevel.Fatal;
                else
                    LogManager.GlobalThreshold = NLog.LogLevel.Trace;

                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// License information
        /// </summary>
        public string OpenSourceLicense { get; private set; }

        /// <summary>
        /// ArenaNet copyright information (for use of images and the API)
        /// </summary>
        public string ArenaNetCopyright { get; private set; }

        /// <summary>
        /// Command to open help
        /// </summary>
        public DelegateCommand HelpCommand { get { return new DelegateCommand(this.OpenHelpPage); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public AboutViewModel()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.Version = fvi.ProductVersion;
            this.OpenSourceLicense = File.ReadAllText("LICENSE.txt", Encoding.Default);
            this.ArenaNetCopyright = File.ReadAllText("AnetCopyright.txt", Encoding.Default);
        }

        /// <summary>
        /// Opens the Help page (https://gw2pao.codeplex.com/documentation) using the default browser
        /// </summary>
        private void OpenHelpPage()
        {
            Process.Start("https://gw2pao.codeplex.com/documentation");
        }
    }
}
