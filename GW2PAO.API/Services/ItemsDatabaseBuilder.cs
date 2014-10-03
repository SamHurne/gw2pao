using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GW2DotNET;
using GW2DotNET.V2.Items;
using GW2PAO.API.Data.Enums;
using Newtonsoft.Json;
using NLog;

namespace GW2PAO.API.Services
{
    public class ItemsDatabaseBuilder
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Filename for the names database file
        /// </summary>
        public const string NAMES_DATABASE_FILENAME = "ItemDatabase.json";

        private ItemService itemService;

        public ItemsDatabaseBuilder()
        {
            this.itemService = (ItemService)ServiceFactory.Default().GetItemService();
        }

        /// <summary>
        /// Loads the item names database from file
        /// </summary>
        /// <returns></returns>
        public IDictionary<int, ItemDBEntry> LoadFromFile()
        {
            // Load the names database
            var db = File.ReadAllText(NAMES_DATABASE_FILENAME);
            Dictionary<int, ItemDBEntry> itemDb = JsonConvert.DeserializeObject<Dictionary<int, ItemDBEntry>>(db);
            return itemDb;
        }

        /// <summary>
        /// Performs a rebuild of the names item database
        /// This includes saving to disk
        /// This is mostly asynchronous, as it returns the total amount of names it will be pulling
        /// from the API, for progress-bar purposes.
        /// </summary>
        /// <param name="incrementProgressAction">Action to perform when the progress continues</param>
        /// <param name="cancelToken">Cancellation token in case the user wishes to cancel the task</param>
        /// <returns>Returns the total amount of requests that will be performed</returns>
        public int RebuildItemDatabase(Action incrementProgressAction, Action rebuildCompleteAction, CancellationToken cancelToken)
        {
            var itemIds = this.itemService.Discover();

            // We'll split this up into multiple requests
            int requestSize = 200; // maybe tweak this
            int totalRequests = (itemIds.Count / requestSize) + 1;

            Task.Factory.StartNew(() =>
            {
                Dictionary<int, ItemDBEntry> itemsDb = new Dictionary<int, ItemDBEntry>();

                for (int i = 0; i < totalRequests; i++)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var items = this.itemService.GetPage(i, requestSize);
                    foreach (var item in items)
                    {
                        var entry = new ItemDBEntry(item.ItemId, item.Name, (ItemRarity)item.Rarity, item.Level);
                        itemsDb.Add(item.ItemId, entry);
                    }
                    incrementProgressAction.Invoke();
                }

                var dbString = JsonConvert.SerializeObject(itemsDb);
                File.WriteAllText(NAMES_DATABASE_FILENAME, dbString);

                rebuildCompleteAction.Invoke();
            }, cancelToken);

            return totalRequests;
        }
    }

    public class ItemDBEntry
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public ItemRarity Rarity { get; set; }
        public int Level { get; set; }

        public ItemDBEntry(int id, string name, ItemRarity rarity, int level)
        {
            this.ID = id;
            this.Name = name;
            this.Rarity = rarity;
            this.Level = level;
        }
    }
}
