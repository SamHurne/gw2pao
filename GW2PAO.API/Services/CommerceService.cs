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
        /// Cache of item names loaded fromthe ItemNames.json file
        /// </summary>
        public ConcurrentDictionary<int, ItemDBEntry> ItemsDB { get; private set; }

        /// <summary>
        /// Helper object for building/rebuilding the items database
        /// </summary>
        public ItemsDatabaseBuilder ItemsDatabaseBuilder { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CommerceService()
        {
            ServiceFactory sf = ServiceFactory.Default();
            this.priceService = (PriceService)sf.GetPriceService();
            this.itemService = (ItemService)sf.GetItemService();
            this.serviceManager = new ServiceManager();
            this.ItemsDatabaseBuilder = new ItemsDatabaseBuilder();
            this.ItemsDB = new ConcurrentDictionary<int, ItemDBEntry>();
            this.ReloadDatabase();
        }

        /// <summary>
        /// Forces a re-load of the items database
        /// </summary>
        public void ReloadDatabase()
        {
            try
            {
                var newItemsDb = this.ItemsDatabaseBuilder.LoadFromFile();
                this.ItemsDB.Clear();
                foreach (var item in newItemsDb)
                    this.ItemsDB.TryAdd(item.Key, item.Value);
            }
            catch (FileNotFoundException ex)
            {
                // Log a meaningful warning and continue
                logger.Warn("Unable to load the items database - file not found: {0}", ex);
            }
            catch (Newtonsoft.Json.JsonReaderException ex)
            {
                // Log a meaningful warning and continue
                logger.Warn("Error loading the items database: {0}", ex);
            }
        }

        /// <summary>
        /// Returns true if the given item exists, else false
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        /// <param name="rarity">Rarity of the item</param>
        /// <param name="level">Level of the item</param>
        /// <returns>true if the given item exists, else false</returns>
        public bool DoesItemExist(string itemName, Data.Enums.ItemRarity rarity, int level)
        {
            return this.ItemsDB.Values.Any(item => item.Name == itemName
                                                && item.Rarity == rarity
                                                && item.Level == level);
        }

        /// <summary>
        /// Returns the item ID of the item with the given name
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        /// <param name="rarity">Rarity of the item</param>
        /// <param name="level">Level of the item</param>
        /// <returns>item ID of the item with the given name, or -1 if not found</returns>
        public int GetItemID(string itemName, Data.Enums.ItemRarity rarity, int level)
        {
            var item = this.ItemsDB.FirstOrDefault(itm => itm.Value.Name == itemName
                                                        && itm.Value.Rarity == rarity
                                                        && itm.Value.Level == level);

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
        /// Returns the item information for the item with the given names
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        /// <param name="rarity">Rarity of the item</param>
        /// <param name="level">Level of the item</param>
        /// <returns>Item object containing all item information, or null if the itemName is invalid</returns>
        public GW2PAO.API.Data.Item GetItem(string itemName, Data.Enums.ItemRarity rarity, int level)
        {
            GW2PAO.API.Data.Item item = null;

            KeyValuePair<int, ItemDBEntry> itemEntry;
            itemEntry = this.ItemsDB.FirstOrDefault(itm => itm.Value.Name == itemName
                                                        && itm.Value.Rarity == rarity
                                                        && itm.Value.Level == level);

            if (itemEntry.Key != 0 || itemEntry.Value != null)
            {
                item = this.GetItem(itemEntry.Key);
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

            try
            {
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
            }
            catch (GW2DotNET.Common.ServiceException ex)
            {
                // Don't crash, just return null
                logger.Warn("Error finding item with id {0}: {1}", itemID, ex);
            }

            return item;
        }

        /// <summary>
        /// Returns the item information for the items with the given IDs
        /// </summary>
        /// <param name="itemIDs">IDs of the items to retrieve</param>
        /// <returns>Collection of Item objects containing all item information</returns>
        public IDictionary<int, GW2PAO.API.Data.Item> GetItems(ICollection<int> itemIDs)
        {
            Dictionary<int, GW2PAO.API.Data.Item> items = new Dictionary<int, GW2PAO.API.Data.Item>();

            try
            {
                // Remove all items with itemID of 0 or less
                var validIDs = itemIDs.Where(id => id > 0).ToList();

                var itemDetails = this.itemService.FindAll(validIDs);
                var prices = this.GetItemPrices(validIDs);

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
                    if (prices.ContainsKey(item.ID))
                        item.Prices = prices[item.ID];
                    else
                        item.Prices = new ItemPrices(); // empty, no prices found

                    // Since there is no need to use ALL details right now, we'll just get what we need...
                    // TODO: Finish this up, get all details, such as Type, SkinID

                    items.Add(item.ID, item);
                }
            }
            catch (GW2DotNET.Common.ServiceException ex)
            {
                // Don't crash, just return null
                logger.Warn("Error finding item: {0}", ex);
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
            ItemPrices itemPrices = null;
            try
            {
                var prices = priceService.Find(itemId);
                if (prices != null && prices.ItemId > 0)
                {
                    itemPrices = new ItemPrices()
                    {
                        HighestBuyOrder = prices.BuyOffers.UnitPrice,
                        BuyOrderQuantity = prices.BuyOffers.Quantity,
                        LowestSellListing = prices.SellOffers.UnitPrice,
                        SellOrderQuantity = prices.SellOffers.Quantity
                    };
                }
            }
            catch (GW2DotNET.Common.ServiceException ex)
            {
                // Don't crash, just return null
                logger.Warn("Error finding prices for itemId {0}: {1}", ex);
            }

            return itemPrices;
        }

        /// <summary>
        /// Returns the ItemPrices of the items with the given item IDs
        /// </summary>
        /// <param name="itemIds">The items to search for</param>
        /// <returns>Dictionary containing all item prices for the given items</returns>
        public IDictionary<int, ItemPrices> GetItemPrices(ICollection<int> itemIds)
        {
            Dictionary<int, ItemPrices> prices = new Dictionary<int, ItemPrices>();

            try
            {
                var listings = priceService.FindAll(itemIds);
                foreach (var listing in listings)
                {
                    if (listing.Value.ItemId > 0)
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
                }
            }
            catch (GW2DotNET.Common.ServiceException ex)
            {
                // Don't crash, just return null
                logger.Warn("Error finding prices: {0}", ex);
            }

            return prices;
        }
    }
}
