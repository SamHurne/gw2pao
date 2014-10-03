using System;
using System.Diagnostics;
using System.IO;
using GW2PAO.API.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GW2PAO.API.UnitTest
{
    [TestClass]
    public class DungeonsServiceUnitTest
    {
        [TestMethod]
        public void DungeonsService_Constructor()
        {
            DungeonsService ds = new DungeonsService();
            Assert.IsNotNull(ds);
            Assert.IsNull(ds.DungeonsTable);
        }

        [TestMethod]
        public void DungeonsService_LoadTable_Success()
        {
            DungeonsService ds = new DungeonsService();

            var sw = new Stopwatch();
            sw.Start();
            ds.LoadTable();
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(ds.DungeonsTable);
            Assert.IsTrue(ds.DungeonsTable.Dungeons.Count > 0);
        }

        [TestMethod]
        public void DungeonsService_LoadTable_FileMissing()
        {
            File.Delete(DungeonsTable.FileName);

            DungeonsService ds = new DungeonsService();

            var sw = new Stopwatch();
            sw.Start();
            ds.LoadTable();
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(ds.DungeonsTable);
            Assert.IsTrue(ds.DungeonsTable.Dungeons.Count > 0);
        }

        [TestMethod]
        public void DungeonsService_LoadTable_InvalidFile()
        {
            File.WriteAllText(DungeonsTable.FileName, "invalid contents");

            DungeonsService ds = new DungeonsService();

            var sw = new Stopwatch();
            sw.Start();
            ds.LoadTable();
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(ds.DungeonsTable);
            Assert.IsTrue(ds.DungeonsTable.Dungeons.Count > 0);
        }
    }
}
