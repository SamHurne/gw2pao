using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Data;
using GW2PAO.PresentationCore;
using GW2PAO.Utility;

namespace GW2PAO.ViewModels.Commerce.PriceNotification
{
    public class PriceNotificationViewModel : NotifyPropertyChangedBase
    {
        private ItemPriceViewModel itemPriceWatch;
        private PriceNotificationType notificationType;
        private Price price;
        private bool isRemovingNotification;
        private ICollection<PriceNotificationViewModel> displayedNotifications;

        /// <summary>
        /// ID of the item
        /// </summary>
        public int ItemID
        {
            get { return this.itemPriceWatch.Data.ItemID; }
        }

        /// <summary>
        /// Name of the item
        /// </summary>
        public string ItemName
        {
            get { return this.itemPriceWatch.ItemName; }
        }

        /// <summary>
        /// The type of notification (buy or sell)
        /// </summary>
        public PriceNotificationType NotificationType
        {
            get { return this.notificationType; }
        }

        /// <summary>
        /// The current price of the item
        /// </summary>
        public Price Price
        {
            get { return this.price; }
        }

        /// <summary>
        /// The original item price watch object
        /// </summary>
        public ItemPriceViewModel PriceWatch
        {
            get { return this.itemPriceWatch; }
        }

        /// <summary>
        /// True if the notification is going to be removed, else false
        /// </summary>
        public bool IsRemovingNotification
        {
            get { return this.isRemovingNotification; }
            set { this.SetField(ref this.isRemovingNotification, value); }
        }

        /// <summary>
        /// Closes the displayed notification
        /// </summary>
        public DelegateCommand CloseNotificationCommand { get { return new DelegateCommand(this.CloseNotification); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="itemPriceWatch">The item's price watch information</param>
        /// <param name="notificationType">The notification type</param>
        /// <param name="price">The current price of the item, in copper</param>
        public PriceNotificationViewModel(ItemPriceViewModel itemPriceWatch, PriceNotificationType notificationType, int price, ICollection<PriceNotificationViewModel> displayedNotificationsCollection)
        {
            this.itemPriceWatch = itemPriceWatch;
            this.notificationType = notificationType;
            this.price = new Price();
            this.price.Value = price;
            this.isRemovingNotification = false;
            this.displayedNotifications = displayedNotificationsCollection;
        }

        /// <summary>
        /// Removes this event from the collection of displayed notifications
        /// </summary>
        private void CloseNotification()
        {
            // Removing... set flag that this was shown
            switch (this.NotificationType)
            {
                case PriceNotificationType.BuyOrder:
                    this.PriceWatch.IsBuyOrderNotificationShown = true;
                    break;
                case PriceNotificationType.SellListing:
                    this.PriceWatch.IsSellListingNotificationShown = true;
                    break;
                default:
                    break;
            }

            Task.Factory.StartNew(() =>
                {
                    Threading.InvokeOnUI(() => this.IsRemovingNotification = true);
                    System.Threading.Thread.Sleep(250);
                    Threading.InvokeOnUI(() =>
                    {
                        this.displayedNotifications.Remove(this);
                        this.IsRemovingNotification = false;
                    });
                });
        }
    }

    public enum PriceNotificationType
    {
        BuyOrder,
        SellListing
    }
}
