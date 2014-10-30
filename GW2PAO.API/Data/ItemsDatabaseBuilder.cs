using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GW2DotNET;
using GW2DotNET.V2.Items;
using GW2PAO.API.Constants;
using GW2PAO.API.Data.Enums;
using Newtonsoft.Json;
using NLog;

namespace GW2PAO.API.Data
{
    public class ItemsDatabaseBuilder
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The service responsible for providing item information data
        /// </summary>
        private ItemService itemService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ItemsDatabaseBuilder()
        {
            this.itemService = (ItemService)ServiceFactory.Default().GetItemService();
        }

        /// <summary>
        /// Loads the item names database from file
        /// </summary>
        /// <returns></returns>
        public IDictionary<int, ItemDBEntry> LoadFromFile(CultureInfo culture)
        {
            var lang = culture.TwoLetterISOLanguageName;

            var supported = new[] { "en", "es", "fr", "de" };
            if (!supported.Contains(lang))
                lang = "en"; // Default to english if not supported

            var filename = this.GetFilePath(lang);

            var db = File.ReadAllText(filename);
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
        public int RebuildItemDatabase(CultureInfo culture, Action incrementProgressAction, Action rebuildCompleteAction, CancellationToken cancelToken)
        {
            this.itemService.Culture = culture;
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
                File.WriteAllText(this.GetFilePath(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName), dbString);

                rebuildCompleteAction.Invoke();
            }, cancelToken);

            return totalRequests;
        }

        /// <summary>
        /// Retrieves the full path of the stored names file using the given culture
        /// </summary>
        public string GetFilePath(string twoLetterIsoLangId)
        {
            string filename = Paths.LocalizationFolder + "ItemDatabase";

            if (twoLetterIsoLangId != "en")
                filename += "." + twoLetterIsoLangId;

            filename += ".json";

            return filename;
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
