using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.Models;
using GW2PAO.PresentationCore;

namespace GW2PAO.ViewModels.PriceNotification
{
    /// <summary>
    /// Price watch model class
    /// TODO: Move this to a difference namespace
    /// </summary>
    public class PriceWatch : NotifyPropertyChangedBase
    {
        private int itemId;
        private string itemName;
        private bool isBuyOrderNotificationEnabled;
        private Price buyOrderLimit;
        private bool isSellListingNotificationEnabled;
        private Price sellListingLimit;

        /// <summary>
        /// ID of the item
        /// </summary>
        public int ItemID
        {
            get { return this.itemId; }
            set { this.SetField(ref this.itemId, value); }
        }

        /// <summary>
        /// Name of the item
        /// </summary>
        public string ItemName
        {
            get { return this.itemName; }
            set { this.SetField(ref this.itemName, value); }
        }

        /// <summary>
        /// True if the buy order notification is enabled for this item, else false
        /// </summary>
        public bool IsBuyOrderNotificationEnabled
        {
            get { return this.isBuyOrderNotificationEnabled; }
            set { this.SetField(ref this.isBuyOrderNotificationEnabled, value); }
        }

        /// <summary>
        /// The buy order limit
        /// </summary>
        public Price BuyOrderLimit
        {
            get { return this.buyOrderLimit; }
            set { this.SetField(ref this.buyOrderLimit, value); }
        }

        /// <summary>
        /// True if the sell listing notification is enabled for this item, else false
        /// </summary>
        public bool IsSellListingNotificationEnabled
        {
            get { return this.isSellListingNotificationEnabled; }
            set { this.SetField(ref this.isSellListingNotificationEnabled, value); }
        }

        /// <summary>
        /// The sell listing limit
        /// </summary>
        public Price SellListingLimit
        {
            get { return this.sellListingLimit; }
            set { this.SetField(ref this.sellListingLimit, value); }
        }

        /// <summary>
        /// Default constructor for serialization/deserialization
        /// </summary>
        public PriceWatch()
        {
            this.itemId = 0;
            this.itemName = string.Empty;
            this.isBuyOrderNotificationEnabled = false;
            this.isSellListingNotificationEnabled = false;
            this.buyOrderLimit = new Price();
            this.sellListingLimit = new Price();
        }

        /// <summary>
        /// Secondary constructor
        /// </summary>
        /// <param name="itemId">ID for the item this price watch is for</param>
        /// <param name="itemName">Name of the item this price watch is for</param>
        public PriceWatch(int itemId, string itemName)
            : this()
        {
            this.itemId = itemId;
            this.itemName = itemName;
        }
    }
}
