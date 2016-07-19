using System;
using System.IO;
using System.Linq;
using GW2PAO.API.Data;
using GW2PAO.API.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GW2PAO.API.UnitTest
{
    [TestClass]
    public class MegaserverEventTimeTableTests
    {
        [TestMethod]
        public void MegaserverEventTimeTable_Constructor()
        {
            WorldBossEventTimeTable mett = new WorldBossEventTimeTable();
            Assert.IsNotNull(mett);
            Assert.IsNotNull(mett.WorldEvents);
        }

        [TestMethod]
        public void MegaserverEventTimeTable_LoadTable_Standard_Success()
        {
            WorldBossEventTimeTable mett = WorldBossEventTimeTable.LoadTable(false);
            Assert.IsNotNull(mett);
            Assert.IsNotNull(mett.WorldEvents);
        }

        [TestMethod]
        public void MegaserverEventTimeTable_LoadTable_Adjusted_Success()
        {
            WorldBossEventTimeTable mett = WorldBossEventTimeTable.LoadTable(true);
            Assert.IsNotNull(mett);
            Assert.IsNotNull(mett.WorldEvents);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void MegaserverEventTimeTable_LoadTable_Standard_MissingFile()
        {
            string renamedFilename = "renamedFile.xml";
            File.Move(WorldBossEventTimeTable.StandardFilename, renamedFilename);

            try
            {
                WorldBossEventTimeTable mett = WorldBossEventTimeTable.LoadTable(false);
            }
            finally
            {
                File.Move(renamedFilename, WorldBossEventTimeTable.StandardFilename);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void MegaserverEventTimeTable_LoadTable_Adjusted_MissingFile()
        {
            string renamedFilename = "renamedFile.xml";
            File.Move(WorldBossEventTimeTable.AdjustedFilename, renamedFilename);

            try
            {
                WorldBossEventTimeTable mett = WorldBossEventTimeTable.LoadTable(true);
            }
            finally
            {
                File.Move(renamedFilename, WorldBossEventTimeTable.AdjustedFilename);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MegaserverEventTimeTable_LoadTable_Standard_InvalidFile()
        {
            string renamedFilename = "renamedFile.xml";
            File.Move(WorldBossEventTimeTable.StandardFilename, renamedFilename);
            File.WriteAllText(WorldBossEventTimeTable.StandardFilename, "invalid data");

            try
            {
                WorldBossEventTimeTable mett = WorldBossEventTimeTable.LoadTable(false);
            }
            finally
            {
                File.Delete(WorldBossEventTimeTable.StandardFilename);
                File.Move(renamedFilename, WorldBossEventTimeTable.StandardFilename);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MegaserverEventTimeTable_LoadTable_Adjusted_InvalidFile()
        {
            string renamedFilename = "renamedFile.xml";
            File.Move(WorldBossEventTimeTable.AdjustedFilename, renamedFilename);
            File.WriteAllText(WorldBossEventTimeTable.AdjustedFilename, "invalid data");

            try
            {
                WorldBossEventTimeTable mett = WorldBossEventTimeTable.LoadTable(true);
            }
            finally
            {
                File.Delete(WorldBossEventTimeTable.AdjustedFilename);
                File.Move(renamedFilename, WorldBossEventTimeTable.AdjustedFilename);
            }
        }

        [TestMethod]
        public void MegaserverEventTimeTable_CreateTable_Standard_Success()
        {
            File.Delete(WorldBossEventTimeTable.StandardFilename);
            WorldBossEventTimeTable.CreateTable(false);
            Assert.IsTrue(File.Exists(WorldBossEventTimeTable.StandardFilename));
        }

        [TestMethod]
        public void MegaserverEventTimeTable_CreateTable_Adjusted_Success()
        {
            File.Delete(WorldBossEventTimeTable.AdjustedFilename);
            WorldBossEventTimeTable.CreateTable(true);
            Assert.IsTrue(File.Exists(WorldBossEventTimeTable.AdjustedFilename));
        }

        [TestMethod]
        public void MegaserverEventTimeTable_LoadTable_Standard_UniqueTimes()
        {
            WorldBossEventTimeTable mett = WorldBossEventTimeTable.LoadTable(false);

            foreach (var worldEvent in mett.WorldEvents)
            {
                Assert.AreEqual(worldEvent.ActiveTimes.Count, worldEvent.ActiveTimes.GroupBy(t => t.XmlTime).Select(at => at.First()).ToList().Count);
            }
        }

        [TestMethod]
        public void MegaserverEventTimeTable_LoadTable_Adjusted_UniqueTimes()
        {
            WorldBossEventTimeTable mett = WorldBossEventTimeTable.LoadTable(true);

            foreach (var worldEvent in mett.WorldEvents)
            {
                Assert.AreEqual(worldEvent.ActiveTimes.Count, worldEvent.ActiveTimes.GroupBy(t => t.XmlTime).Select(at => at.First()).ToList().Count);
            }
        }
    }
}
