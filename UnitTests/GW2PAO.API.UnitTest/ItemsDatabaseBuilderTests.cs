using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using GW2PAO.API.Data;
using GW2PAO.API.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GW2PAO.API.UnitTest
{
    [TestClass]
    public class ItemsDatabaseBuilderTests
    {
        [TestMethod]
        public void RebuildItemDatabase_LoadFromFile_Basic()
        {
            ItemsDatabaseBuilder dbBuilder = new ItemsDatabaseBuilder();

            var sw = new Stopwatch();
            sw.Start();
            var itemDb = dbBuilder.LoadFromFile(new CultureInfo("en"));
            sw.Stop();
            Console.WriteLine("{0}ms to load {1} entries", sw.ElapsedMilliseconds, itemDb.Count);

            Assert.IsNotNull(itemDb);
            Assert.IsTrue(itemDb.Count > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void RebuildItemDatabase_LoadFromFile_NoDatabaseFile()
        {
            ItemsDatabaseBuilder dbBuilder = new ItemsDatabaseBuilder();

            string renamedFilename = "renamedDB.json";
            File.Move(dbBuilder.GetFilePath("en"), renamedFilename);

            try
            {
                var itemDb = dbBuilder.LoadFromFile(new CultureInfo("en"));
            }
            finally
            {
                File.Move(renamedFilename, dbBuilder.GetFilePath("en"));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Newtonsoft.Json.JsonReaderException))]
        public void RebuildItemDatabase_LoadFromFile_InvalidDatabaseFile()
        {
            ItemsDatabaseBuilder dbBuilder = new ItemsDatabaseBuilder();

            string renamedFilename = "renamedDB.json";
            File.Move(dbBuilder.GetFilePath("en"), renamedFilename);
            File.WriteAllText(dbBuilder.GetFilePath("en"), "invalid file");

            try
            {
                
                var itemDb = dbBuilder.LoadFromFile(new CultureInfo("en"));
            }
            finally
            {
                File.Delete(dbBuilder.GetFilePath("en"));
                File.Move(renamedFilename, dbBuilder.GetFilePath("en"));
            }
        }

        [TestMethod]
        [Ignore]
        [TestCategory("Long Running")]
        public void RebuildItemDatabase_RebuildItemDatabase_Success()
        {
            AutoResetEvent progressEvent = new AutoResetEvent(false);
            AutoResetEvent completedEvent = new AutoResetEvent(false);

            ItemsDatabaseBuilder dbBuilder = new ItemsDatabaseBuilder();
            var cancelToken = new CancellationTokenSource();
            int progress = 0;
            int totalRequests = 0;

            var sw = new Stopwatch();
            sw.Start();
            totalRequests = dbBuilder.RebuildItemDatabase(
                CultureInfo.CurrentUICulture,
                () =>
                {
                    progress++;
                    Console.WriteLine("Rebuild at {0} percent", ((double)progress / (double)totalRequests) * 100);
                    progressEvent.Set();
                },
                () =>
                {
                    Console.WriteLine("Rebuild complete");
                    completedEvent.Set();
                },
                cancelToken.Token);

            int i = 0;
            while (progress < totalRequests)
            {
                Assert.IsTrue(progressEvent.WaitOne());
                Assert.AreEqual(++i, progress);
            }

            Assert.IsTrue(completedEvent.WaitOne());
            sw.Stop();

            Console.WriteLine("It took {0} minutes to rebuild the database", sw.Elapsed.TotalMinutes);
        }

        [TestMethod]
        public void RebuildItemDatabase_RebuildItemDatabase_Canceled()
        {
            AutoResetEvent progressEvent = new AutoResetEvent(false);
            AutoResetEvent completedEvent = new AutoResetEvent(false);

            ItemsDatabaseBuilder dbBuilder = new ItemsDatabaseBuilder();
            var cancelToken = new CancellationTokenSource();
            int progress = 0;
            int totalRequests = 0;

            totalRequests = dbBuilder.RebuildItemDatabase(
                CultureInfo.CurrentUICulture,
                () =>
                {
                    progress++;
                    Console.WriteLine("Rebuild at {0} percent", ((double)progress / (double)totalRequests) * 100);
                    progressEvent.Set();
                },
                () =>
                {
                    Console.WriteLine("Rebuild complete");
                    completedEvent.Set();
                },
                cancelToken.Token);

            for (int i = 1; i <= 3; i++)
            {
                Assert.IsTrue(progressEvent.WaitOne(5000));
                Assert.AreEqual(i, progress);
            }

            // Cancel it
            cancelToken.Cancel();
            Assert.IsTrue(progressEvent.WaitOne(5000));
            Assert.IsFalse(progressEvent.WaitOne(5000));
        }
    }
}
