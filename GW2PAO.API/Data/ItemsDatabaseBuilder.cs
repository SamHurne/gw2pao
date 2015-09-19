using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GW2NET;
using GW2PAO.API.Constants;

using Newtonsoft.Json;
using NLog;

namespace GW2PAO.API.Data
{
    using System.Net;

    using GW2NET.Common;
    using GW2NET.Items;

    using ItemRarity = GW2PAO.API.Data.Enums.ItemRarity;

    public class ItemsDatabaseBuilder
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
            var itemService = GW2.V2.Items.ForCulture(culture);
            int requestSize = 200;
            IPageContext ctx = itemService.FindPage(0, requestSize);

            // Start up a task that will kick off the requests and wait for their completion
            Task.Factory.StartNew(
                () =>
                    {
                        ServicePointManager.DefaultConnectionLimit = ctx.PageCount;
                        var itemsDb = new Dictionary<int, ItemDBEntry>(capacity: ctx.TotalCount);
                        var tasks = itemService.FindAllPagesAsync(ctx.PageSize, ctx.PageCount, cancelToken);
                        var buckets = Interleaved(tasks);
                        for (int i = 0; i < buckets.Length; i++)
                        {
                            if (cancelToken.IsCancellationRequested)
                            {
                                return;
                            }

                            var bucket = buckets[i];
                            var task = bucket.Result;
                            if (task.IsCanceled)
                            {
                                logger.Info("Rebuilding the item names database was canceled.");
                            }
                            else if (task.IsFaulted)
                            {
                                logger.Warn("Failed to retrieve items page " + i + " of " + ctx.PageCount + ".");
                            }
                            else
                            {
                                foreach (var item in task.Result)
                                {
                                    itemsDb[item.ItemId] = new ItemDBEntry(
                                        item.ItemId,
                                        item.Name,
                                        (ItemRarity)item.Rarity,
                                        item.Level);
                                }

                                incrementProgressAction.Invoke();
                            }
                        }

                        var filePath = this.GetFilePath(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                        using (var streamWriter = new StreamWriter(filePath))
                        {
                            var serializer = new JsonSerializer();
                            serializer.Serialize(streamWriter, itemsDb);
                        }

                        rebuildCompleteAction.Invoke();
                    },
                cancelToken);

            return ctx.PageCount;
        }

        /// <summary>
        /// Retrieves the full path of the stored names file using the given culture
        /// </summary>
        public string GetFilePath(string twoLetterIsoLangId)
        {
            return string.Format("{0}\\{1}\\{2}", Paths.LocalizationFolder, twoLetterIsoLangId, "ItemDatabase.json");
        }

        // http://blogs.msdn.com/b/pfxteam/archive/2012/08/02/processing-tasks-as-they-complete.aspx
        private static Task<Task<T>>[] Interleaved<T>(IEnumerable<Task<T>> tasks)
        {
            var inputTasks = tasks.ToList();

            var buckets = new TaskCompletionSource<Task<T>>[inputTasks.Count];
            var results = new Task<Task<T>>[buckets.Length];
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = new TaskCompletionSource<Task<T>>();
                results[i] = buckets[i].Task;
            }

            int nextTaskIndex = -1;
            Action<Task<T>> continuation = completed =>
            {
                var bucket = buckets[Interlocked.Increment(ref nextTaskIndex)];
                bucket.TrySetResult(completed);
            };

            foreach (var inputTask in inputTasks)
                inputTask.ContinueWith(continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

            return results;
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

        public override string ToString()
        {
            return string.Format("{0} - \"{1}\"", this.ID, this.Name);
        }
    }
}
