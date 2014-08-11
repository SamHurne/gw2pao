using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.PresentationCore;
using NLog;

namespace GW2PAO.ViewModels.TradingPost
{
    public class TPCalculatorViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Backing-field for the Quantity property
        /// </summary>
        private int quantity;

        /// <summary>
        /// User-entered Buy-Price
        /// </summary>
        public Price BuyPrice { get; private set; }

        /// <summary>
        /// User-entered Sell-Price
        /// </summary>
        public Price SellPrice { get; private set; }

        /// <summary>
        /// Total quantity to calculate for
        /// </summary>
        public int Quantity
        {
            get { return this.quantity; }
            set
            {
                if (value > 0)
                {
                    if (SetField(ref this.quantity, value))
                    {
                        // Recalculate when this changes
                        this.CalculateAll();
                    }
                }
            }
        }

        /// <summary>
        /// Total calculated revenue
        /// </summary>
        public Price Revenue { get; private set; }

        /// <summary>
        /// Total calculated cost
        /// </summary>
        public Price Cost { get; private set; }

        /// <summary>
        /// Total listing fee
        /// </summary>
        public Price ListingFee { get; private set; }

        /// <summary>
        /// Total sale fee
        /// </summary>
        public Price SaleFee { get; private set; }

        /// <summary>
        /// Total profit
        /// </summary>
        public Price Profit { get; private set; }

        /// <summary>
        /// Command to reset all values
        /// </summary>
        public DelegateCommand ResetCommand { get { return new DelegateCommand(this.ResetValues); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TPCalculatorViewModel()
        {
            this.BuyPrice = new Price();
            this.SellPrice = new Price();
            this.Revenue = new Price();
            this.Cost = new Price();
            this.ListingFee = new Price();
            this.SaleFee = new Price();
            this.Profit = new Price();

            this.BuyPrice.PropertyChanged += (o, e) => this.CalculateAll();
            this.SellPrice.PropertyChanged += (o, e) => this.CalculateAll();

            this.Quantity = 1; // minimum of 1
        }

        /// <summary>
        /// Resets all input values to 0
        /// </summary>
        private void ResetValues()
        {
            this.BuyPrice.Value = 0.0;
            this.SellPrice.Value = 0.0;
            this.Quantity = 1;
        }

        /// <summary>
        /// Recalculates all output values
        /// </summary>
        private void CalculateAll()
        {
            logger.Debug("Calculating all values");
            this.CalculateRevenue();
            this.CalculateCost();
            this.CalculateListingFee();
            this.CalculateSaleFee();
            this.CalculateProfit();
        }

        /// <summary>
        /// Calculates revenue
        /// </summary>
        private void CalculateRevenue()
        {
            this.Revenue.Value = this.SellPrice.Value * this.Quantity;
            logger.Debug("Revenue: {0}", this.Revenue.Value);
        }

        /// <summary>
        /// Calculates cost
        /// </summary>
        private void CalculateCost()
        {
            this.Cost.Value = this.BuyPrice.Value * this.Quantity;
            logger.Debug("Cost: {0}", this.Cost.Value);
        }

        /// <summary>
        /// Calculates the listing fee (5% of revenue)
        /// </summary>
        private void CalculateListingFee()
        {
            this.ListingFee.Value = this.Revenue.Value * 0.05;
            logger.Debug("ListingFee: {0}", this.ListingFee.Value);
        }

        /// <summary>
        /// Calculates sale fee (10% of revenue)
        /// </summary>
        private void CalculateSaleFee()
        {
            this.SaleFee.Value = this.Revenue.Value * 0.10;
            logger.Debug("SaleFee: {0}", this.SaleFee.Value);
        }

        /// <summary>
        /// Calculates profit (revenue - cost - fees)
        /// </summary>
        private void CalculateProfit()
        {
            this.Profit.Value = this.Revenue.Value - (this.ListingFee.Value + this.SaleFee.Value) - this.Cost.Value;
            logger.Debug("Profit: {0}", this.Profit.Value);
        }
    }

    /// <summary>
    /// Helper class containing the gold, silver, and copper components of a price in GW2
    /// </summary>
    public class Price : NotifyPropertyChangedBase
    {
        private int gold;
        private int silver;
        private int copper;

        /// <summary>
        /// Gold-component for the price
        /// </summary>
        public int Gold
        {
            get { return this.gold; }
            set
            {
                SetField(ref this.gold, value);
            }
        }

        /// <summary>
        /// Silver-component for the price
        /// </summary>
        public int Silver
        {
            get
            {
                if (this.silver >= 0)
                    return this.silver;
                else
                    return this.silver * -1; // Never display silver as negative
            }
            set
            {
                if (value < 100)
                    SetField(ref this.silver, value);
                else
                    SetField(ref this.silver, 99);
            }
        }

        /// <summary>
        /// Copper-component for the price
        /// </summary>
        public int Copper
        {
            get
            {
                if (this.copper >= 0)
                    return this.copper;
                else
                    return this.copper * -1; // Never display silver as negative
            }
            set
            {
                if (value < 100)
                    SetField(ref this.copper, value);
                else
                    SetField(ref this.copper, 99);
            }
        }

        /// <summary>
        /// The total value, base-silver
        /// </summary>
        public double Value
        {
            get
            {
                return (this.gold * 100.0) + this.silver + (this.copper / 100.0);
            }
            set
            {
                this.Gold = (int)(value / 100.0);
                this.Silver = (int)(value - (this.Gold * 100.0));
                this.Copper = (int)((value - ((int)value)) * 100.0);
            }
        }
    }
}
