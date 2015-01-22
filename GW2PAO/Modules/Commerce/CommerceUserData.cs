using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.Data.UserData;
using GW2PAO.Modules.Commerce.Models;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.Modules.Commerce
{
    /// <summary>
    /// User data for Commerce overlays, like the price notifications
    /// </summary>
    [Serializable]
    public class CommerceUserData : UserData<CommerceUserData>
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public const string PRICE_TRACKER_SORT_NAME = "ItemName";
        public const string PRICE_TRACKER_SORT_BUY_PRICE = "CurrentBuyOrder";
        public const string PRICE_TRACKER_SORT_SALE_PRICE = "CurrentSellListing";
        public const string PRICE_TRACKER_SORT_PROFIT = "CurrentProfit";

        /// <summary>
        /// The default settings filename
        /// </summary>
        public const string Filename = "CommerceUserData.xml";

        private int resetPriceNotificationsInterval;
        private bool areBuyOrderPriceNotificationsEnabled;
        private bool areSellListingPriceNotificationsEnabled;
        private int maxHistoricalDataPoints;
        private string priceTrackerSortProperty;
        private ObservableCollection<PriceWatch> priceWatches = new ObservableCollection<PriceWatch>();

        /// <summary>
        /// Interval at which to automatically reset the shown-state of price notifications, in minutes
        /// </summary>
        public int ResetPriceNotificationsInterval
        {
            get { return this.resetPriceNotificationsInterval; }
            set { this.SetProperty(ref this.resetPriceNotificationsInterval, value); }
        }

        /// <summary>
        /// True if buy order price notifications are enabled, else false
        /// </summary>
        public bool AreBuyOrderPriceNotificationsEnabled
        {
            get { return areBuyOrderPriceNotificationsEnabled; }
            set { this.SetProperty(ref this.areBuyOrderPriceNotificationsEnabled, value); }
        }

        /// <summary>
        /// True if sell listing price notifications are enabled, else false
        /// </summary>
        public bool AreSellListingPriceNotificationsEnabled
        {
            get { return areSellListingPriceNotificationsEnabled; }
            set { this.SetProperty(ref this.areSellListingPriceNotificationsEnabled, value); }
        }

        /// <summary>
        /// The maximum amount of data points to store for historical data
        /// </summary>
        public int MaxHistoricalDataPoints
        {
            get { return this.maxHistoricalDataPoints; }
            set { this.SetProperty(ref this.maxHistoricalDataPoints, value); }
        }

        /// <summary>
        /// The property name to use when sorting items in the Price Tracker
        /// </summary>
        public string PriceTrackerSortProperty
        {
            get { return this.priceTrackerSortProperty; }
            set { this.SetProperty(ref priceTrackerSortProperty, value); }
        }

        /// <summary>
        /// Collection of price watches for the price watch notifications
        /// </summary>
        public ObservableCollection<PriceWatch> PriceWatches { get { return this.priceWatches; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CommerceUserData()
        {
            // Defaults:
            this.ResetPriceNotificationsInterval = 15;
            this.AreBuyOrderPriceNotificationsEnabled = true;
            this.AreSellListingPriceNotificationsEnabled = true;
            this.MaxHistoricalDataPoints = 600; // 600 data points = 10 hours of data
        }

        /// <summary>
        /// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
        /// </summary>
        public override void EnableAutoSave()
        {
            logger.Info("Enabling auto save");
            this.PropertyChanged += (o, e) => CommerceUserData.SaveData(this, CommerceUserData.Filename);
            this.PriceWatches.CollectionChanged += PriceWatches_CollectionChanged;

            foreach (var pw in this.PriceWatches)
            {
                pw.PropertyChanged += (o, arg) => CommerceUserData.SaveData(this, CommerceUserData.Filename);
                pw.BuyOrderLowerLimit.PropertyChanged += (o, arg) => CommerceUserData.SaveData(this, CommerceUserData.Filename);
                pw.BuyOrderUpperLimit.PropertyChanged += (o, arg) => CommerceUserData.SaveData(this, CommerceUserData.Filename);
                pw.SellListingLowerLimit.PropertyChanged += (o, arg) => CommerceUserData.SaveData(this, CommerceUserData.Filename);
                pw.SellListingUpperLimit.PropertyChanged += (o, arg) => CommerceUserData.SaveData(this, CommerceUserData.Filename);
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the PriceWatches collection
        /// </summary>
        private void PriceWatches_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // If an item is added, register for it's property changed event so we can save settings when it changes
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (PriceWatch newItem in e.NewItems)
                {
                    newItem.PropertyChanged += (o, arg) => CommerceUserData.SaveData(this, CommerceUserData.Filename);
                }
            }

            CommerceUserData.SaveData(this, CommerceUserData.Filename);
        }
    }
}
