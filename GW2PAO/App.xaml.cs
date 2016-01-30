using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GW2PAO.Infrastructure;
using GW2PAO.Utility;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Practices.Prism.Commands;
using NLog;

namespace GW2PAO
{
    using System.Net;

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
            if (Process.GetProcessesByName("GW2PAO").Count() > 1)
            {
                // Application is already running
                Application.Current.Shutdown();
                return;
            }

            // Software only mode provides improved performance when using transparent windows
            System.Windows.Media.RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;

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

            // Disable client-side connection throttling
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
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
                var lang = LanguageExtensions.FromTwoLetterISOLanguageName(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                GW2PAO.Properties.Settings.Default.Language = lang.ToTwoLetterISOLanguageName();
                GW2PAO.Properties.Settings.Default.Save();
            }

            // Note: this conversion, while it may seem redundant, ensures that we use only use a known language
            // If the CurrentUICulture is something other than the supported languages, this call defaults it to english.
            var savedLang = LanguageExtensions.FromTwoLetterISOLanguageName(GW2PAO.Properties.Settings.Default.Language);
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(savedLang.ToTwoLetterISOLanguageName());
        }

        private void DoShutdown()
        {
            logger.Info("Program shutting down");
        }

        /// <summary>
        /// Last chance exception handler - Logs the offending exception
        /// </summary>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                logger.Fatal((Exception)e.ExceptionObject);

                // Cleanup the tray icon even if we crash
                Commands.CleanupTrayIcon.Execute(null);

#if !DEBUG
                // Show a message to the user to allow them to send an error report
                var selection = MessageBox.Show(
                    "A fatal error has occurred. Would you like to send an anonymous error report?",
                    "GW2 Personal Assistant Overlay",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Error);

                if (selection == MessageBoxResult.Yes)
                {
                    var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                    byte[] logData = null;
                    if (File.Exists("Logs\\logfile.log"))
                        logData = File.ReadAllBytes("Logs\\logfile.log");

                    string body = "Fatal exception:\r\n\r\n" + e.ExceptionObject.ToString();

                    if (logData == null)
                    {
                        MailUtility.Email(
                            MailUtility.MAIL_USER,
                            body,
                            "Crash Report",
                            MailUtility.MAIL_USER,
                            executingAssembly.GetName().Name + " - " + executingAssembly.GetName().Version,
                            MailUtility.MAIL_USER,
                            MailUtility.MAIL_PASS);
                    }
                    else
                    {
                        MailUtility.Email(
                            MailUtility.MAIL_USER,
                            body,
                            "Crash Report",
                            MailUtility.MAIL_USER,
                            executingAssembly.GetName().Name + " - " + executingAssembly.GetName().Version,
                            MailUtility.MAIL_USER,
                            MailUtility.MAIL_PASS,
                            new MailAttachment(logData, "crashlog.txt"));
                    }
                }

                Process.GetCurrentProcess().Kill();
#endif
            }
            catch
            {
                // Swallow
            }
        }
    }
}
