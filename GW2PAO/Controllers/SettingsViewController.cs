using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GW2PAO.Infrastructure;
using GW2PAO.Interfaces;
using GW2PAO.Modules.Commerce.Views;
using GW2PAO.Modules.Dungeons.Views;
using GW2PAO.Modules.Events.Views;
using GW2PAO.Modules.WvW.Views;
using GW2PAO.PresentationCore;
using GW2PAO.Utility;
using GW2PAO.Views;
using NLog;

namespace GW2PAO.Controllers
{
    [Export(typeof(ISettingsViewController))]
    public class SettingsViewController : ISettingsViewController
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Composition container of composed parts
        /// </summary>
        [Import]
        private CompositionContainer Container { get; set; }

        /// <summary>
        /// The actual settings view
        /// </summary>
        private SettingsView settingsView;

        /// <summary>
        /// Initializes the settings view controller
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing");

            logger.Debug("Registering command handlers");
            Commands.OpenGeneralSettingsCommand.RegisterCommand(new DelegateCommand(() => this.OpenSettings()));
            Commands.OpenHotkeySettingsCommand.RegisterCommand(new DelegateCommand(() => this.OpenSettings(typeof(HotkeySettingsView))));
            Commands.OpenEventSettingsCommand.RegisterCommand(new DelegateCommand(() => this.OpenSettings(typeof(EventSettingsView))));
            Commands.OpenDungeonSettingsCommand.RegisterCommand(new DelegateCommand(() => this.OpenSettings(typeof(DungeonSettingsView))));
            Commands.OpenCommerceSettingsCommand.RegisterCommand(new DelegateCommand(() => this.OpenSettings(typeof(CommerceSettingsView))));
            Commands.OpenWvWSettingsCommand.RegisterCommand(new DelegateCommand(() => this.OpenSettings(typeof(WvWSettingsView))));
            Commands.PromptForRestartCommand.RegisterCommand(new DelegateCommand(this.PromptForRestart));
        }

        /// <summary>
        /// Shuts down the settings view controller
        /// </summary>
        public void Shutdown()
        {
            if (this.settingsView != null && !this.settingsView.IsClosed)
            {
                Threading.InvokeOnUI(() =>
                    {
                        this.settingsView.Close();
                        this.settingsView = null;
                    });
            }
        }

        /// <summary>
        /// Opens the settings window, with an optional specific tab
        /// </summary>
        /// <param name="specificTab">Type of tab to open</param>
        private void OpenSettings(Type specificTab = null)
        {
            if (this.settingsView != null && !this.settingsView.IsClosed)
            {
                this.settingsView.Focus();
            }
            else
            {
                this.settingsView = new SettingsView();
                this.Container.ComposeParts(this.settingsView);
                this.settingsView.Show();
            }

            if (specificTab != null)
            {
                foreach (TabItem item in this.settingsView.SettingsTabControl.Items)
                {
                    if (item.Content.GetType() == specificTab)
                    {
                        this.settingsView.SettingsTabControl.SelectedItem = item;
                        break;
                    }
                }

            }
        }

        /// <summary>
        /// Displays the window that prompts the user to restart the application
        /// </summary>
        private void PromptForRestart()
        {
            var promptView = new RestartPromptView();
            this.Container.ComposeParts(promptView);
            promptView.Show();
        }
    }
}
