using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Infrastructure;
using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Infrastructure.ViewModels;
using GW2PAO.Properties;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.ViewModels
{
    [Export]
    public class ShellViewModel : BindableBase, IPartImportsSatisfiedNotification
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [ImportMany]
        private Lazy<IMenuItem, IOrderMetadata>[] unorderedMainMenu;

        /// <summary>
        /// Collection of menu items that make up the application's main menu
        /// </summary>
        public ObservableCollection<IMenuItem> MainMenu { get; private set; }

        /// <summary>
        /// True if the overlay menu icon is visible, else false
        /// </summary>
        public bool IsOverlayMenuIconVisible
        {
            get { return Settings.Default.IsOverlayIconVisible; }
            set
            {
                if (Settings.Default.IsOverlayIconVisible != value)
                {
                    Settings.Default.IsOverlayIconVisible = value;
                    Settings.Default.Save();
                    this.OnPropertyChanged(() => this.IsOverlayMenuIconVisible);
                }
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        [ImportingConstructor]
        public ShellViewModel(ISystemService systemService)
        {
            this.MainMenu = new ObservableCollection<IMenuItem>();

            // Initialize the process monitor
            GW2PAO.Views.OverlayWindow.ProcessMonitor = new ProcessMonitor(systemService);
        }

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            // Re-order all menu items according to their Order metadata.
            foreach (var menuItem in this.unorderedMainMenu.OrderBy(menu => menu.Metadata.Order))
            {
                var menu = menuItem.Value;
                this.MainMenu.Add(menu);
            }

            // Add the application-specific menu items
            this.MainMenu.Add(null); // Null for separator

            // Settings
            var settingsMenu = new MenuItem(GW2PAO.Properties.Resources.Settings);
            settingsMenu.SubMenuItems.Add(new CheckableMenuItem(GW2PAO.Properties.Resources.ShowNotificationBorders, false, () => Settings.Default.AreNotificationWindowBordersVisible, Settings.Default));
            settingsMenu.SubMenuItems.Add(new CheckableMenuItem(GW2PAO.Properties.Resources.NonInteractiveWindows, false, () => Settings.Default.IsClickthroughEnabled, Settings.Default));
            settingsMenu.SubMenuItems.Add(new CheckableMenuItem(GW2PAO.Properties.Resources.StickyWindows, false, () => Settings.Default.AreWindowsSticky, Settings.Default));
            settingsMenu.SubMenuItems.Add(new CheckableMenuItem(GW2PAO.Properties.Resources.OverlayMenuIcon, false, () => this.IsOverlayMenuIconVisible, this));
            settingsMenu.SubMenuItems.Add(new CheckableMenuItem(GW2PAO.Properties.Resources.CheckForUpdatesAtStartup, false, () => Settings.Default.CheckForUpdates, Settings.Default));
            this.MainMenu.Add(settingsMenu);

            // About
            this.MainMenu.Add(new MenuItem(GW2PAO.Properties.Resources.About, () => new GW2PAO.Views.AboutView().Show()));
            
            // Exit
            this.MainMenu.Add(new MenuItem(GW2PAO.Properties.Resources.Exit, () => 
                {
                    Task.Factory.StartNew(() =>
                        {
                            Commands.ApplicationShutdownCommand.Execute(null);
                            Application.Current.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Normal);
                        });
                }));

            // At this point, the program startup should be completed
            logger.Info("Program startup complete");
        }
    }
}
