using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Modules.Teamspeak.Interfaces;
using GW2PAO.Modules.Teamspeak.Views;
using GW2PAO.Utility;
using NLog;

namespace GW2PAO.Modules.Teamspeak
{
    [Export(typeof(ITeamspeakViewController))]
    public class TeamspeakViewController : ITeamspeakViewController
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
        /// The teamspeak overlay view
        /// </summary>
        private TeamspeakView teamspeakView;

        /// <summary>
        /// Displays all previously-opened windows and other windows
        /// that must be shown at startup
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing");
            Threading.BeginInvokeOnUI(() =>
            {
                if (Properties.Settings.Default.IsTeamspeakOpen && this.CanDisplayTeamspeakOverlay())
                    this.DisplayTeamspeakOverlay();
            });
        }

        /// <summary>
        /// Closes all windows and saves the "was previously opened" state for those windows.
        /// </summary>
        public void Shutdown()
        {
            logger.Debug("Shutting down");

            if (this.teamspeakView != null)
            {
                Properties.Settings.Default.IsTeamspeakOpen = this.teamspeakView.IsVisible;
                Threading.InvokeOnUI(() => this.teamspeakView.Close());
            }

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Displays the Teamspeak overlay window, or, if already displayed,
        /// sets focus to the window
        /// </summary>
        public void DisplayTeamspeakOverlay()
        {
            if (this.teamspeakView == null || !this.teamspeakView.IsVisible)
            {
                this.teamspeakView = new TeamspeakView();
                this.Container.ComposeParts(this.teamspeakView);
                this.teamspeakView.Show();
            }
            else
            {
                this.teamspeakView.Focus();
            }
        }

        /// <summary>
        /// Determines if the Teamspeak overlay window can be displayed
        /// </summary>
        /// <returns></returns>
        public bool CanDisplayTeamspeakOverlay()
        {
            return true;
        }
    }
}
