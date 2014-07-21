using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
                    LogManager.DisableLogging();
                else
                    LogManager.EnableLogging();
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public AboutViewModel()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.Version = fvi.ProductVersion;
        }
    }
}
