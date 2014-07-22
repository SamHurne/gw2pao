using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
        /// View model for the primary application tray icon
        /// </summary>
        private static TrayIconViewModel TrayIconVm;

        /// <summary>
        /// The primary application tray icon
        /// </summary>
        public static IApplicationTrayIcon TrayIcon { get; private set; }

        /// <summary>
        /// Application startup
        /// </summary>
        public void AppStartup(object sender, StartupEventArgs e)
        {
            // Set up logging configuration
            if (!GW2PAO.Properties.Settings.Default.IsLoggingEnabled)
                LogManager.DisableLogging();

            // Log application information
            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(executingAssembly.Location);
            logger.Info("Application starting - " + executingAssembly.GetName().Name + " - " + executingAssembly.GetName().Version + " - " + fvi.FileVersion + " - " + fvi.ProductVersion);

            // Register the Exit event handler
            this.Exit += App_Exit;

            // Initialize the last chance exception handlers
            logger.Debug("Registering last chance exception handlers");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Initialize the application controller
            ApplicationController appController = new ApplicationController();

            // Create the tray icon
            logger.Debug("Creating tray icon");
            TaskbarIcon trayIcon = (TaskbarIcon)this.FindResource("TrayIcon");
            TrayIconVm = new TrayIconViewModel();
            trayIcon.DataContext = TrayIconVm;
            trayIcon.ContextMenu.DataContext = TrayIconVm;
            trayIcon.ContextMenu.ItemsSource = TrayIconVm.MenuItems;
            App.TrayIcon = new ApplicationTrayIcon(trayIcon);
            logger.Debug("Tray icon created");

            // Set up the menu items
            logger.Debug("Initializing menu items");
            if (TrayIconVm != null)
            {
                foreach (var item in appController.GetMenuItems())
                    TrayIconVm.MenuItems.Add(item);

                TrayIconVm.MenuItems.Add(null); // Null is treated as a seperator

                TrayIconVm.MenuItems.Add(new MenuItemViewModel("About", () => new GW2PAO.Views.AboutView().Show()));
                TrayIconVm.MenuItems.Add(new MenuItemViewModel("Exit", Application.Current.Shutdown));
            }

            // Create dummy window so that the only way to exit the app is by using the tray icon
            Window dummyWindow = new Window()
            {
                WindowStyle = System.Windows.WindowStyle.None,
                AllowsTransparency = true,
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent),
                ShowInTaskbar = false
            };
            dummyWindow.Show();
            dummyWindow.Hide();

            // Show a notification that the program is now running
            TrayIcon.DisplayNotification("GW2 Personal Assistant Overlay is now running", "Click here for options", TrayInfoMessageType.None);

            logger.Info("Program startup complete");
        }

        /// <summary>
        /// Handler for the application exit event
        /// </summary>
        private void App_Exit(object sender, ExitEventArgs e)
        {
            logger.Info("Program shutting down");
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
