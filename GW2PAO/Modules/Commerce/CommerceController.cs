using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Data;
using GW2PAO.Data.UserData;
using GW2PAO.Modules.Commerce.Interfaces;
using GW2PAO.Modules.Commerce.Models;
using GW2PAO.Modules.Commerce.ViewModels;
using GW2PAO.Modules.Commerce.ViewModels.PriceNotification;
using GW2PAO.Utility;
using GW2PAO.ViewModels;
using NLog;
using OxyPlot;
using OxyPlot.Axes;

namespace GW2PAO.Modules.Commerce
{
    /// <summary>
    /// Controller class responsible for showing notifications for the price watch feature
    /// May expand in the future
    /// </summary>
    [Export(typeof(ICommerceController))]
    public class CommerceController : ICommerceController
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The commerce service object
        /// </summary>
        private ICommerceService commerceService;

        /// <summary>
        /// Collection used for keeping track of when to reset the shown-state of notifications
        /// </summary>
        private Dictionary<ItemPriceViewModel, DateTime> NotificationsResetDateTimes = new Dictionary<ItemPriceViewModel, DateTime>();

        /// <summary>
        /// Keeps track of how many times Start() has been called in order
        /// to support reuse of a single object
        /// </summary>
        private int startCallCount;

        /// <summary>
        /// True if the controller's timers are no longer running, else false
        /// </summary>
        private bool isStopped;

        /// <summary>
        /// The primary refresh timer object
        /// </summary>
        private Timer refreshTimer;

        /// <summary>
        /// Locking object for operations performed with the refresh timer
        /// </summary>
        private readonly object refreshTimerLock = new object();

        /// <summary>
        /// The interval by which to refresh the dungeon reset state (in ms)
        /// </summary>
        public int RefreshInterval { get; set; }

        /// <summary>
        /// The commers user data
        /// </summary>
        public CommerceUserData UserData { get; private set; }

        /// <summary>
        /// Collection of items for the price watch tracker and notifications
        /// </summary>
        public ObservableCollection<ItemPriceViewModel> ItemPrices { get; private set; }

        /// <summary>
        /// Collection of price notifications currently shown to the user
        /// </summary>
        public ObservableCollection<PriceNotificationViewModel> PriceNotifications { get; private set; }

        /// <summary>
        /// Data associated with the ectoplasm salvaging threshold tracker
        /// </summary>
        [Export]
        public EctoSalvageHelperViewModel EcoSalvageData { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="commerceService">The commerce service object</param>
        /// <param name="userData">The commerce user data object</param>
        [ImportingConstructor]
        public CommerceController(ICommerceService commerceService, CommerceUserData userData)
        {
            logger.Debug("Initializing Commerce Controller");
            this.commerceService = commerceService;
            this.UserData = userData;
            this.ItemPrices = new ObservableCollection<ItemPriceViewModel>();
            this.PriceNotifications = new ObservableCollection<PriceNotificationViewModel>();
            this.EcoSalvageData = new EctoSalvageHelperViewModel(userData);

            // Initialize the refresh timer
            this.refreshTimer = new Timer(this.Refresh);
            this.RefreshInterval = 30000; // 30 seconds... really only need to do this once a minute, but twice a minute isn't terrible

            // Initialize the start call count to 0
            this.startCallCount = 0;

            logger.Info("Commerce Controller initialized");
        }

        /// <summary>
        /// Starts the automatic refresh
        /// </summary>
        public void Start()
        {
            logger.Debug("Start called");
            Task.Factory.StartNew(() =>
            {
                // Start the timer if this is the first time that Start() has been called
                if (this.startCallCount == 0)
                {
                    this.InitializeItemPrices();

                    logger.Debug("Starting refresh timers");
                    this.isStopped = false;
                    this.Refresh();
                }

                this.startCallCount++;
                logger.Debug("startCallCount = {0}", this.startCallCount);

            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Stops the automatic refresh
        /// </summary>
        public void Stop()
        {
            this.startCallCount--;
            logger.Debug("Stop called - startCallCount = {0}", this.startCallCount);

            // Stop the refresh timer if all calls to Start() have had a matching call to Stop()
            if (this.startCallCount == 0)
            {
                logger.Debug("Stopping refresh timers");
                lock (this.refreshTimerLock)
                {
                    this.isStopped = true;
                    this.refreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Forces a shutdown of the controller, including all running timers/threads
        /// </summary>
        public void Shutdown()
        {
            logger.Debug("Shutdown called");
            logger.Debug("Stopping refresh timers");
            lock (this.refreshTimerLock)
            {
                this.isStopped = true;
                this.refreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Initializes the collection of price watches
        /// </summary>
        private void InitializeItemPrices()
        {
            // If for some reason there are items with an ID of 0, just remove them now
            var itemsToRemove = new List<PriceWatch>(this.UserData.PriceWatches.Where(pw => pw.ItemID == 0));
            foreach (var emptyItem in itemsToRemove)
                this.UserData.PriceWatches.Remove(emptyItem);

            if (this.UserData.PriceWatches.Count > 0)
            {
                var itemIds = this.UserData.PriceWatches.Select(pw => pw.ItemID);
                var itemData = this.commerceService.GetItems(itemIds.ToArray());

                // Initialize view models
                Threading.InvokeOnUI(() =>
                    {
                        foreach (var priceWatch in this.UserData.PriceWatches)
                        {
                            if (priceWatch.ItemID > 0)
                                this.ItemPrices.Add(new ItemPriceViewModel(priceWatch, itemData[priceWatch.ItemID], this, this.commerceService));
                            else
                                this.ItemPrices.Add(new ItemPriceViewModel(priceWatch, null, this, this.commerceService));
                        }
                    });
            }
        }

        /// <summary>
        /// The main Refresh functionality
        /// Refreshes the buy/sell listings for any items currently watched
        /// </summary>
        private void Refresh(object state = null)
        {
            lock (this.refreshTimerLock)
            {
                // Don't do anything if we are supposed to be stopped
                if (this.isStopped)
                    return;

                // Using the UI thread, grab a snapshot of the current price watches
                List<ItemPriceViewModel> itemPrices = null;
                Threading.InvokeOnUI(() => itemPrices = new List<ItemPriceViewModel>(this.ItemPrices));

                if (itemPrices != null)
                {
                    // Ignore anything with an itemId of 0 or less
                    itemPrices.RemoveAll(pw => pw.Data.ItemID <= 0);

                    if (itemPrices.Count > 0)
                    {
                        // Retrieve price information for all price-watched items
                        var allPrices = this.commerceService.GetItemPrices(itemPrices.Select(pw => pw.Data.ItemID).ToArray());

                        foreach (var priceWatch in itemPrices)
                        {
                            // Determine if we need to reset the notifications shown-state
                            if (this.NotificationsResetDateTimes.ContainsKey(priceWatch))
                            {
                                var lastResetTime = this.NotificationsResetDateTimes[priceWatch];
                                if (DateTime.Now.Subtract(lastResetTime).TotalMilliseconds >= UserData.ResetPriceNotificationsInterval * 60000)
                                {
                                    priceWatch.IsBuyOrderNotificationShown = false;
                                    priceWatch.IsSellListingNotificationShown = false;
                                }
                            }
                            else
                            {
                                this.NotificationsResetDateTimes.Add(priceWatch, DateTime.Now);
                            }

                            if (allPrices.ContainsKey(priceWatch.Data.ItemID))
                            {
                                var prices = allPrices[priceWatch.Data.ItemID];
                                Threading.InvokeOnUI(() => priceWatch.CurrentBuyOrder.Value = prices.HighestBuyOrder);
                                Threading.InvokeOnUI(() => priceWatch.CurrentSellListing.Value = prices.LowestSellListing);
                                Threading.InvokeOnUI(() => priceWatch.CurrentProfit.Value = (prices.LowestSellListing * 0.85) - prices.HighestBuyOrder);

                                // Buy Order
                                bool buyOrderOutOfLimits = false;

                                if (priceWatch.Data.IsBuyOrderUpperLimitEnabled && priceWatch.Data.IsBuyOrderLowerLimitEnabled)
                                    buyOrderOutOfLimits = prices.HighestBuyOrder <= priceWatch.Data.BuyOrderUpperLimit.Value
                                                                && prices.HighestBuyOrder >= priceWatch.Data.BuyOrderLowerLimit.Value;
                                else if (priceWatch.Data.IsBuyOrderUpperLimitEnabled)
                                    buyOrderOutOfLimits = prices.HighestBuyOrder <= priceWatch.Data.BuyOrderUpperLimit.Value;
                                else if (priceWatch.Data.IsBuyOrderLowerLimitEnabled)
                                    buyOrderOutOfLimits = prices.HighestBuyOrder >= priceWatch.Data.BuyOrderLowerLimit.Value;

                                Threading.InvokeOnUI(() => priceWatch.IsBuyOrderOutOfLimits = buyOrderOutOfLimits);

                                if (buyOrderOutOfLimits)
                                {
                                    if (this.CanShowNotification(priceWatch, PriceNotificationType.BuyOrder))
                                    {
                                        priceWatch.IsBuyOrderNotificationShown = true;
                                        this.DisplayNotification(new PriceNotificationViewModel(priceWatch, PriceNotificationType.BuyOrder, prices.HighestBuyOrder, this.PriceNotifications));
                                        this.NotificationsResetDateTimes[priceWatch] = DateTime.Now;
                                    }
                                }

                                // Sell Listing
                                bool sellListingOutOfLimits = false;

                                if (priceWatch.Data.IsSellListingUpperLimitEnabled && priceWatch.Data.IsSellListingLowerLimitEnabled)
                                    sellListingOutOfLimits = prices.LowestSellListing <= priceWatch.Data.SellListingUpperLimit.Value
                                                                    && prices.LowestSellListing >= priceWatch.Data.SellListingLowerLimit.Value;
                                else if (priceWatch.Data.IsSellListingUpperLimitEnabled)
                                    sellListingOutOfLimits = prices.LowestSellListing <= priceWatch.Data.SellListingUpperLimit.Value;
                                else if (priceWatch.Data.IsSellListingLowerLimitEnabled)
                                    sellListingOutOfLimits = prices.LowestSellListing >= priceWatch.Data.SellListingLowerLimit.Value;

                                Threading.InvokeOnUI(() => priceWatch.IsSellListingOutOfLimits = sellListingOutOfLimits);

                                if (sellListingOutOfLimits)
                                {
                                    if (this.CanShowNotification(priceWatch, PriceNotificationType.SellListing))
                                    {
                                        priceWatch.IsSellListingNotificationShown = true;
                                        this.DisplayNotification(new PriceNotificationViewModel(priceWatch, PriceNotificationType.SellListing, prices.LowestSellListing, this.PriceNotifications));
                                        this.NotificationsResetDateTimes[priceWatch] = DateTime.Now;
                                    }
                                }
                            }

                            // Update the historical values for the item
                            Threading.BeginInvokeOnUI(() =>
                                {
                                    priceWatch.PastBuyOrders.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), priceWatch.CurrentBuyOrder.Value));
                                    priceWatch.PastSellListings.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), priceWatch.CurrentSellListing.Value));

                                    if (priceWatch.PastBuyOrders.Count > this.UserData.MaxHistoricalDataPoints)
                                        priceWatch.PastBuyOrders.RemoveAt(0);
                                    if (priceWatch.PastSellListings.Count > this.UserData.MaxHistoricalDataPoints)
                                        priceWatch.PastSellListings.RemoveAt(0);
                                });
                        }
                    }
                }


                // Additionally, update the Ectoplasm Salvage Threshold tracker data
                var ectoPrices = this.commerceService.GetItemPrices(EctoSalvageHelperViewModel.EctoplasmItemID);
                Threading.BeginInvokeOnUI(() =>
                {
                    this.EcoSalvageData.EctoplasmBuyOrder.Value = ectoPrices.HighestBuyOrder;
                    this.EcoSalvageData.EctoplasmSellListing.Value = ectoPrices.LowestSellListing;
                });

                this.refreshTimer.Change(this.RefreshInterval, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Adds an objective to the notifications collection, and then removes the objective 10 seconds later
        /// </summary>
        private void DisplayNotification(PriceNotificationViewModel priceNotification)
        {
            const int SLEEP_TIME = 250;

            if (!this.PriceNotifications.Contains(priceNotification))
            {
                Task.Factory.StartNew(() =>
                {
                    logger.Info("Displaying notification for \"{0}\" - {1}", priceNotification.ItemName, priceNotification.NotificationType);
                    Threading.BeginInvokeOnUI(() => this.PriceNotifications.Add(priceNotification));

                    if (this.UserData.NotificationDuration > 0)
                    {
                        // For X seconds, loop and sleep
                        for (int i = 0; i < (this.UserData.NotificationDuration * 1000 / SLEEP_TIME); i++)
                        {
                            System.Threading.Thread.Sleep(SLEEP_TIME);
                        }

                        logger.Debug("Removing notification for \"{0}\" - {1}", priceNotification.ItemName, priceNotification.NotificationType);

                        // TODO: I hate having this here, but due to a limitation in WPF, there's no reasonable way around this at this time
                        // This makes it so that the notifications can fade out before they are removed from the notification window
                        Threading.BeginInvokeOnUI(() => priceNotification.IsRemovingNotification = true);
                        System.Threading.Thread.Sleep(SLEEP_TIME);
                        Threading.BeginInvokeOnUI(() =>
                        {
                            this.PriceNotifications.Remove(priceNotification);
                            priceNotification.IsRemovingNotification = false;
                        });
                    }
                });
            }
        }

        /// <summary>
        /// Determines if we can show a notification for the given price notification, based on user settings
        /// </summary>
        /// <param name="objectiveData">The price watch's data</param>
        /// <param name="notificationType">The type of notification</param>
        /// <returns>True if the notification can be shown, else false</returns>
        private bool CanShowNotification(ItemPriceViewModel priceWatch, PriceNotificationType notificationType)
        {
            bool canShow = false;

            switch (notificationType)
            {
                case PriceNotificationType.BuyOrder:
                    canShow = this.UserData.AreBuyOrderPriceNotificationsEnabled && !priceWatch.IsBuyOrderNotificationShown;
                    break;
                case PriceNotificationType.SellListing:
                    canShow = this.UserData.AreSellListingPriceNotificationsEnabled && !priceWatch.IsSellListingNotificationShown;
                    break;
                default:
                    break;
            }

            return canShow;
        }
    }
}
