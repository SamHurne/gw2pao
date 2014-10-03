using System;
using System.IO;
using GW2PAO.API.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GW2PAO.API.UnitTest
{
    [TestClass]
    public class DungeonsTableUnitTest
    {
        [TestMethod]
        public void DungeonsTable_Constructor()
        {
            DungeonsTable dt = new DungeonsTable();
            Assert.IsNotNull(dt);
            Assert.IsNotNull(dt.Dungeons);
        }

        [TestMethod]
        public void DungeonsTable_LoadTable_Success()
        {
            DungeonsTable dt = DungeonsTable.LoadTable();
            Assert.IsNotNull(dt);
            Assert.IsNotNull(dt.Dungeons);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void DungeonsTable_LoadTable_MissingFile()
        {
            string renamedFilename = "renamedFile.xml";
            File.Move(DungeonsTable.FileName, renamedFilename);

            try
            {
                DungeonsTable dt = DungeonsTable.LoadTable();
            }
            finally
            {
                File.Move(renamedFilename, DungeonsTable.FileName);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DungeonsTable_LoadTable_InvalidFile()
        {
            string renamedFilename = "renamedFile.xml";
            File.Move(DungeonsTable.FileName, renamedFilename);
            File.WriteAllText(DungeonsTable.FileName, "invalid data");

            try
            {
                DungeonsTable dt = DungeonsTable.LoadTable();
            }
            finally
            {
                File.Delete(DungeonsTable.FileName);
                File.Move(renamedFilename, DungeonsTable.FileName);
            }
        }

        [TestMethod]
        public void DungeonsTable_CreateTable_Success()
        {
            File.Delete(DungeonsTable.FileName);
            DungeonsTable.CreateTable();
            Assert.IsTrue(File.Exists(DungeonsTable.FileName));
        }
    }
}
