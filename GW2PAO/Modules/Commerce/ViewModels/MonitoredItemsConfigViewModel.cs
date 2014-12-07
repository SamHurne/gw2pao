using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Data;
using GW2PAO.Modules.Commerce.Interfaces;
using GW2PAO.Modules.Commerce.Models;
using GW2PAO.PresentationCore;
using NLog;
using Microsoft.Practices.Prism.Mvvm;
using GW2PAO.Modules.Commerce.Services;
using FeserWard.Controls;
using GW2PAO.API.Data.Entities;

namespace GW2PAO.Modules.Commerce.ViewModels
{
    [Export]
    public class MonitoredItemsConfigViewModel : BindableBase
    {
        private ItemDBEntry selectedItem;

        /// <summary>
        /// Default logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The commerce service
        /// </summary>
        private ICommerceService commerceService;

        /// <summary>
        /// The events controller
        /// </summary>
        private ICommerceController controller;

        /// <summary>
        /// Interval at which to reset the shown-state of price notifications
        /// </summary>
        public int ResetPriceNotificationsInterval
        {
            get { return this.controller.UserData.ResetPriceNotificationsInterval; }
            set
            {
                if (this.controller.UserData.ResetPriceNotificationsInterval != value)
                {
                    this.controller.UserData.ResetPriceNotificationsInterval = value;
                    this.OnPropertyChanged(() => this.ResetPriceNotificationsInterval);
                }
            }
        }

        /// <summary>
        /// Provider object for finding an item for the user to select
        /// </summary>
        public IIntelliboxResultsProvider ItemsProvider
        {
            get;
            private set;
        }

        /// <summary>
        /// The item selected by the user when configuring this price watch
        /// </summary>
        public ItemDBEntry SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                if (this.SetProperty(ref this.selectedItem, value))
                {
                    if (this.selectedItem != null)
                        this.ItemToAdd.SetItem(this.selectedItem.ID);
                    else
                        this.ItemToAdd = new ItemPriceViewModel(new PriceWatch(), new Item(-1, null), this.controller, this.commerceService);
                }
            }
        }

        /// <summary>
        /// The item that will be added to the collection of monitored item prices
        /// </summary>
        private ItemPriceViewModel ItemToAdd
        {
            get;
            set;
        }

        /// <summary>
        /// Collection of monitored item prices
        /// </summary>
        public ObservableCollection<ItemPriceViewModel> ItemPrices { get { return this.controller.ItemPrices; } }

        /// <summary>
        /// Command to add a new pricewatch
        /// </summary>
        public DelegateCommand AddCommand { get { return new DelegateCommand(this.AddPriceWatch); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="controller">The commerce controller</param>
        [ImportingConstructor]
        public MonitoredItemsConfigViewModel(ICommerceService commerceService, ICommerceController controller)
        {
            this.commerceService = commerceService;
            this.controller = controller;

            this.ItemsProvider = new ItemResultsProvider(this.commerceService);
            this.ItemToAdd = new ItemPriceViewModel(new PriceWatch(), new Item(-1, null), this.controller, this.commerceService);
        }

        /// <summary>
        /// Adds a new price watch
        /// </summary>
        private void AddPriceWatch()
        {
            if (this.ItemToAdd.ItemData.ID != -1)
            {
                this.controller.UserData.PriceWatches.Add(this.ItemToAdd.Data);
                this.ItemPrices.Add(this.ItemToAdd);
                this.SelectedItem = null;
            }
        }
    }
}