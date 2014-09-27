using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2DotNET;
using GW2DotNET.Entities.Items;
using GW2DotNET.V2.Commerce;
using GW2PAO.API.Data;
using GW2PAO.API.Services.Interfaces;
using Newtonsoft.Json;

namespace GW2PAO.API.Services
{
    public class CommerceService : ICommerceService
    {
        private const string NAMES_DATABASE_FILENAME = "ItemNames.json";
        private ServiceManager serviceManager;
        private PriceService priceService;

        /// <summary>
        /// Cache of item names loaded from the ItemNames.json file
        /// </summary>
        private Dictionary<int, string> itemNames;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CommerceService()
        {
            ServiceFactory sf = ServiceFactory.Default();
            this.priceService = (PriceService)sf.GetPriceService();
            this.serviceManager = new ServiceManager();

            // Load the names database
            var db = File.ReadAllText(NAMES_DATABASE_FILENAME);
            itemNames = JsonConvert.DeserializeObject<Dictionary<int, string>>(db); // Consider just keeping this in memory
        }

        /// <summary>
        /// Performs a rebuild of the names item database
        /// For now, this makes use of the http://api.gw2tp.com/1/bulk/items-names.json
        /// request so that we don't have to spend 10+ minutes building up our item database
        /// </summary>
        public void BuildItemDatabase()
        {
            Dictionary<int, GW2DotNET.Entities.Items.Item> itemsDb = new Dictionary<int, GW2DotNET.Entities.Items.Item>();
            var items = this.serviceManager.GetItems();
            foreach (var item in items)
            { 
                var itemDetails = this.serviceManager.GetItemDetails(item);
                itemsDb.Add(item, itemDetails);
            };

            // Save collection to disk, probably as JSON to save disk space
            var db = JsonConvert.SerializeObject(itemsDb, Formatting.None);

            //File.WriteAllText(DATABASE_FILENAME, db);
        }

        /// <summary>
        /// Returns true if the given item exists, else false
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        /// <returns>true if the given item exists, else false</returns>
        public bool DoesItemExist(string itemName)
        {
            return itemNames.Values.Any(name => name == itemName);
        }

        /// <summary>
        /// Returns the item ID of the item with the given name
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        /// <returns>item ID of the item with the given name, or -1 if not found</returns>
        public int GetItemID(string itemName)
        {
            var item = itemNames.FirstOrDefault(i => i.Value == itemName);

            if (item.Key != 0 || item.Value != null)
            {
                return item.Key;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns the item information for the item with the given name
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        /// <returns>Item object containing all item information, or null if the itemName is invalid</returns>
        public GW2PAO.API.Data.Item GetItem(string itemName)
        {
            GW2PAO.API.Data.Item item = null;

            var itemId = itemNames.FirstOrDefault(i => i.Value == itemName);

            if (itemId.Key != 0 || itemId.Value != null)
            {
                var itemDetails = this.serviceManager.GetItemDetails(itemId.Key);

                // TODO: More details, such as item Icon URI, description, etc
                item = new Data.Item(itemId.Key, itemDetails.Name);
                item.Prices = this.GetItemPrices(item.ID);
            }

            return item;
        }

        /// <summary>
        /// Returns the ItemPrices of the item with the given item ID
        /// </summary>
        /// <param name="itemId">the item to retrieve the prices for</param>
        /// <returns>The prices for the item</returns>
        public ItemPrices GetItemPrices(int itemId)
        {
            var prices = priceService.Find(itemId);

            return new ItemPrices()
            {
                HighestBuyOrder = prices.BuyOffers.UnitPrice,
                BuyOrderQuantity = prices.BuyOffers.Quantity,
                LowestSellListing = prices.SellOffers.UnitPrice,
                SellOrderQuantity = prices.SellOffers.Quantity
            };
        }

        /// <summary>
        /// Returns the ItemPrices of the items with the given item IDs
        /// </summary>
        /// <param name="itemIds">The items to search for</param>
        public IDictionary<int, ItemPrices> GetItemPrices(ICollection<int> itemIds)
        {
            var listings = priceService.FindAll(itemIds);

            Dictionary<int, ItemPrices> prices = new Dictionary<int, ItemPrices>();

            foreach (var listing in listings)
            {
                var listingPrices = new ItemPrices()
                {
                    HighestBuyOrder = listing.Value.BuyOffers.UnitPrice,
                    BuyOrderQuantity = listing.Value.BuyOffers.Quantity,
                    LowestSellListing = listing.Value.SellOffers.UnitPrice,
                    SellOrderQuantity = listing.Value.SellOffers.Quantity
                };
                prices.Add(listing.Key, listingPrices);
            }

            return prices;
        }
    }
}
