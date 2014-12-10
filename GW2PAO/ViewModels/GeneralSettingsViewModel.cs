using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GW2PAO.Infrastructure;
using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Properties;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.ViewModels
{
    [Export(typeof(GeneralSettingsViewModel))]
    public class GeneralSettingsViewModel : BindableBase, ISettingsViewModel
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private Language currentLanguage;

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
        /// The currently selected language
        /// </summary>
        public Language CurrentLanguage
        {
            get { return this.currentLanguage; }
            set
            {
                if (this.SetProperty(ref this.currentLanguage, value))
                {
                    this.ApplyLanguageCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Full collection of possible languages
        /// </summary>
        public ICollection<Language> PossibleLanguages
        {
            get
            {
                return Enum.GetValues(typeof(Language)).Cast<Language>().ToList();
            }
        }

        /// <summary>
        /// Command to apply the currently selected language
        /// </summary>
        public DelegateCommand ApplyLanguageCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public GeneralSettingsViewModel()
        {
            this.ApplyLanguageCommand = new DelegateCommand(this.ApplyLanguage, this.CanApplyLanguage);
            this.CurrentLanguage = LanguageExtensions.FromTwoLetterISOLanguageName(Settings.Default.Language);
        }

        /// <summary>
        /// Initializes any setting-related hotkey command handlers
        /// </summary>
        public void InitializeHotkeyCommandHandlers()
        {
            HotkeyCommands.ToggleInteractiveWindowsCommand.RegisterCommand(new DelegateCommand(() => this.IsClickthroughEnabled = !this.IsClickthroughEnabled));
            HotkeyCommands.ToggleNotificationWindowBordersCommand.RegisterCommand(new DelegateCommand(() => this.AreNotificationWindowBordersVisible = !this.AreNotificationWindowBordersVisible));
            HotkeyCommands.ToggleOverlayMenuIconCommand.RegisterCommand(new DelegateCommand(() => this.IsOverlayMenuIconVisible = !this.IsOverlayMenuIconVisible));
        }

        /// <summary>
        /// Performs the actual language apply, which involves restarting the application
        /// </summary>
        private void ApplyLanguage()
        {
            Settings.Default.Language = this.CurrentLanguage.ToTwoLetterISOLanguageName();
            Settings.Default.Save();
            this.ApplyLanguageCommand.RaiseCanExecuteChanged();

            // Show notification that the user must restart the application
            Commands.PromptForRestartCommand.Execute(null);
        }

        /// <summary>
        /// Determines if the current language can be applied (e.g. if it is different)
        /// </summary>
        private bool CanApplyLanguage()
        {
            return LanguageExtensions.FromTwoLetterISOLanguageName(Settings.Default.Language) != this.CurrentLanguage;
        }
    }
}
