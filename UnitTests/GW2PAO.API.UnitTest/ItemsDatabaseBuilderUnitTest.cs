using System;
using System.Diagnostics;
using System.Threading;
using GW2PAO.API.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GW2PAO.API.UnitTest
{
    [TestClass]
    public class ItemsDatabaseBuilderUnitTest
    {
        [TestMethod]
        public void RebuildItemDatabase_LoadFromFile()
        {
        }

        [TestMethod]
        public void RebuildItemDatabase_RebuildItemDatabase()
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
    }
}
