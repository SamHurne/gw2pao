using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.Commerce.Interfaces;
using GW2PAO.Modules.Commerce.Views;
using GW2PAO.Modules.Commerce.Views.PriceNotification;
using GW2PAO.Modules.Commerce.Views.PriceTracker;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Commands;
using NLog;

namespace GW2PAO.Modules.Commerce
{
    [Export(typeof(ICommerceViewController))]
    public class CommerceViewController : ICommerceViewController
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
        /// The TP Calculator utility window
        /// </summary>
        private TPCalculatorView tpCalculatorView;

        /// <summary>
        /// Window containing price notifications
        /// </summary>
        private PriceNotificationWindow priceNotificationsView;

        /// <summary>
        /// View used when rebuilding the item names database
        /// </summary>
        private RebuildNamesDatabaseView rebuildItemNamesView;

        /// <summary>
        /// The price tracker view
        /// </summary>
        private PriceTrackerView priceTrackerView;

        /// <summary>
        /// The price tracker view
        /// </summary>
        private EctoSalvageHelperView ectoSalvageView;

        /// <summary>
        /// Displays all previously-opened windows and other windows
        /// that must be shown at startup
        /// </summary>
        public void Initialize()
        {
            logger.Debug("Initializing");

            logger.Debug("Registering hotkey commands");
            HotkeyCommands.TogglePriceTrackerCommand.RegisterCommand(new DelegateCommand(this.TogglePriceTracker));

            Threading.BeginInvokeOnUI(() =>
            {
                if (Properties.Settings.Default.IsTPCalculatorOpen && this.CanDisplayTPCalculator())
                    this.DisplayTPCalculator();

                if (Properties.Settings.Default.IsPriceTrackerOpen && this.CanDisplayPriceTracker())
                    this.DisplayPriceTracker();

                if (Properties.Settings.Default.IsEctoHelperOpen && this.CanDisplayEctoSalvageTracker())
                    this.DisplayEctoSalvageTracker();

                if (this.CanDisplayPriceNotificationsWindow())
                    this.DisplayPriceNotificationsWindow();
            });
        }

        /// <summary>
        /// Closes all windows and saves the "was previously opened" state for those windows.
        /// </summary>
        public void Shutdown()
        {
            logger.Debug("Shutting down");
            
            if (this.tpCalculatorView != null)
            {
                Properties.Settings.Default.IsTPCalculatorOpen = this.tpCalculatorView.IsVisible;
                Threading.InvokeOnUI(() => this.tpCalculatorView.Close());
            }

            if (this.priceTrackerView != null)
            {
                Properties.Settings.Default.IsPriceTrackerOpen = this.priceTrackerView.IsVisible;
                Threading.InvokeOnUI(() => this.priceTrackerView.Close());
            }

            if (this.ectoSalvageView != null)
            {
                Properties.Settings.Default.IsEctoHelperOpen = this.ectoSalvageView.IsVisible;
                Threading.InvokeOnUI(() => this.ectoSalvageView.Close());
            }

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Displays the TP Calculator window, or, if already displayed,
        /// sets focus to the window
        /// </summary>
        public void DisplayTPCalculator()
        {
            if (this.tpCalculatorView == null || !this.tpCalculatorView.IsVisible)
            {
                this.tpCalculatorView = new TPCalculatorView();
                this.Container.ComposeParts(this.tpCalculatorView);
                this.tpCalculatorView.Show();
            }
            else
            {
                this.tpCalculatorView.Focus();
            }
        }

        /// <summary>
        /// Determines if the TP Calculator can be displayed
        /// </summary>
        /// <returns></returns>
        public bool CanDisplayTPCalculator()
        {
            return true;
        }

        /// <summary>
        /// Displays the Price Rebuild Item Names Database window, or, if already displayed,
        /// sets focus to the window
        /// </summary>
        public void DisplayRebuildItemNamesView()
        {
            if (this.rebuildItemNamesView == null || !this.rebuildItemNamesView.IsVisible)
            {
                this.rebuildItemNamesView = new RebuildNamesDatabaseView();
                this.Container.ComposeParts(this.rebuildItemNamesView);
                this.rebuildItemNamesView.Show();
            }
            else
            {
                this.rebuildItemNamesView.Focus();
            }
        }

        /// <summary>
        /// Determines if the Rebuild Item Names Database window can be displayed
        /// </summary>
        public bool CanDisplayRebuildItemNamesView()
        {
            return this.rebuildItemNamesView == null || this.rebuildItemNamesView.IsClosed;
        }

        /// <summary>
        /// Displays the Price Tracker window, or, if already displayed, sets
        /// focus to the window
        /// </summary>
        public void DisplayPriceTracker()
        {
            if (this.priceTrackerView == null || !this.priceTrackerView.IsVisible)
            {
                this.priceTrackerView = new PriceTrackerView();
                this.Container.ComposeParts(this.priceTrackerView);
                this.priceTrackerView.Show();
            }
            else
            {
                this.priceTrackerView.Focus();
            }
        }

        /// <summary>
        /// Determines if the price tracker can be displayed
        /// </summary>
        /// <returns>Always true</returns>
        public bool CanDisplayPriceTracker()
        {
            return true;
        }

        /// <summary>
        /// Displays the Price Notifications window
        /// </summary>
        public void DisplayPriceNotificationsWindow()
        {
            if (this.priceNotificationsView == null || !this.priceNotificationsView.IsVisible)
            {
                this.priceNotificationsView = new PriceNotificationWindow();
                this.Container.ComposeParts(this.priceNotificationsView);
                this.priceNotificationsView.Show();
            }
            else
            {
                this.priceNotificationsView.Focus();
            }
        }

        /// <summary>
        /// Determines if the Price Notifications window can be displayed
        /// </summary>
        /// <returns>Always true</returns>
        public bool CanDisplayPriceNotificationsWindow()
        {
            return true;
        }

        /// <summary>
        /// Displays the Ecto Salvage Helper window, or, if already displayed, sets
        /// focus to the window
        /// </summary>
        public void DisplayEctoSalvageTracker()
        {
            if (this.ectoSalvageView == null || !this.ectoSalvageView.IsVisible)
            {
                this.ectoSalvageView = new EctoSalvageHelperView();
                this.Container.ComposeParts(this.ectoSalvageView);
                this.ectoSalvageView.Show();
            }
            else
            {
                this.ectoSalvageView.Focus();
            }
        }

        /// <summary>
        /// Determines if the price tracker can be displayed
        /// </summary>
        /// <returns>Always true</returns>
        public bool CanDisplayEctoSalvageTracker()
        {
            return true;
        }

        /// <summary>
        /// Toggles whether or not the price tracker is visible
        /// </summary>
        private void TogglePriceTracker()
        {
            if (this.priceTrackerView == null || !this.priceTrackerView.IsVisible)
            {
                this.DisplayPriceTracker();
            }
            else
            {
                this.priceTrackerView.Close();
            }
        }
    }
}
