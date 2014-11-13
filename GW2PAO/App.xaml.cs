using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Awesomium.Core;
using GW2PAO.Infrastructure;
using GW2PAO.Utility;
using GW2PAO.Utility.Interfaces;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Practices.Prism.Commands;
using NLog;

namespace GW2PAO
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.InitializeSettings();
            this.InitializeLogging();

            // Log application information
            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(executingAssembly.Location);
            logger.Info("Application starting - " + executingAssembly.GetName().Name + " - " + executingAssembly.GetName().Version + " - " + fvi.FileVersion + " - " + fvi.ProductVersion);

            // Initialize the last chance exception handlers
            logger.Debug("Registering last chance exception handlers");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            this.InitializeInternationalization();

            if (GW2PAO.Properties.Settings.Default.CheckForUpdates)
                UpdateChecker.CheckForUpdateAndNotify();

            ApplicationBootstrapper ab = new ApplicationBootstrapper();
            ab.Run();

            Commands.ApplicationShutdownCommand.RegisterCommand(new DelegateCommand(this.DoShutdown));

            GW2PAO.Properties.Settings.Default.FirstTimeRun = false;
            GW2PAO.Properties.Settings.Default.Save();
        }

        private void InitializeSettings()
        {
            // We save this so that if we perform an upgrade of settings,
            //  we still treat this startup as a first-time run of the application
            bool firstTimeUse = GW2PAO.Properties.Settings.Default.FirstTimeRun;

            // Update settings if neccessary
            if (GW2PAO.Properties.Settings.Default.UpgradeRequired)
            {
                GW2PAO.Properties.Settings.Default.Upgrade();
                GW2PAO.Properties.Settings.Default.UpgradeRequired = false;
                GW2PAO.Properties.Settings.Default.FirstTimeRun = firstTimeUse;
                GW2PAO.Properties.Settings.Default.Save();
            }
        }

        private void InitializeLogging()
        {
#if DEBUG
            // Enable logging if running in debug
            LogManager.GlobalThreshold = NLog.LogLevel.Trace;
#else
            // Set up logging configuration
            if (!GW2PAO.Properties.Settings.Default.IsLoggingEnabled)
                LogManager.GlobalThreshold = NLog.LogLevel.Fatal;
#endif

            // Disable the debug assert windows that pop-up from NLog
            System.Diagnostics.Trace.Listeners.OfType<System.Diagnostics.DefaultTraceListener>().First().AssertUiEnabled = false;

        }

        private void InitializeInternationalization()
        {
            // Set up language information
            if (string.IsNullOrWhiteSpace(GW2PAO.Properties.Settings.Default.Language))
            {
                GW2PAO.Properties.Settings.Default.Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                GW2PAO.Properties.Settings.Default.Save();
            }
            else
            {
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(GW2PAO.Properties.Settings.Default.Language);
            }
            ////////////////////////////////////////// DEBUG ///////////////////////////////////////////////////////
            //CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("en");
            ////////////////////////////////////////// DEBUG ///////////////////////////////////////////////////////
        }

        private void DoShutdown()
        {
            logger.Info("Program shutting down");

            // Really hate to have to do this, but I can't have logs filling up people's disk space
            //  There's no way to disable logging (tried but it didn't work)
            logger.Debug("Cleaning up GwApiNETLog");
            foreach (System.IO.FileInfo f in new System.IO.DirectoryInfo(".").GetFiles("GwApiNETLog_*.txt"))
            {
                f.Delete();
            }

            // Do this on a worker thread so we don't dead-lock when shutting down controllers and views
            Task.Factory.StartNew(() =>
            {
                if (GW2PAO.Views.OverlayWindow.ProcessMonitor != null)
                    GW2PAO.Views.OverlayWindow.ProcessMonitor.Dispose();

                Application.Current.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Normal);
            });
        }

        /// <summary>
        /// Last chance exception handler - Logs the offending exception
        /// </summary>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Fatal((Exception)e.ExceptionObject);
        }
    }
}
