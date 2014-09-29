using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.Models;
using GW2PAO.Utility;
using GW2PAO.ViewModels.PriceNotification;
using GW2PAO.ViewModels.TradingPost;
using NLog;

namespace GW2PAO.Controllers
{
    /// <summary>
    /// Controller class responsible for showing notifications for the price watch feature
    /// May expand in the future
    /// </summary>
    public class CommerceController : ICommerceController
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The commerce service object
        /// </summary>
        private ICommerceService commerceService;

        /// <summary>
        /// Collection used for keeping track of when to reset the shown-state of notifications
        /// </summary>
        private Dictionary<PriceWatchViewModel, DateTime> NotificationsResetDateTimes = new Dictionary<PriceWatchViewModel, DateTime>();

        /// <summary>
        /// Keeps track of how many times Start() has been called in order
        /// to support reuse of a single object
        /// </summary>
        private int startCallCount;

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
        /// The commers user settings
        /// </summary>
        public CommerceSettings UserSettings { get; private set; }

        /// <summary>
        /// Collection of price watches for the price watch notifications
        /// </summary>
        public ObservableCollection<PriceWatchViewModel> PriceWatches { get; private set; }

        /// <summary>
        /// Collection of price notifications currently shown to the user
        /// </summary>
        public ObservableCollection<PriceNotificationViewModel> PriceNotifications { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dungeonsService">The dungeons service object</param>
        /// <param name="userSettings">The dungeons user settings object</param>
        public CommerceController(ICommerceService commerceService, CommerceSettings userSettings)
        {
            logger.Debug("Initializing Commerce Controller");
            this.commerceService = commerceService;
            this.UserSettings = userSettings;
            this.PriceWatches = new ObservableCollection<PriceWatchViewModel>();
            this.PriceNotifications = new ObservableCollection<PriceNotificationViewModel>();

            this.InitializePriceWatches();

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
                    logger.Debug("Starting refresh timers");
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
                    this.refreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Initializes the collection of price watches
        /// </summary>
        private void InitializePriceWatches()
        {
            // If for some reason there are items with an ID of 0, just remove them now
            var itemsToRemove = new List<PriceWatch>(this.UserSettings.PriceWatches.Where(pw => pw.ItemID == 0));
            foreach (var emptyItem in itemsToRemove)
                this.UserSettings.PriceWatches.Remove(emptyItem);

            if (this.UserSettings.PriceWatches.Count > 0)
            {
                var itemIds = this.UserSettings.PriceWatches.Select(pw => pw.ItemID);
                var itemData = this.commerceService.GetItems(itemIds.ToArray());

                // Initialize view models
                foreach (var priceWatch in this.UserSettings.PriceWatches)
                {
                    if (priceWatch.ItemID > 0)
                        this.PriceWatches.Add(new PriceWatchViewModel(priceWatch, itemData[priceWatch.ItemID], this, this.commerceService));
                    else
                        this.PriceWatches.Add(new PriceWatchViewModel(priceWatch, null, this, this.commerceService));
                }
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
                // Using the UI thread, grab a snapshot of the current price watches
                List<PriceWatchViewModel> priceWatches = null;
                Threading.InvokeOnUI(() => priceWatches = new List<PriceWatchViewModel>(this.PriceWatches));

                if (priceWatches != null)
                {
                    // Ignore anything with an itemId of 0 or less
                    priceWatches.RemoveAll(pw => pw.Data.ItemID <= 0);

                    // Retrieve price information for all price-watched items
                    var allPrices = this.commerceService.GetItemPrices(priceWatches.Select(pw => pw.Data.ItemID).ToArray());

                    foreach (var priceWatch in priceWatches)
                    {
                        // Determine if we need to reset the notifications shown-state
                        if (this.NotificationsResetDateTimes.ContainsKey(priceWatch))
                        {
                            var lastResetTime = this.NotificationsResetDateTimes[priceWatch];
                            if (DateTime.Now.Subtract(lastResetTime).TotalMilliseconds >= UserSettings.ResetPriceNotificationsInterval * 60000)
                            {
                                priceWatch.IsBuyOrderNotificationShown = false;
                                priceWatch.IsSellListingNotificationShown = false;
                            }
                        }
                        else
                        {
                            this.NotificationsResetDateTimes.Add(priceWatch, DateTime.Now);
                        }

                        var prices = allPrices[priceWatch.Data.ItemID];
                        //var prices = this.commerceService.GetItemPrices(priceWatch.Data.ItemID);
                        Threading.InvokeOnUI(() => priceWatch.CurrentBuyOrder.Value = prices.HighestBuyOrder);
                        Threading.InvokeOnUI(() => priceWatch.CurrentSellListing.Value = prices.LowestSellListing);

                        // Buy Order
                        bool displayBuyOrderNotification = false;

                        if (priceWatch.Data.IsBuyOrderUpperLimitEnabled && priceWatch.Data.IsBuyOrderLowerLimitEnabled)
                            displayBuyOrderNotification = prices.HighestBuyOrder <= priceWatch.Data.BuyOrderUpperLimit.Value
                                                        && prices.HighestBuyOrder >= priceWatch.Data.BuyOrderLowerLimit.Value;
                        else if (priceWatch.Data.IsBuyOrderUpperLimitEnabled)
                            displayBuyOrderNotification = prices.HighestBuyOrder <= priceWatch.Data.BuyOrderUpperLimit.Value;
                        else if (priceWatch.Data.IsBuyOrderLowerLimitEnabled)
                            displayBuyOrderNotification = prices.HighestBuyOrder >= priceWatch.Data.BuyOrderLowerLimit.Value;

                        if (displayBuyOrderNotification)
                        {
                            if (this.CanShowNotification(priceWatch, PriceNotificationType.BuyOrder))
                            {
                                priceWatch.IsBuyOrderNotificationShown = true;
                                this.DisplayNotification(new PriceNotificationViewModel(priceWatch, PriceNotificationType.BuyOrder, prices.HighestBuyOrder, this.PriceNotifications));
                                this.NotificationsResetDateTimes[priceWatch] = DateTime.Now;
                            }
                        }

                        // Sell Listing
                        bool displaySellListingNotification = false;

                        if (priceWatch.Data.IsSellListingUpperLimitEnabled && priceWatch.Data.IsSellListingLowerLimitEnabled)
                            displaySellListingNotification = prices.LowestSellListing <= priceWatch.Data.SellListingUpperLimit.Value
                                                            && prices.LowestSellListing >= priceWatch.Data.SellListingLowerLimit.Value;
                        else if (priceWatch.Data.IsSellListingUpperLimitEnabled)
                            displaySellListingNotification = prices.LowestSellListing <= priceWatch.Data.SellListingUpperLimit.Value;
                        else if (priceWatch.Data.IsSellListingLowerLimitEnabled)
                            displaySellListingNotification = prices.LowestSellListing >= priceWatch.Data.SellListingLowerLimit.Value;

                        if (displaySellListingNotification)
                        {
                            if (this.CanShowNotification(priceWatch, PriceNotificationType.SellListing))
                            {
                                priceWatch.IsSellListingNotificationShown = true;
                                this.DisplayNotification(new PriceNotificationViewModel(priceWatch, PriceNotificationType.SellListing, prices.LowestSellListing, this.PriceNotifications));
                                this.NotificationsResetDateTimes[priceWatch] = DateTime.Now;
                            }
                        }
                    }
                }

                this.refreshTimer.Change(this.RefreshInterval, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Adds an objective to the notifications collection, and then removes the objective 10 seconds later
        /// </summary>
        private void DisplayNotification(PriceNotificationViewModel priceNotification)
        {
            
            Task.Factory.StartNew(() =>
            {
                logger.Debug("Adding notification for \"{0}\" - {1}", priceNotification.ItemName, priceNotification.NotificationType);
                Threading.InvokeOnUI(() => this.PriceNotifications.Add(priceNotification));

                // TODO: Consider making these stay open forever, until the user closes it
                // For 20 seconds, loop and sleep, with checks to see if notifications have been disabled
                //for (int i = 0; i < 40; i++)
                //{
                //    System.Threading.Thread.Sleep(500);
                //    if (!this.CanShowNotification(priceNotification.PriceWatch, priceNotification.NotificationType))
                //    {
                //        logger.Debug("Removing notification for \"{0}\" - {1}", priceNotification.ItemName, priceNotification.NotificationType);
                //        Threading.InvokeOnUI(() => 
                //            {
                //                priceNotification.IsRemovingNotification = true;
                //                this.PriceNotifications.Remove(priceNotification);
                //                priceNotification.IsRemovingNotification = false;
                //            });
                //        return;
                //    }
                //}
                System.Threading.Thread.Sleep(20000);

                logger.Debug("Removing notification for \"{0}\" - {1}", priceNotification.ItemName, priceNotification.NotificationType);

                // TODO: I hate having this here, but due to a limitation in WPF, there's no reasonable way around this at this time
                // This makes it so that the notifications can fade out before they are removed from the notification window
                Threading.InvokeOnUI(() => priceNotification.IsRemovingNotification = true);
                System.Threading.Thread.Sleep(250);
                Threading.InvokeOnUI(() =>
                {
                    this.PriceNotifications.Remove(priceNotification);
                    priceNotification.IsRemovingNotification = false;
                });
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Determines if we can show a notification for the given price notification, based on user settings
        /// </summary>
        /// <param name="objectiveData">The price watch's data</param>
        /// <param name="notificationType">The type of notification</param>
        /// <returns>True if the notification can be shown, else false</returns>
        private bool CanShowNotification(PriceWatchViewModel priceWatch, PriceNotificationType notificationType)
        {
            bool canShow = false;

            switch (notificationType)
            {
                case PriceNotificationType.BuyOrder:
                    canShow = this.UserSettings.AreBuyOrderPriceNotificationsEnabled && !priceWatch.IsBuyOrderNotificationShown;
                    break;
                case PriceNotificationType.SellListing:
                    canShow = this.UserSettings.AreSellListingPriceNotificationsEnabled && !priceWatch.IsSellListingNotificationShown;
                    break;
                default:
                    break;
            }

            return canShow;
        }
    }
}
