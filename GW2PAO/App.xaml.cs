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
using GW2PAO.Utility;
using GW2PAO.Utility.Interfaces;
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
        /// The Process Monitor object that monitors the GW2 Process
        /// </summary>
        private static IProcessMonitor ProcessMonitor;

        /// <summary>
        /// Application startup
        /// </summary>
        public void AppStartup(object sender, StartupEventArgs e)
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

            GW2PAO.API.Services.CommerceService test = new GW2PAO.API.Services.CommerceService();
            //test.BuildItemDatabase();
            //var testt = test.GetItemID("test");
            var testId = test.GetItemID("Superior Rune of the Sunless");
            var itemPrices = test.GetItemPrices(testId);

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

            // Initialize the process monitor
            ProcessMonitor = new ProcessMonitor(AppController.SystemService);
            GW2PAO.Views.OverlayWindow.ProcessMonitor = ProcessMonitor;

            // Initialize the OverlayMenuIcon
            ApplicationOverlayMenuIcon = new OverlayMenuIcon(TrayIconVm);

            // Set up the menu items
            logger.Debug("Initializing menu items");
            if (TrayIconVm != null)
            {
                foreach (var item in AppController.GetMenuItems())
                    TrayIconVm.MenuItems.Add(item);

                TrayIconVm.MenuItems.Add(null); // Null is treated as a seperator

                var settingsMenu = new MenuItemViewModel("Settings", null);

                settingsMenu.SubMenuItems.Add(new MenuItemViewModel("Non-Interactive Windows", null, true,
                    () => { return GW2PAO.Properties.Settings.Default.IsClickthroughEnabled; },
                    (enabled) => {
                                    GW2PAO.Properties.Settings.Default.IsClickthroughEnabled = enabled;
                                    GW2PAO.Properties.Settings.Default.Save();
                                 },
                    GW2PAO.Properties.Settings.Default, "IsClickthroughEnabled"));

                settingsMenu.SubMenuItems.Add(new MenuItemViewModel("Sticky Windows", null, true,
                    () => { return GW2PAO.Properties.Settings.Default.AreWindowsSticky; },
                    (enabled) =>
                    {
                        GW2PAO.Properties.Settings.Default.AreWindowsSticky = enabled;
                        GW2PAO.Properties.Settings.Default.Save();
                    },
                    GW2PAO.Properties.Settings.Default, "AreWindowsSticky"));

                settingsMenu.SubMenuItems.Add(new MenuItemViewModel("Overlay Menu Icon", null, true,
                    () => { return ApplicationOverlayMenuIcon.IsVisible; },
                    (show) => { ApplicationOverlayMenuIcon.IsVisible = show; },
                    ApplicationOverlayMenuIcon, "IsVisible"));

                settingsMenu.SubMenuItems.Add(new MenuItemViewModel("Check for Updates at Startup", null, true,
                    () => { return GW2PAO.Properties.Settings.Default.CheckForUpdates; },
                    (enabled) =>
                    {
                        GW2PAO.Properties.Settings.Default.CheckForUpdates = enabled;
                        GW2PAO.Properties.Settings.Default.Save();
                    },
                    GW2PAO.Properties.Settings.Default, "CheckForUpdates"));

                TrayIconVm.MenuItems.Add(settingsMenu);
                TrayIconVm.MenuItems.Add(new MenuItemViewModel("About", () => new GW2PAO.Views.AboutView().Show()));
                TrayIconVm.MenuItems.Add(new MenuItemViewModel("Exit", this.ExitAndCleanup));
            }

            logger.Info("Program startup complete");

            // Reopen windows based on user settings
            AppController.ReopenWindowsFromSettings();

            GW2PAO.Properties.Settings.Default.FirstTimeRun = false;
            GW2PAO.Properties.Settings.Default.Save();

            // Perform a check for new updates
            if (GW2PAO.Properties.Settings.Default.CheckForUpdates)
                UpdateChecker.CheckForUpdateAndNotify();
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
                ProcessMonitor.Dispose();
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
