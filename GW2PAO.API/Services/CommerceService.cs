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
using GW2DotNET.V2.Items;
using GW2PAO.API.Data;
using GW2PAO.API.Services.Interfaces;
using NLog;

namespace GW2PAO.API.Services
{
    public class CommerceService : ICommerceService
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private ServiceManager serviceManager;
        private PriceService priceService;
        private ItemService itemService;

        /// <summary>
        /// Locking object for the ItemNames collection
        /// </summary>
        private readonly object itemNamesLock = new object();

        /// <summary>
        /// Cache of item names loaded from the ItemNames.json file
        /// </summary>
        public IDictionary<int, string> ItemNames { get; private set; }

        /// <summary>
        /// Helper object for building/rebuilding the names database
        /// </summary>
        public ItemNamesDatabaseBuilder NamesDatabaseBuilder { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CommerceService()
        {
            ServiceFactory sf = ServiceFactory.Default();
            this.priceService = (PriceService)sf.GetPriceService();
            this.itemService = (ItemService)sf.GetItemService();
            this.serviceManager = new ServiceManager();
            this.NamesDatabaseBuilder = new ItemNamesDatabaseBuilder();

            lock (this.itemNamesLock)
            {
                this.ItemNames = this.NamesDatabaseBuilder.LoadFromFile();
            }
        }

        /// <summary>
        /// Forces a re-load of the item names database
        /// </summary>
        public void ReloadNames()
        {
            lock (this.itemNamesLock)
            {
                this.ItemNames = this.NamesDatabaseBuilder.LoadFromFile();
            }
        }

        /// <summary>
        /// Returns true if the given item exists, else false
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        /// <returns>true if the given item exists, else false</returns>
        public bool DoesItemExist(string itemName)
        {
            lock (this.itemNamesLock)
            {
                return this.ItemNames.Values.Any(name => name == itemName);
            }
        }

        /// <summary>
        /// Returns the item ID of the item with the given name
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        /// <returns>item ID of the item with the given name, or -1 if not found</returns>
        public int GetItemID(string itemName)
        {
            lock (this.itemNamesLock)
            {
                var item = ItemNames.FirstOrDefault(i => i.Value == itemName);

                if (item.Key != 0 || item.Value != null)
                {
                    return item.Key;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Returns the item information for the item with the given names
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        /// <returns>Item object containing all item information, or null if the itemName is invalid</returns>
        public GW2PAO.API.Data.Item GetItem(string itemName)
        {
            GW2PAO.API.Data.Item item = null;

            KeyValuePair<int, string> itemId;
            lock (this.itemNamesLock)
            {
                itemId = this.ItemNames.FirstOrDefault(i => i.Value == itemName);
            }

            if (itemId.Key != 0 || itemId.Value != null)
            {
                item = this.GetItem(itemId.Key);
            }

            return item;
        }

        /// <summary>
        /// Returns the item information for the items with the given ID
        /// </summary>
        /// <param name="itemID">ID of the item</param>
        /// <returns>Item object containing all item information, or null if the itemName is invalid</returns>
        public GW2PAO.API.Data.Item GetItem(int itemID)
        {
            GW2PAO.API.Data.Item item = null;

            var itemDetails = this.itemService.Find(itemID);
            if (itemDetails != null)
            {
                item = new Data.Item(itemID, itemDetails.Name);
                item.Icon = itemDetails.IconFileUrl;
                item.Description = itemDetails.Description;
                item.Rarity = (Data.Enums.ItemRarity)itemDetails.Rarity;
                item.Flags = (Data.Enums.ItemFlags)itemDetails.Flags;
                item.GameTypes = (Data.Enums.ItemGameTypes)itemDetails.GameTypes;
                item.LevelRequirement = itemDetails.Level;
                item.VenderValue = itemDetails.VendorValue;
                item.ChatCode = itemDetails.GetItemChatLink().ToString();
                item.Prices = this.GetItemPrices(itemID);

                // Since there is no need to use ALL details right now, we'll just get what we need...
                // TODO: Finish this up, get all details, such as Type, SkinID
            }

            return item;
        }

        /// <summary>
        /// Returns the item information for the items with the given Names
        /// </summary>
        /// <param name="itemNames">Names of the items to retrieve</param>
        /// <returns>Collection of Item objects containing all item information</returns>
        public IDictionary<string, GW2PAO.API.Data.Item> GetItems(ICollection<string> itemNames)
        {
            List<int> itemIDs = new List<int>();
            foreach (var name in itemNames)
            {
                var id = this.GetItemID(name);
                if (id > 0)
                    itemIDs.Add(id);
            }

            return this.GetItems(itemIDs).Values.ToDictionary(i => i.Name);
        }

        /// <summary>
        /// Returns the item information for the items with the given IDs
        /// </summary>
        /// <param name="itemIDs">IDs of the items to retrieve</param>
        /// <returns>Collection of Item objects containing all item information</returns>
        public IDictionary<int, GW2PAO.API.Data.Item> GetItems(ICollection<int> itemIDs)
        {
            Dictionary<int, GW2PAO.API.Data.Item> items = new Dictionary<int, GW2PAO.API.Data.Item>();

            var itemDetails = this.itemService.FindAll(itemIDs);
            var prices = this.GetItemPrices(itemIDs);

            foreach (var itemDetail in itemDetails)
            {
                GW2PAO.API.Data.Item item = new Data.Item(itemDetail.Key, itemDetail.Value.Name);
                item.Icon = itemDetail.Value.IconFileUrl;
                item.Description = itemDetail.Value.Description;
                item.Rarity = (Data.Enums.ItemRarity)itemDetail.Value.Rarity;
                item.Flags = (Data.Enums.ItemFlags)itemDetail.Value.Flags;
                item.GameTypes = (Data.Enums.ItemGameTypes)itemDetail.Value.GameTypes;
                item.LevelRequirement = itemDetail.Value.Level;
                item.VenderValue = itemDetail.Value.VendorValue;
                item.ChatCode = itemDetail.Value.GetItemChatLink().ToString();
                item.Prices = prices[item.ID];

                // Since there is no need to use ALL details right now, we'll just get what we need...
                // TODO: Finish this up, get all details, such as Type, SkinID

                items.Add(item.ID, item);
            }

            return items;
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
