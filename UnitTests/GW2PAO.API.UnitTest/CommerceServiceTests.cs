using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GW2PAO.API.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GW2PAO.API.Data;
using GW2PAO.API.Constants;

namespace GW2PAO.API.UnitTest
{
    [TestClass]
    public class CommerceServiceTests
    {
        private const string VALID_ITEM_NAME = "Copper Ore";
        private const int VALID_ITEM_NAME_ID = 19697;

        private const string INVALID_ITEM_NAME = "Item_Name_That_Will_Never_Exist";

        [TestMethod]
        public void CommerceService_Constructor_Basic()
        {
            CommerceService cs = new CommerceService();

            Assert.IsNotNull(cs.ItemsDatabaseBuilder);
            Assert.IsNotNull(cs.ItemsDB);
        }

        [TestMethod]
        public void CommerceService_Constructor_NoDatabaseFile()
        {
            string renamedFilename = "renamedDB.json";
            File.Move(Paths.LocalizationFolder + "\\ItemDatabase.json", renamedFilename);

            try
            {
                CommerceService cs = new CommerceService();

                Assert.IsNotNull(cs.ItemsDatabaseBuilder);
                Assert.IsNotNull(cs.ItemsDB);
            }
            finally
            {
                File.Move(renamedFilename, Paths.LocalizationFolder + "\\ItemDatabase.json");
            }
        }

        [TestMethod]
        public void CommerceService_Constructor_InvalidDatabaseFile()
        {
            string renamedFilename = "renamedDB.json";
            File.Move(Paths.LocalizationFolder + "\\ItemDatabase.json", renamedFilename);
            File.WriteAllText(Paths.LocalizationFolder + "\\ItemDatabase.json", "invalid file");

            try
            {
                CommerceService cs = new CommerceService();

                Assert.IsNotNull(cs.ItemsDatabaseBuilder);
                Assert.IsNotNull(cs.ItemsDB);
            }
            finally
            {
                File.Delete(Paths.LocalizationFolder + "\\ItemDatabase.json");
                File.Move(renamedFilename, Paths.LocalizationFolder + "\\ItemDatabase.json");
            }
        }

        [TestMethod]
        public void CommerceService_ReloadDatabase_Basic()
        {
            CommerceService cs = new CommerceService();
            cs.ItemsDB.Clear();
            cs.ReloadDatabase();
            Assert.IsNotNull(cs.ItemsDB);
            Assert.IsTrue(cs.ItemsDB.Count > 0);
        }

        [TestMethod]
        public void CommerceService_ReloadDatabase_PerfCheck()
        {
            // For this test, we'll time how long it takes to load
            CommerceService cs = new CommerceService();
            cs.ItemsDB.Clear();

            var sw = new Stopwatch();
            sw.Start();
            cs.ReloadDatabase();
            sw.Stop();
            Console.WriteLine("To load {0} item names, it took {1}ms", cs.ItemsDB.Count, sw.ElapsedMilliseconds);
        }

        [TestMethod]
        public void CommerceService_ReloadDatabase_NoDatabaseFile()
        {
            CommerceService cs = new CommerceService();
            cs.ItemsDB.Clear();

            string renamedFilename = "renamedDB.json";
            File.Move(Paths.LocalizationFolder + "\\ItemDatabase.json", renamedFilename);

            try
            {
                cs.ReloadDatabase();
                Assert.IsNotNull(cs.ItemsDB);
            }
            finally
            {
                File.Move(renamedFilename, Paths.LocalizationFolder + "\\ItemDatabase.json");
            }
        }

        [TestMethod]
        public void CommerceService_ReloadNames_InvalidDatabaseFile()
        {
            CommerceService cs = new CommerceService();
            cs.ItemsDB.Clear();

            string renamedFilename = "renamedDB.json";
            File.Move(Paths.LocalizationFolder + "\\ItemDatabase.json", renamedFilename);
            File.WriteAllText(Paths.LocalizationFolder + "\\ItemDatabase.json", "invalid file");

            try
            {
                cs.ReloadDatabase();
                Assert.IsNotNull(cs.ItemsDB);
            }
            finally
            {
                File.Delete(Paths.LocalizationFolder + "\\ItemDatabase.json");
                File.Move(renamedFilename, Paths.LocalizationFolder + "\\ItemDatabase.json");
            }
        }

        [TestMethod]
        public void CommerceService_DoesItemExist_True()
        {
            CommerceService cs = new CommerceService();

            var sw = new Stopwatch();
            sw.Start();
            Assert.IsTrue(cs.DoesItemExist(VALID_ITEM_NAME, Data.Enums.ItemRarity.Basic, 0));
            sw.Stop();
            Console.WriteLine("To search for a valid name, it took {0}ms", sw.ElapsedMilliseconds);
        }

        [TestMethod]
        public void CommerceService_DoesItemExist_False()
        {
            // For this test, we'll also time how long it takes to check
            CommerceService cs = new CommerceService();

            var sw = new Stopwatch();
            sw.Start();
            Assert.IsFalse(cs.DoesItemExist(INVALID_ITEM_NAME, Data.Enums.ItemRarity.Basic, 0));
            sw.Stop();
            Console.WriteLine("To search for an invalid name, it took {0}ms", sw.ElapsedMilliseconds);
        }

        [TestMethod]
        public void CommerceService_GetItemID_Valid()
        {
            CommerceService cs = new CommerceService();

            var sw = new Stopwatch();
            sw.Start();
            var id = cs.GetItemID(VALID_ITEM_NAME, Data.Enums.ItemRarity.Basic, 0);
            sw.Stop();
            Console.WriteLine("To search for an ID using a valid name, it took {0}ms", sw.ElapsedMilliseconds);

            Assert.AreEqual(VALID_ITEM_NAME_ID, id);
        }

        [TestMethod]
        public void CommerceService_GetItemID_Invalid()
        {
            CommerceService cs = new CommerceService();

            var sw = new Stopwatch();
            sw.Start();
            var id = cs.GetItemID(INVALID_ITEM_NAME, Data.Enums.ItemRarity.Basic, 0);
            sw.Stop();
            Console.WriteLine("To search for an ID using an invalid name, it took {0}ms", sw.ElapsedMilliseconds);

            Assert.AreEqual(-1, id);
        }

        [TestMethod]
        public void CommerceService_GetItem_ByNameRarityLevel_Valid()
        {
            CommerceService cs = new CommerceService();

            var sw = new Stopwatch();
            sw.Start();
            var item = cs.GetItem(VALID_ITEM_NAME, Data.Enums.ItemRarity.Basic, 0);
            sw.Stop();
            Console.WriteLine("To retrieve details of an item using a valid name, it took {0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(item);
            Assert.AreEqual(VALID_ITEM_NAME_ID, item.ID);
            Assert.AreEqual(VALID_ITEM_NAME, item.Name);
            // TODO: check more
        }

        [TestMethod]
        public void CommerceService_GetItem_ByNameRarityLevel_Invalid()
        {
            CommerceService cs = new CommerceService();

            var sw = new Stopwatch();
            sw.Start();
            var item = cs.GetItem(INVALID_ITEM_NAME, Data.Enums.ItemRarity.Basic, 0);
            sw.Stop();
            Console.WriteLine("To retrieve details of an item using a invalid name, it took {0}ms", sw.ElapsedMilliseconds);

            Assert.IsNull(item);
        }

        [TestMethod]
        public void CommerceService_GetItem_ByID_Valid()
        {
            CommerceService cs = new CommerceService();

            var sw = new Stopwatch();
            sw.Start();
            var item = cs.GetItem(VALID_ITEM_NAME_ID);
            sw.Stop();
            Console.WriteLine("To retrieve details of an item using a valid ID, it took {0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(item);
            Assert.AreEqual(VALID_ITEM_NAME_ID, item.ID);
            Assert.AreEqual(VALID_ITEM_NAME, item.Name);
            // TODO: check more
        }

        [TestMethod]
        public void CommerceService_GetItem_ByID_Invalid()
        {
            CommerceService cs = new CommerceService();

            var sw = new Stopwatch();
            sw.Start();
            var item = cs.GetItem(-1);
            sw.Stop();
            Console.WriteLine("To retrieve details of an item using a invalid ID, it took {0}ms", sw.ElapsedMilliseconds);

            Assert.IsNull(item);
        }

        [TestMethod]
        public void CommerceService_GetItems_ByID_AllValid()
        {
            CommerceService cs = new CommerceService();

            // Create the collection of items
            List<int> ids = new List<int>(cs.ItemsDB.Keys.Take(200));

            var sw = new Stopwatch();
            sw.Start();
            var items = cs.GetItems(ids);
            sw.Stop();
            Console.WriteLine("To retrieve details {0} items using a valid IDs, it took {1}ms", ids.Count, sw.ElapsedMilliseconds);

            Assert.IsNotNull(items);
            foreach (var item in items)
            {
                var itemFromDb = cs.ItemsDB.FirstOrDefault(i => i.Key == item.Key);
                Assert.IsNotNull(itemFromDb);
                Assert.AreEqual(itemFromDb.Key, item.Value.ID);
                Assert.AreEqual(itemFromDb.Value.ID, item.Value.ID);
                Assert.AreEqual(itemFromDb.Value.Name, item.Value.Name);
                Assert.AreEqual(itemFromDb.Value.Rarity, item.Value.Rarity);
                Assert.AreEqual(itemFromDb.Value.Level, item.Value.LevelRequirement);
            }
        }

        [TestMethod]
        public void CommerceService_GetItems_ByID_AllInvalid()
        {
            CommerceService cs = new CommerceService();

            // Create the collection of items
            List<int> ids = new List<int>();
            for (int i = 0; i > -200; i--)
                ids.Add(i);

            var sw = new Stopwatch();
            sw.Start();
            var items = cs.GetItems(ids);
            sw.Stop();
            Console.WriteLine("To retrieve details {0} items using invalid IDs, it took {1}ms", ids.Count, sw.ElapsedMilliseconds);

            Assert.IsNotNull(items);
            Assert.AreEqual(0, items.Count);
        }

        [TestMethod]
        public void CommerceService_GetItems_ByID_SomeInvalid()
        {
            CommerceService cs = new CommerceService();

            // Create the collection of items
            List<int> ids = new List<int>();
            for (int i = 0; i > -50; i--)
                ids.Add(i);
            ids.AddRange(cs.ItemsDB.Keys.Take(100));
            for (int i = -150; i > -200; i--)
                ids.Add(i);

            var sw = new Stopwatch();
            sw.Start();
            var items = cs.GetItems(ids);
            sw.Stop();
            Console.WriteLine("To retrieve details {0} items using a valid and invalid IDs, it took {1}ms", ids.Count, sw.ElapsedMilliseconds);

            Assert.IsNotNull(items);
            Assert.AreEqual(100, items.Count);
            foreach (var item in items)
            {
                var itemFromDb = cs.ItemsDB.FirstOrDefault(i => i.Key == item.Key);
                Assert.IsNotNull(itemFromDb);
                Assert.AreEqual(itemFromDb.Key, item.Value.ID);
                Assert.AreEqual(itemFromDb.Value.ID, item.Value.ID);
                Assert.AreEqual(itemFromDb.Value.Name, item.Value.Name);
                Assert.AreEqual(itemFromDb.Value.Rarity, item.Value.Rarity);
                Assert.AreEqual(itemFromDb.Value.Level, item.Value.LevelRequirement);
            }
        }

        [TestMethod]
        public void CommerceService_GetItemPrices_Single_Valid()
        {
            CommerceService cs = new CommerceService();

            var sw = new Stopwatch();
            sw.Start();
            var prices = cs.GetItemPrices(VALID_ITEM_NAME_ID);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(prices);
            Assert.IsTrue(prices.BuyOrderQuantity > 0);
            Assert.IsTrue(prices.HighestBuyOrder > 0);
            Assert.IsTrue(prices.LowestSellListing > 0);
            Assert.IsTrue(prices.SellOrderQuantity > 0);
        }

        [TestMethod]
        public void CommerceService_GetItemPrices_Single_Invalid()
        {
            CommerceService cs = new CommerceService();

            var sw = new Stopwatch();
            sw.Start();
            var prices = cs.GetItemPrices(-1);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNull(prices);
        }

        [TestMethod]
        public void CommerceService_GetItemPrices_Multiple_Valid()
        {
            CommerceService cs = new CommerceService();

            // Create the collection of items
            List<int> ids = new List<int>(cs.ItemsDB.Keys.Take(100));

            var sw = new Stopwatch();
            sw.Start();
            var prices = cs.GetItemPrices(ids);
            sw.Stop();
            Console.WriteLine("To retrieve prices of {0} items using valid IDs, it took {1}ms", ids.Count, sw.ElapsedMilliseconds);

            Assert.IsNotNull(prices);
            foreach (var itemPrices in prices)
            {
                Assert.IsTrue(ids.Contains(itemPrices.Key));
            }
        }

        [TestMethod]
        public void CommerceService_GetItemPrices_Multiple_Invalid()
        {
            CommerceService cs = new CommerceService();

            // Create the collection of items
            List<int> ids = new List<int>();
            for (int i = 0; i > -200; i--)
                ids.Add(i);

            var sw = new Stopwatch();
            sw.Start();
            var prices = cs.GetItemPrices(ids);
            sw.Stop();
            Console.WriteLine("To retrieve prices of {0} items using invalid IDs, it took {1}ms", ids.Count, sw.ElapsedMilliseconds);

            Assert.IsNotNull(prices);
            Assert.AreEqual(0, prices.Count);
        }
    }
}
