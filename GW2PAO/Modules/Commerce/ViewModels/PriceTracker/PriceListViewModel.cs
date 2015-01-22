using GW2PAO.Data.UserData;
using GW2PAO.Modules.Commerce.Interfaces;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.Infrastructure;
using System.ComponentModel;

namespace GW2PAO.Modules.Commerce.ViewModels.PriceTracker
{
    [Export]
    public class PriceListViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The Commerce controller
        /// </summary>
        private ICommerceController controller;

        /// <summary>
        /// Collection of watched Item Prices
        /// </summary>
        public AutoRefreshCollectionViewSource ItemPrices
        {
            get;
            private set;
        }

        /// <summary>
        /// Commerce user data
        /// </summary>
        public CommerceUserData UserData { get { return this.controller.UserData; } }

        /// <summary>
        /// True if the items should be sorted by Name, else false
        /// </summary>
        public bool SortByName
        {
            get
            {
                return this.UserData.PriceTrackerSortProperty == CommerceUserData.PRICE_TRACKER_SORT_NAME;
            }
            set
            {
                if (this.UserData.PriceTrackerSortProperty != CommerceUserData.PRICE_TRACKER_SORT_NAME)
                {
                    this.OnSortingPropertyChanged(CommerceUserData.PRICE_TRACKER_SORT_NAME, ListSortDirection.Ascending);
                }
            }
        }

        /// <summary>
        /// True if the items should be sorted by Buy Price, else false
        /// </summary>
        public bool SortByBuyPrice
        {
            get
            {
                return this.UserData.PriceTrackerSortProperty == CommerceUserData.PRICE_TRACKER_SORT_BUY_PRICE;
            }
            set
            {
                if (this.UserData.PriceTrackerSortProperty != CommerceUserData.PRICE_TRACKER_SORT_BUY_PRICE)
                {
                    this.OnSortingPropertyChanged(CommerceUserData.PRICE_TRACKER_SORT_BUY_PRICE, ListSortDirection.Ascending);
                }
            }
        }

        /// <summary>
        /// True if the items should be sorted by Sell Price, else false
        /// </summary>
        public bool SortBySellPrice
        {
            get
            {
                return this.UserData.PriceTrackerSortProperty == CommerceUserData.PRICE_TRACKER_SORT_SALE_PRICE;
            }
            set
            {
                if (this.UserData.PriceTrackerSortProperty != CommerceUserData.PRICE_TRACKER_SORT_SALE_PRICE)
                {
                    this.OnSortingPropertyChanged(CommerceUserData.PRICE_TRACKER_SORT_SALE_PRICE, ListSortDirection.Ascending);
                }
            }
        }

        /// <summary>
        /// True if the items should be sorted by Profit, else false
        /// </summary>
        public bool SortByProfit
        {
            get
            {
                return this.UserData.PriceTrackerSortProperty == CommerceUserData.PRICE_TRACKER_SORT_PROFIT;
            }
            set
            {
                if (this.UserData.PriceTrackerSortProperty != CommerceUserData.PRICE_TRACKER_SORT_PROFIT)
                {
                    this.OnSortingPropertyChanged(CommerceUserData.PRICE_TRACKER_SORT_PROFIT, ListSortDirection.Descending);
                }
            }
        }

        /// <summary>
        /// Opens the configuration screen for price-tracked items
        /// </summary>
        public ICommand ConfigureCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="commerceController">The commerce controller</param>
        [ImportingConstructor]
        public PriceListViewModel(ICommerceController commerceController)
        {
            this.controller = commerceController;
            this.ConfigureCommand = new DelegateCommand(() => Commands.OpenCommerceSettingsCommand.Execute(null));

            var collectionViewSource = new AutoRefreshCollectionViewSource();
            collectionViewSource.Source = this.controller.ItemPrices;
            this.ItemPrices = collectionViewSource;

            switch (this.UserData.PriceTrackerSortProperty)
            {
                case CommerceUserData.PRICE_TRACKER_SORT_NAME:
                    this.OnSortingPropertyChanged(CommerceUserData.PRICE_TRACKER_SORT_NAME, ListSortDirection.Ascending);
                    break;
                case CommerceUserData.PRICE_TRACKER_SORT_BUY_PRICE:
                    this.OnSortingPropertyChanged(CommerceUserData.PRICE_TRACKER_SORT_BUY_PRICE, ListSortDirection.Ascending);
                    break;
                case CommerceUserData.PRICE_TRACKER_SORT_SALE_PRICE:
                    this.OnSortingPropertyChanged(CommerceUserData.PRICE_TRACKER_SORT_SALE_PRICE, ListSortDirection.Ascending);
                    break;
                case CommerceUserData.PRICE_TRACKER_SORT_PROFIT:
                    this.OnSortingPropertyChanged(CommerceUserData.PRICE_TRACKER_SORT_PROFIT, ListSortDirection.Descending);
                    break;
                default:
                    this.OnSortingPropertyChanged(CommerceUserData.PRICE_TRACKER_SORT_NAME, ListSortDirection.Ascending);
                    break;
            }
        }

        /// <summary>
        /// Handles updating the sorting descriptions of the Objectives collection
        /// and raising INotifyPropertyChanged for all sort properties
        /// </summary>
        private void OnSortingPropertyChanged(string property, ListSortDirection direction)
        {
            this.ItemPrices.SortDescriptions.Clear();
            this.ItemPrices.SortDescriptions.Add(new SortDescription(property, direction));
            this.ItemPrices.View.Refresh();

            this.UserData.PriceTrackerSortProperty = property;
            this.OnPropertyChanged(() => this.SortByName);
            this.OnPropertyChanged(() => this.SortByBuyPrice);
            this.OnPropertyChanged(() => this.SortBySellPrice);
            this.OnPropertyChanged(() => this.SortByProfit);
        }
    }
}
