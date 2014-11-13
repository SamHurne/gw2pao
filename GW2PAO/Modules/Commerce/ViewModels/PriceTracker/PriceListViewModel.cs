using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Data.UserData;
using GW2PAO.Modules.Commerce.Interfaces;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.Modules.Commerce.ViewModels.PriceTracker
{
    [Export]
    public class PriceListViewModel : NotifyPropertyChangedBase
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
        /// Collection of all World Events
        /// </summary>
        public ObservableCollection<ItemPriceViewModel> ItemPrices { get { return this.controller.ItemPrices; } }

        /// <summary>
        /// Commerce user data
        /// </summary>
        public CommerceUserData UserData { get { return this.controller.UserData; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="commerceController">The commerce controller</param>
        [ImportingConstructor]
        public PriceListViewModel(ICommerceController commerceController)
        {
            this.controller = commerceController;
        }
    }
}
