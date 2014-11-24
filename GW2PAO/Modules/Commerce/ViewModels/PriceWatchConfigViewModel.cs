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

namespace GW2PAO.Modules.Commerce.ViewModels
{
    [Export]
    public class PriceWatchConfigViewModel : BindableBase
    {
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
        public PriceWatchConfigViewModel(ICommerceService commerceService, ICommerceController controller)
        {
            this.commerceService = commerceService;
            this.controller = controller;
        }

        /// <summary>
        /// Adds a new price watch
        /// </summary>
        private void AddPriceWatch()
        {
            var priceWatch = new PriceWatch();
            var priceWatchVm = new ItemPriceViewModel(priceWatch, null, this.controller, this.commerceService);
            this.controller.UserData.PriceWatches.Add(priceWatch);
            this.ItemPrices.Add(priceWatchVm);
        }
    }
}