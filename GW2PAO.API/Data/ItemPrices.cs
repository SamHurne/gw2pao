using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Data
{
    public class ItemPrices
    {
        /// <summary>
        /// The highest buy order, in copper
        /// </summary>
        public int HighestBuyOrder { get; set; }

        /// <summary>
        /// The lowest sale listing, in copper
        /// </summary>
        public int LowestSellListing { get; set; }

        /// <summary>
        /// Amount of buy orders
        /// </summary>
        public int BuyOrderQuantity { get; set; }

        /// <summary>
        /// Amount of sale listings
        /// </summary>
        public int SellOrderQuantity { get; set; }
    }
}
