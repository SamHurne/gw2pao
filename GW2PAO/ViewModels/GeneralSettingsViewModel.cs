using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure;
using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.PresentationCore;
using GW2PAO.Properties;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.ViewModels
{
    [Export(typeof(GeneralSettingsViewModel))]
    public class GeneralSettingsViewModel : BindableBase, ISettingsViewModel
    {
        public string SettingsHeader
        {
            // TODO: Add resource string
            get { return "General"; }
        }

        /// <summary>
        /// True if notification window borders are visible, else false
        /// </summary>
        public bool AreNotificationWindowBordersVisible
        {
            get { return Settings.Default.AreNotificationWindowBordersVisible; }
            set
            {
                if (Settings.Default.AreNotificationWindowBordersVisible != value)
                {
                    Settings.Default.AreNotificationWindowBordersVisible = value;
                    Settings.Default.Save();
                    this.OnPropertyChanged(() => this.AreNotificationWindowBordersVisible);
                }
            }
        }

        /// <summary>
        /// True if window click-through is enabled, else false
        /// </summary>
        public bool IsClickthroughEnabled
        {
            get { return Settings.Default.IsClickthroughEnabled; }
            set
            {
                if (Settings.Default.IsClickthroughEnabled != value)
                {
                    Settings.Default.IsClickthroughEnabled = value;
                    Settings.Default.Save();
                    this.OnPropertyChanged(() => this.IsClickthroughEnabled);
                }
            }
        }

        /// <summary>
        /// True if windows are sticky, else false
        /// </summary>
        public bool AreWindowsSticky
        {
            get { return Settings.Default.AreWindowsSticky; }
            set
            {
                if (Settings.Default.AreWindowsSticky != value)
                {
                    Settings.Default.AreWindowsSticky = value;
                    Settings.Default.Save();
                    this.OnPropertyChanged(() => this.AreWindowsSticky);
                }
            }
        }

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
        /// True if the application should check for updates at startup, else false
        /// </summary>
        public bool CheckForUpdates
        {
            get { return Settings.Default.CheckForUpdates; }
            set
            {
                if (Settings.Default.CheckForUpdates != value)
                {
                    Settings.Default.CheckForUpdates = value;
                    Settings.Default.Save();
                    this.OnPropertyChanged(() => this.CheckForUpdates);
                }
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public GeneralSettingsViewModel()
        {
            // Register for any general-setting hotkey commands
            this.InitializeHotkeyCommandHandlers();
        }

        /// <summary>
        /// Initializes any setting-related hotkey command handlers
        /// </summary>
        private void InitializeHotkeyCommandHandlers()
        {
            HotkeyCommands.ToggleInteractiveWindowsCommand.RegisterCommand(new DelegateCommand(() => this.IsClickthroughEnabled = !this.IsClickthroughEnabled));
            HotkeyCommands.ToggleNotificationWindowBordersCommand.RegisterCommand(new DelegateCommand(() => this.AreNotificationWindowBordersVisible = !this.AreNotificationWindowBordersVisible));
            HotkeyCommands.ToggleOverlayMenuIconCommand.RegisterCommand(new DelegateCommand(() => this.IsOverlayMenuIconVisible = !this.IsOverlayMenuIconVisible));
        }
    }
}
