using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.ZoneCompletion.Interfaces;
using GW2PAO.Modules.ZoneCompletion.Views;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Commands;
using NLog;
namespace GW2PAO.Modules.ZoneCompletion
{
    [Export(typeof(IZoneCompletionViewController))]
    public class ZoneCompletionViewController : IZoneCompletionViewController
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
        /// The zone completion assistant view
        /// </summary>
        private ZoneCompletionView zoneCompletionAssistantView;

        /// <summary>
        /// Displays all previously-opened windows and other windows
        /// that must be shown at startup
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing");

            logger.Debug("Registering hotkey commands");
            HotkeyCommands.ToggleZoneAssistantCommand.RegisterCommand(new DelegateCommand(this.ToggleZoneCompletionAssistant));

            Threading.BeginInvokeOnUI(() =>
            {
                if (Properties.Settings.Default.IsZoneAssistantOpen && this.CanDisplayZoneCompletionAssistant())
                    this.DisplayZoneCompletionAssistant();
            });
        }

        /// <summary>
        /// Closes all windows and saves the "was previously opened" state for those windows.
        /// </summary>
        public void Shutdown()
        {
            logger.Debug("Shutting down");

            if (this.zoneCompletionAssistantView != null)
            {
                Properties.Settings.Default.IsZoneAssistantOpen = this.zoneCompletionAssistantView.IsVisible;
                Threading.InvokeOnUI(() => this.zoneCompletionAssistantView.Close());
            }

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Displays the Zone Completion Assistant window, or, if already displayed,
        /// sets focus to the window
        /// </summary>
        public void DisplayZoneCompletionAssistant()
        {
            if (this.zoneCompletionAssistantView == null || !this.zoneCompletionAssistantView.IsVisible)
            {
                this.zoneCompletionAssistantView = new ZoneCompletionView();
                this.Container.ComposeParts(this.zoneCompletionAssistantView);
                this.zoneCompletionAssistantView.Show();
            }
            else
            {
                this.zoneCompletionAssistantView.Focus();
            }
        }

        /// <summary>
        /// Determines if the zone assistant can be displayed
        /// </summary>
        public bool CanDisplayZoneCompletionAssistant()
        {
            return true;
        }

        /// <summary>
        /// Toggles whether or not the zone assistant is visible
        /// </summary>
        private void ToggleZoneCompletionAssistant()
        {
            if (this.zoneCompletionAssistantView == null || !this.zoneCompletionAssistantView.IsVisible)
            {
                this.DisplayZoneCompletionAssistant();
            }
            else
            {
                this.zoneCompletionAssistantView.Close();
            }
        }
    }
}
