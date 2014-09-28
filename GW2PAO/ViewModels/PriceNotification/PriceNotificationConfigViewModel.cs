using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.ViewModels.PriceNotification
{
    public class PriceNotificationConfigViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The commerce service
        /// </summary>
        private ICommerceService commerceService;

        /// <summary>
        /// The events controller
        /// </summary>
        private ICommerceController controller;

        /// <summary>
        /// Collection of active event notifications
        /// </summary>
        public ObservableCollection<PriceWatchViewModel> PriceWatches { get { return this.controller.PriceWatches; } }

        /// <summary>
        /// Command to add a new pricewatch
        /// </summary>
        public DelegateCommand AddCommand { get { return new DelegateCommand(this.AddPriceWatch); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="controller">The commerce controller</param>
        public PriceNotificationConfigViewModel(ICommerceService commerceService, ICommerceController controller)
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
            var priceWatchVm = new PriceWatchViewModel(priceWatch, null, this.controller, this.commerceService);
            this.controller.UserSettings.PriceWatches.Add(priceWatch);
            this.PriceWatches.Add(priceWatchVm);
        }
    }
}