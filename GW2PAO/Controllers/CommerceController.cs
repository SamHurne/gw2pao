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

            // Initialize view models
            foreach (var priceWatch in this.UserSettings.PriceWatches)
            {
                this.PriceWatches.Add(new PriceWatchViewModel(priceWatch, this, this.commerceService));
            }

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

            //////////////////////////////////////////////////////////
            //"26836":"Berserker's Iron Shield of Battle"
            //var testPriceWatch = new PriceWatch(26836, "Berserker's Iron Shield of Battle");
            //var existing = this.PriceWatches.FirstOrDefault(pw => pw.Data.ItemID == testPriceWatch.ItemID);
            //if (existing == null)
            //{
            //    testPriceWatch.IsBuyOrderNotificationEnabled = true;
            //    testPriceWatch.BuyOrderLimit.Value = 3133;
            //    testPriceWatch.IsSellListingNotificationEnabled = true;
            //    testPriceWatch.SellListingLimit.Value = 4498;
            //    this.PriceWatches.Add(new PriceWatchViewModel(testPriceWatch, this, this.commerceService));
            //}
            //else
            //{
            //    existing.Data.IsBuyOrderNotificationEnabled = true;
            //    existing.Data.BuyOrderLimit.Value = 3133;
            //    existing.Data.IsSellListingNotificationEnabled = true;
            //    existing.Data.SellListingLimit.Value = 4498;
            //}
            ////"891","Honed Cabalist Boots of Lyssa"
            //var testPriceWatch2 = new PriceWatch(891, "Honed Cabalist Boots of Lyssa");
            //var existing2 = this.PriceWatches.FirstOrDefault(pw => pw.Data.ItemID == testPriceWatch2.ItemID);
            //if (existing2 == null)
            //{
            //    testPriceWatch2.IsBuyOrderNotificationEnabled = true;
            //    testPriceWatch2.BuyOrderLimit.Value = 100;
            //    testPriceWatch2.IsSellListingNotificationEnabled = true;
            //    testPriceWatch2.SellListingLimit.Value = 900;
            //    this.PriceWatches.Add(new PriceWatchViewModel(testPriceWatch2, this, this.commerceService));
            //}
            //else
            //{
            //    existing2.Data.IsBuyOrderNotificationEnabled = true;
            //    existing2.Data.BuyOrderLimit.Value = 100;
            //    existing2.Data.IsSellListingNotificationEnabled = true;
            //    existing2.Data.SellListingLimit.Value = 900;
            //}
            ////"32818","Carrion Steam Gizmo of Agony"
            //var testPriceWatch3 = new PriceWatch(32818, "Carrion Steam Gizmo of Agony");
            //var existing3 = this.PriceWatches.FirstOrDefault(pw => pw.Data.ItemID == testPriceWatch3.ItemID);
            //if (existing3 == null)
            //{
            //    testPriceWatch3.IsBuyOrderNotificationEnabled = true;
            //    testPriceWatch3.BuyOrderLimit.Value = 100;
            //    testPriceWatch3.IsSellListingNotificationEnabled = true;
            //    testPriceWatch3.SellListingLimit.Value = 300;
            //    this.PriceWatches.Add(new PriceWatchViewModel(testPriceWatch3, this, this.commerceService));
            //}
            //else
            //{
            //    existing3.Data.IsBuyOrderNotificationEnabled = true;
            //    existing3.Data.BuyOrderLimit.Value = 100;
            //    existing3.Data.IsSellListingNotificationEnabled = true;
            //    existing3.Data.SellListingLimit.Value = 300;
            //}
            //////////////////////////////////////////////////////////

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
                    // TODO: There is a bug that prevents this from working, for now, just request one at a time
                    //var allPrices = this.commerceService.GetItemPrices(priceWatches.Select(pw => pw.Data.ItemID).ToArray());

                    foreach (var priceWatch in priceWatches)
                    {
                        //var prices = allPrices[priceWatch.Data.ItemID];
                        var prices = this.commerceService.GetItemPrices(priceWatch.Data.ItemID);

                        // Buy Order
                        if (prices.HighestBuyOrder >= priceWatch.Data.BuyOrderLimit.Value)
                        {
                            this.DisplayNotification(new PriceNotificationViewModel(priceWatch, PriceNotificationType.BuyOrder, prices.HighestBuyOrder, this.PriceNotifications));
                        }

                        // Sell Listing
                        if (prices.LowestSellListing <= priceWatch.Data.SellListingLimit.Value)
                        {
                            this.DisplayNotification(new PriceNotificationViewModel(priceWatch, PriceNotificationType.SellListing, prices.LowestSellListing, this.PriceNotifications));
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
            if (this.CanShowNotification(priceNotification))
            {
                Task.Factory.StartNew(() =>
                {
                    logger.Debug("Adding notification for \"{0}\" - {1}", priceNotification.ItemName, priceNotification.NotificationType);
                    Threading.InvokeOnUI(() => this.PriceNotifications.Add(priceNotification));

                    // TODO: Consider making these stay open forever, until the user closes it
                    // For 20 seconds, loop and sleep, with checks to see if notifications have been disabled
                    for (int i = 0; i < 40; i++)
                    {
                        System.Threading.Thread.Sleep(500);
                        if (!this.CanShowNotification(priceNotification))
                        {
                            logger.Debug("Removing notification for \"{0}\" - {1}", priceNotification.ItemName, priceNotification.NotificationType);
                            Threading.InvokeOnUI(() => 
                                {
                                    priceNotification.IsRemovingNotification = true;
                                    this.PriceNotifications.Remove(priceNotification);
                                    priceNotification.IsRemovingNotification = false;
                                });
                            return;
                        }
                    }

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
        }

        /// <summary>
        /// Determines if we can show a notification for the given price notification, based on user settings
        /// </summary>
        /// <param name="objectiveData">The price notification's data</param>
        /// <returns>True if the notification can be shown, else false</returns>
        private bool CanShowNotification(PriceNotificationViewModel priceNotification)
        {
            bool canShow = false;

            switch (priceNotification.NotificationType)
            {
                case PriceNotificationType.BuyOrder:
                    canShow = priceNotification.PriceWatch.Data.IsBuyOrderNotificationEnabled && !priceNotification.PriceWatch.IsBuyOrderNotificationShown;
                    break;
                case PriceNotificationType.SellListing:
                    canShow = priceNotification.PriceWatch.Data.IsSellListingNotificationEnabled && !priceNotification.PriceWatch.IsSellListingNotificationShown;
                    break;
                default:
                    break;
            }

            return canShow;
        }
    }
}
