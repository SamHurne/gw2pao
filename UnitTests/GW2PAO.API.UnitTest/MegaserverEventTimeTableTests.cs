using System;
using System.IO;
using System.Linq;
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
            MegaserverEventTimeTable mett = new MegaserverEventTimeTable();
            Assert.IsNotNull(mett);
            Assert.IsNotNull(mett.WorldEvents);
        }

        [TestMethod]
        public void MegaserverEventTimeTable_LoadTable_Standard_Success()
        {
            MegaserverEventTimeTable mett = MegaserverEventTimeTable.LoadTable(false);
            Assert.IsNotNull(mett);
            Assert.IsNotNull(mett.WorldEvents);
        }

        [TestMethod]
        public void MegaserverEventTimeTable_LoadTable_Adjusted_Success()
        {
            MegaserverEventTimeTable mett = MegaserverEventTimeTable.LoadTable(true);
            Assert.IsNotNull(mett);
            Assert.IsNotNull(mett.WorldEvents);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void MegaserverEventTimeTable_LoadTable_Standard_MissingFile()
        {
            string renamedFilename = "renamedFile.xml";
            File.Move(MegaserverEventTimeTable.StandardFilename, renamedFilename);

            try
            {
                MegaserverEventTimeTable mett = MegaserverEventTimeTable.LoadTable(false);
            }
            finally
            {
                File.Move(renamedFilename, MegaserverEventTimeTable.StandardFilename);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void MegaserverEventTimeTable_LoadTable_Adjusted_MissingFile()
        {
            string renamedFilename = "renamedFile.xml";
            File.Move(MegaserverEventTimeTable.AdjustedFilename, renamedFilename);

            try
            {
                MegaserverEventTimeTable mett = MegaserverEventTimeTable.LoadTable(true);
            }
            finally
            {
                File.Move(renamedFilename, MegaserverEventTimeTable.AdjustedFilename);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MegaserverEventTimeTable_LoadTable_Standard_InvalidFile()
        {
            string renamedFilename = "renamedFile.xml";
            File.Move(MegaserverEventTimeTable.StandardFilename, renamedFilename);
            File.WriteAllText(MegaserverEventTimeTable.StandardFilename, "invalid data");

            try
            {
                MegaserverEventTimeTable mett = MegaserverEventTimeTable.LoadTable(false);
            }
            finally
            {
                File.Delete(MegaserverEventTimeTable.StandardFilename);
                File.Move(renamedFilename, MegaserverEventTimeTable.StandardFilename);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MegaserverEventTimeTable_LoadTable_Adjusted_InvalidFile()
        {
            string renamedFilename = "renamedFile.xml";
            File.Move(MegaserverEventTimeTable.AdjustedFilename, renamedFilename);
            File.WriteAllText(MegaserverEventTimeTable.AdjustedFilename, "invalid data");

            try
            {
                MegaserverEventTimeTable mett = MegaserverEventTimeTable.LoadTable(true);
            }
            finally
            {
                File.Delete(MegaserverEventTimeTable.AdjustedFilename);
                File.Move(renamedFilename, MegaserverEventTimeTable.AdjustedFilename);
            }
        }

        [TestMethod]
        public void MegaserverEventTimeTable_CreateTable_Standard_Success()
        {
            File.Delete(MegaserverEventTimeTable.StandardFilename);
            MegaserverEventTimeTable.CreateTable(false);
            Assert.IsTrue(File.Exists(MegaserverEventTimeTable.StandardFilename));
        }

        [TestMethod]
        public void MegaserverEventTimeTable_CreateTable_Adjusted_Success()
        {
            File.Delete(MegaserverEventTimeTable.AdjustedFilename);
            MegaserverEventTimeTable.CreateTable(true);
            Assert.IsTrue(File.Exists(MegaserverEventTimeTable.AdjustedFilename));
        }

        [TestMethod]
        public void MegaserverEventTimeTable_LoadTable_Standard_UniqueTimes()
        {
            MegaserverEventTimeTable mett = MegaserverEventTimeTable.LoadTable(false);

            foreach (var worldEvent in mett.WorldEvents)
            {
                Assert.AreEqual(worldEvent.ActiveTimes.Count, worldEvent.ActiveTimes.GroupBy(t => t.XmlTime).Select(at => at.First()).ToList().Count);
            }
        }

        [TestMethod]
        public void MegaserverEventTimeTable_LoadTable_Adjusted_UniqueTimes()
        {
            MegaserverEventTimeTable mett = MegaserverEventTimeTable.LoadTable(true);

            foreach (var worldEvent in mett.WorldEvents)
            {
                Assert.AreEqual(worldEvent.ActiveTimes.Count, worldEvent.ActiveTimes.GroupBy(t => t.XmlTime).Select(at => at.First()).ToList().Count);
            }
        }
    }
}
