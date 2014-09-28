using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;

namespace GW2PAO.API.Services.Interfaces
{
    public interface ICommerceService
    {
        /// <summary>
        /// Cache of item names loaded from the ItemNames.json file
        /// </summary>
        IDictionary<int, string> ItemNames { get; }

        /// <summary>
        /// Helper object for building/rebuilding the names database
        /// </summary>
        ItemNamesDatabaseBuilder NamesDatabaseBuilder { get;}

        /// <summary>
        /// Forces a re-load of the item names database
        /// </summary>
        void ReloadNames();

        /// <summary>
        /// Returns true if the given item exists, else false
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        /// <returns>true if the given item exists, else false</returns>
        bool DoesItemExist(string itemName);

        /// <summary>
        /// Returns the item ID of the item with the given name
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        /// <returns>item ID of the item with the given name, or -1 if not found</returns>
        int GetItemID(string itemName);

        /// <summary>
        /// Returns the item information for the item with the given names
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        /// <returns>Item object containing all item information, or null if the itemName is invalid</returns>
        GW2PAO.API.Data.Item GetItem(string itemName);

        /// <summary>
        /// Returns the item information for the items with the given ID
        /// </summary>
        /// <param name="itemID">ID of the item</param>
        /// <returns>Item object containing all item information, or null if the itemName is invalid</returns>
        GW2PAO.API.Data.Item GetItem(int itemID);

        /// <summary>
        /// Returns the item information for the items with the given Names
        /// </summary>
        /// <param name="itemNames">Names of the items to retrieve</param>
        /// <returns>Collection of Item objects containing all item information</returns>
        IDictionary<string, Item> GetItems(ICollection<string> itemNames);

        /// <summary>
        /// Returns the item information for the items with the given IDs
        /// </summary>
        /// <param name="itemIDs">IDs of the items to retrieve</param>
        /// <returns>Dictionary of Item objects containing all item information</returns>
        IDictionary<int, Item> GetItems(ICollection<int> itemIDs);

        /// <summary>
        /// Returns the ItemPrices of the item with the given item ID
        /// </summary>
        /// <param name="itemId">the item to retrieve the prices for</param>
        /// <returns>The prices for the item</returns>
        ItemPrices GetItemPrices(int itemId);

        /// <summary>
        /// Returns the ItemPrices of the items with the given item IDs
        /// </summary>
        /// <param name="itemIds">The items to search for</param>
        IDictionary<int, ItemPrices> GetItemPrices(ICollection<int> itemIds);
    }
}
