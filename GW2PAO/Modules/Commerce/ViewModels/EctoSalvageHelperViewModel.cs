using GW2PAO.Data;
using GW2PAO.Modules.Commerce.Models;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.Commerce.ViewModels
{
    public class EctoSalvageHelperViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public const int EctoplasmItemID = 19721;
        public const double EctoplasmChanceCoeff = 0.875; // see wiki
        public const double SalvageCost = 10.496; // 26s24c for 250 uses

        /// <summary>
        /// The Commerce User Data
        /// </summary>
        private CommerceUserData userData;

        /// <summary>
        /// Price of the highest buy order for Ectoplasm
        /// </summary>
        public Price EctoplasmBuyOrder { get; private set; }

        /// <summary>
        /// Price of the lowest sell listing for Ectoplasm
        /// </summary>
        public Price EctoplasmSellListing { get; private set; }

        /// <summary>
        /// Cost of Tax
        /// </summary>
        public Price Tax { get; private set; }

        /// <summary>
        /// The final calculated salvage threshold
        /// </summary>
        public Price SalvageThreshold { get; private set; }

        /// <summary>
        /// True if the Buy Order price should be used for ectoplasm prices
        /// </summary>
        public bool UseBuyOrder
        {
            get { return !this.userData.EctoplasmThresholdUsesSellListing; }
            set
            {
                if (this.userData.EctoplasmThresholdUsesSellListing != !value)
                {
                    this.userData.EctoplasmThresholdUsesSellListing = !value;

                    this.OnPropertyChanged(() => this.UseBuyOrder);
                    this.OnPropertyChanged(() => this.UseSellListing);
                    this.CalculateSalvageThreshold();
                }
            }
        }

        /// <summary>
        /// /// True if the Sell Listing price should be used for ectoplasm prices
        /// </summary>
        public bool UseSellListing
        {
            get { return this.userData.EctoplasmThresholdUsesSellListing; }
            set
            {
                if (this.userData.EctoplasmThresholdUsesSellListing != value)
                {
                    this.userData.EctoplasmThresholdUsesSellListing = value;

                    this.OnPropertyChanged(() => this.UseBuyOrder);
                    this.OnPropertyChanged(() => this.UseSellListing);
                    this.CalculateSalvageThreshold();
                }
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public EctoSalvageHelperViewModel(CommerceUserData userData)
        {
            this.EctoplasmBuyOrder = new Price();
            this.EctoplasmSellListing = new Price();
            this.Tax = new Price();
            this.SalvageThreshold = new Price();
            this.userData = userData;

            this.EctoplasmBuyOrder.PropertyChanged += (o, e) => this.CalculateSalvageThreshold();
            this.EctoplasmSellListing.PropertyChanged += (o, e) => this.CalculateSalvageThreshold();
        }

        private void CalculateSalvageThreshold()
        {
            if (this.userData.EctoplasmThresholdUsesSellListing)
            {
                this.Tax.Value = this.EctoplasmSellListing.Value * 0.85;
                this.SalvageThreshold.Value = (this.EctoplasmSellListing.Value * 0.85 * EctoplasmChanceCoeff - SalvageCost) / 0.85;
            }
            else
            {
                this.Tax.Value = this.EctoplasmBuyOrder.Value * 0.85;
                this.SalvageThreshold.Value = (this.EctoplasmBuyOrder.Value * 0.85 * EctoplasmChanceCoeff - SalvageCost) / 0.85;
            }
        }
    }
}
