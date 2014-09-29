using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.Models;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.ViewModels.PriceNotification
{
    public class PriceWatchViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private bool isBuyOrderNotificationShown;
        private bool isSellListingNotificationShown;

        /// <summary>
        /// Service object, used for looking up names, etc
        /// </summary>
        private ICommerceService commerceService;

        /// <summary>
        /// Commerce controller object
        /// </summary>
        private ICommerceController controller;

        /// <summary>
        /// The price watch model data
        /// </summary>
        public PriceWatch Data { get; private set; }

        /// <summary>
        /// Item data
        /// </summary>
        public Item ItemData { get; private set; }

        /// <summary>
        /// The current lowest sell listing
        /// </summary>
        public Price CurrentSellListing { get; private set; }

        /// <summary>
        /// The current highest buy order
        /// </summary>
        public Price CurrentBuyOrder { get; private set; }

        /// <summary>
        /// Name of the item
        /// </summary>
        public string ItemName
        {
            get { return this.Data.ItemName; }
            set
            {
                if (this.Data.ItemName != value)
                {
                    // Do a search to see if the item exists
                    if (this.commerceService.DoesItemExist(value))
                    {
                        // It exists, so update our model data
                        this.ItemData = this.commerceService.GetItem(value);
                        this.Data.ItemID = this.ItemData.ID;
                        this.Data.ItemName = this.ItemData.Name;
                        this.Data.BuyOrderUpperLimit.Value = this.ItemData.Prices.HighestBuyOrder + 1; // +1 so we don't immediately do a notification
                        this.Data.BuyOrderLowerLimit.Value = this.ItemData.Prices.HighestBuyOrder - 1; // +1 so we don't immediately do a notification
                        this.Data.SellListingUpperLimit.Value = this.ItemData.Prices.LowestSellListing + 1; // -1 so we don't immediately do a notification
                        this.Data.SellListingLowerLimit.Value = this.ItemData.Prices.LowestSellListing - 1; // -1 so we don't immediately do a notification
                        this.CurrentBuyOrder.Value = this.ItemData.Prices.HighestBuyOrder;
                        this.CurrentSellListing.Value = this.ItemData.Prices.LowestSellListing;

                        this.RaisePropertyChanged();

                        // Also raise property changed for the icon
                        this.RaisePropertyChanged("IconUrl");
                    }
                }
            }
        }

        /// <summary>
        /// The full collection of item names
        /// </summary>
        public ICollection<string> ItemNames
        {
            get { return this.commerceService.ItemNames.Values; }
        }

        /// <summary>
        /// Path to the icon url
        /// </summary>
        public string IconUrl
        {
            get
            {
                if (this.ItemData == null || this.ItemData.Icon == null)
                {
                    return "/Resources/unknown_icon.png";
                }
                else
                {
                    return this.ItemData.Icon.ToString();
                }
            }
        }

        /// <summary>
        /// True if the buy order notification has been shown, else false
        /// </summary>
        public bool IsBuyOrderNotificationShown
        {
            get { return this.isBuyOrderNotificationShown; }
            set { this.SetField(ref this.isBuyOrderNotificationShown, value); }
        }

        /// <summary>
        /// True if the buy order notification has been shown, else false
        /// </summary>
        public bool IsSellListingNotificationShown
        {
            get { return this.isSellListingNotificationShown; }
            set { this.SetField(ref this.isSellListingNotificationShown, value); }
        }

        /// <summary>
        /// Removes the price watch
        /// </summary>
        public DelegateCommand RemoveCommand { get { return new DelegateCommand(this.Remove); } }

        /// <summary>
        /// Copies the Item's name to the clipboard
        /// </summary>
        public DelegateCommand CopyNameCommand { get { return new DelegateCommand(this.CopyNameToClipboard); } }

        /// <summary>
        /// Copies the Item's Chatcode to the clipboard
        /// </summary>
        public DelegateCommand CopyChatcodeCommand { get { return new DelegateCommand(this.CopyChatcodeToClipboard); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="modelData">The price watch model data</param>
        /// <param name="controller">Commerce controller</param>
        public PriceWatchViewModel(PriceWatch priceData, Item itemData, ICommerceController controller, ICommerceService service)
        {
            this.Data = priceData;
            this.ItemData = itemData;
            this.controller = controller;
            this.commerceService = service;
            this.CurrentBuyOrder = new Price();
            this.CurrentSellListing = new Price();

            this.IsBuyOrderNotificationShown = false;
            this.IsSellListingNotificationShown = false;

            this.Data.PropertyChanged += Data_PropertyChanged;
            this.Data.BuyOrderUpperLimit.PropertyChanged += BuyOrderLimit_PropertyChanged;
            this.Data.BuyOrderLowerLimit.PropertyChanged += BuyOrderLimit_PropertyChanged;
            this.Data.SellListingUpperLimit.PropertyChanged += SellListingLimit_PropertyChanged;
            this.Data.SellListingLowerLimit.PropertyChanged += SellListingLimit_PropertyChanged;
        }

        /// <summary>
        /// Handles the PropertyChanged event of the PriceWatch data object
        /// </summary>
        private void Data_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsBuyOrderUpperLimitEnabled" || e.PropertyName == "IsBuyOrderLowerLimitEnabled")
            {
                this.IsBuyOrderNotificationShown = false;
            }
            else if (e.PropertyName == "IsSellListingUpperLimitEnabled" || e.PropertyName == "IsSellListingLowerLimitEnabled")
            {
                this.IsSellListingNotificationShown = false;
            }
            else
            {
                // Name or ID changed, so reset both
                this.IsBuyOrderNotificationShown = false;
                this.IsSellListingNotificationShown = false;
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the BuyOrderLimit
        /// </summary>
        private void BuyOrderLimit_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // If the value changes, reset our notification shown flag
            this.IsBuyOrderNotificationShown = false;
        }

        /// <summary>
        /// Handles the PropertyChanged event of the SellListingLimit
        /// </summary>
        private void SellListingLimit_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // If the value changes, reset our notification shown flag
            this.IsSellListingNotificationShown = false;
        }

        /// <summary>
        /// Removes the item from the collection of price watches
        /// </summary>
        private void Remove()
        {
            this.controller.UserSettings.PriceWatches.Remove(this.Data);
            this.controller.PriceWatches.Remove(this);
        }

        /// <summary>
        /// Copies the name of the item to the clipboard
        /// </summary>
        private void CopyNameToClipboard()
        {
            logger.Debug("Copying name of \"{0}\" ", this.ItemName);
            System.Windows.Clipboard.SetText(this.ItemName);
        }

        /// <summary>
        /// Copies the chatcode of the item to the clipboard
        /// </summary>
        private void CopyChatcodeToClipboard()
        {
            logger.Debug("Copying chatcode of \"{0}\" as \"{1}\" ", this.ItemName, this.ItemData.ChatCode);
            System.Windows.Clipboard.SetText(this.ItemData.ChatCode);
        }
    }
}
