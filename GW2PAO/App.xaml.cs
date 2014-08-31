using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Awesomium.Core;
using GW2PAO.Controllers;
using GW2PAO.TrayIcon;
using GW2PAO.ViewModels.TrayIcon;
using Hardcodet.Wpf.TaskbarNotification;
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

        /// <summary>
        /// The actual taskbar icon object
        /// </summary>
        private static TaskbarIcon TaskbarIcon;

        /// <summary>
        /// View model for the primary application tray icon
        /// </summary>
        private static TrayIconViewModel TrayIconVm;

        /// <summary>
        /// The primary application tray icon
        /// </summary>
        public static IApplicationTrayIcon TrayIcon { get; private set; }

        /// <summary>
        /// The main application controller
        /// </summary>
        private static ApplicationController AppController;

        /// <summary>
        /// The overlay menu icon (little icon that shows up on the screen, rather than in the tray)
        /// </summary>
        private static OverlayMenuIcon ApplicationOverlayMenuIcon;

        /// <summary>
        /// Application startup
        /// </summary>
        public void AppStartup(object sender, StartupEventArgs e)
        {
            // Update settings if neccessary
            if (GW2PAO.Properties.Settings.Default.UpgradeRequired)
            {
                GW2PAO.Properties.Settings.Default.Upgrade();
                GW2PAO.Properties.Settings.Default.UpgradeRequired = false;
                GW2PAO.Properties.Settings.Default.Save();
            }

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

            // Log application information
            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(executingAssembly.Location);
            logger.Info("Application starting - " + executingAssembly.GetName().Name + " - " + executingAssembly.GetName().Version + " - " + fvi.FileVersion + " - " + fvi.ProductVersion);

            // Initialize the last chance exception handlers
            logger.Debug("Registering last chance exception handlers");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

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

            // Create dummy window so that the only way to exit the app is by using the tray icon
            Window dummyWindow = new Window()
            {
                WindowStyle = System.Windows.WindowStyle.None,
                AllowsTransparency = true,
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent),
                ShowInTaskbar = false,
                Title = "GW2 Personal Assistant Overlay"
            };
            dummyWindow.Show();
            GW2PAO.Views.OverlayWindow.OwnerWindow = dummyWindow;

            // Create the tray icon
            logger.Debug("Creating tray icon");
            TaskbarIcon = (TaskbarIcon)this.FindResource("TrayIcon");
            TrayIconVm = new TrayIconViewModel();
            TaskbarIcon.DataContext = TrayIconVm;
            TaskbarIcon.ContextMenu.DataContext = TrayIconVm;
            TaskbarIcon.ContextMenu.ItemsSource = TrayIconVm.MenuItems;
            App.TrayIcon = new ApplicationTrayIcon(TaskbarIcon);
            logger.Debug("Tray icon created");

            // Initialize the application controller
            AppController = new ApplicationController();

            // Initialize the OverlayMenuIcon
            ApplicationOverlayMenuIcon = new OverlayMenuIcon(TrayIconVm);

            // Set up the menu items
            logger.Debug("Initializing menu items");
            if (TrayIconVm != null)
            {
                foreach (var item in AppController.GetMenuItems())
                    TrayIconVm.MenuItems.Add(item);

                TrayIconVm.MenuItems.Add(null); // Null is treated as a seperator

                TrayIconVm.MenuItems.Add(new MenuItemViewModel("Click-Through Windows", null, true,
                    () => { return GW2PAO.Properties.Settings.Default.IsClickthroughEnabled; },
                    (enabled) => {
                                    GW2PAO.Properties.Settings.Default.IsClickthroughEnabled = enabled;
                                    GW2PAO.Properties.Settings.Default.Save();
                                 },
                    GW2PAO.Properties.Settings.Default, "IsClickthroughEnabled"));

                TrayIconVm.MenuItems.Add(new MenuItemViewModel("Overlay Menu Icon", null, true,
                    () => { return ApplicationOverlayMenuIcon.IsVisible; },
                    (show) => { ApplicationOverlayMenuIcon.IsVisible = show; },
                    ApplicationOverlayMenuIcon, "IsVisible"));
                TrayIconVm.MenuItems.Add(new MenuItemViewModel("About", () => new GW2PAO.Views.AboutView().Show()));
                TrayIconVm.MenuItems.Add(new MenuItemViewModel("Exit", this.ExitAndCleanup));
            }

            // Show a notification that the program is now running
            // UPDATE: Removed for now...
            //TrayIcon.DisplayNotification("GW2 Personal Assistant Overlay is now running", "Click here for options", TrayInfoMessageType.None);

            logger.Info("Program startup complete");

            // Check admin privileges - some parts of the application won't work if the application doesn't have permissions to access the Mumble interface
            try
            {
                bool isRunning = AppController.SystemService.IsGw2Running;
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                // An exception can happen if GW2 is running as admin
                // If that occurs, display a notification
                if (ex.NativeErrorCode == 5)
                {
                    App.TrayIcon.DisplayNotification("Warning", "Some features cannot be started because GW2 is running as administrator.", TrayInfoMessageType.Warning);
                    logger.Warn(ex);
                }
            }

            // Reopen windows based on user settings
            AppController.ReopenWindowsFromSettings();
        }

        /// <summary>
        /// Exits and cleans up the application
        /// </summary>
        private void ExitAndCleanup()
        {
            logger.Info("Program shutting down");

#if !NO_BROWSER
            logger.Debug("Cleaning up Awesomium WebCore");
            if (WebCore.IsInitialized)
            {
                WebCore.Shutdown();
            }
#endif

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
                ApplicationOverlayMenuIcon.Shutdown();
                AppController.Shutdown();
                Application.Current.Dispatcher.Invoke(TaskbarIcon.Dispose);
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
