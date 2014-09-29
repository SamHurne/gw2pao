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
        private bool isBuyOrderUpperLimitEnabled;
        private bool isBuyOrderLowerLimitEnabled;
        private Price buyOrderUpperLimit;
        private Price buyOrderLowerLimit;
        private bool isSellListingUpperLimitEnabled;
        private bool isSellListingLowerLimitEnabled;
        private Price sellListingUpperLimit;
        private Price sellListingLowerLimit;

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
        /// True if the buy order upper limit is enabled for this item, else false
        /// </summary>
        public bool IsBuyOrderUpperLimitEnabled
        {
            get { return this.isBuyOrderUpperLimitEnabled; }
            set { this.SetField(ref this.isBuyOrderUpperLimitEnabled, value); }
        }

        /// <summary>
        /// True if the buy order lower limit is enabled for this item, else false
        /// </summary>
        public bool IsBuyOrderLowerLimitEnabled
        {
            get { return this.isBuyOrderLowerLimitEnabled; }
            set { this.SetField(ref this.isBuyOrderLowerLimitEnabled, value); }
        }

        /// <summary>
        /// The buy order upper limit
        /// </summary>
        public Price BuyOrderUpperLimit
        {
            get { return this.buyOrderUpperLimit; }
            set { this.SetField(ref this.buyOrderUpperLimit, value); }
        }

        /// <summary>
        /// The buy order lower limit
        /// </summary>
        public Price BuyOrderLowerLimit
        {
            get { return this.buyOrderLowerLimit; }
            set { this.SetField(ref this.buyOrderLowerLimit, value); }
        }

        /// <summary>
        /// True if the sell listing upper limit is enabled for this item, else false
        /// </summary>
        public bool IsSellListingUpperLimitEnabled
        {
            get { return this.isSellListingUpperLimitEnabled; }
            set { this.SetField(ref this.isSellListingUpperLimitEnabled, value); }
        }

        /// <summary>
        /// True if the sell listing lower limit is enabled for this item, else false
        /// </summary>
        public bool IsSellListingLowerLimitEnabled
        {
            get { return this.isSellListingLowerLimitEnabled; }
            set { this.SetField(ref this.isSellListingLowerLimitEnabled, value); }
        }

        /// <summary>
        /// The sell listing upper limit
        /// </summary>
        public Price SellListingUpperLimit
        {
            get { return this.sellListingUpperLimit; }
            set { this.SetField(ref this.sellListingUpperLimit, value); }
        }

        /// <summary>
        /// The sell listing lower limit
        /// </summary>
        public Price SellListingLowerLimit
        {
            get { return this.sellListingLowerLimit; }
            set { this.SetField(ref this.sellListingLowerLimit, value); }
        }

        /// <summary>
        /// Default constructor for serialization/deserialization
        /// </summary>
        public PriceWatch()
        {
            this.itemId = 0;
            this.itemName = string.Empty;
            this.isBuyOrderUpperLimitEnabled = false;
            this.isBuyOrderLowerLimitEnabled = false;
            this.isSellListingUpperLimitEnabled = false;
            this.isSellListingLowerLimitEnabled = false;
            this.buyOrderUpperLimit = new Price();
            this.buyOrderLowerLimit = new Price();
            this.sellListingUpperLimit = new Price();
            this.sellListingLowerLimit = new Price();
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
