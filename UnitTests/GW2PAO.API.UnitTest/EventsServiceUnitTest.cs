using System;
using System.Diagnostics;
using System.IO;
using GW2PAO.API.Data.Enums;
using GW2PAO.API.Services;
using GW2PAO.API.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GW2PAO.API.UnitTest
{
    [TestClass]
    public class EventsServiceUnitTest
    {
        [TestMethod]
        public void EventsService_LoadTable_Standard_Success()
        {
            EventsService es = new EventsService();

            var sw = new Stopwatch();
            sw.Start();
            es.LoadTable(false);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(es.EventTimeTable);
            Assert.IsTrue(es.EventTimeTable.WorldEvents.Count > 0);
        }

        [TestMethod]
        public void EventsService_LoadTable_Standard_MissingFile()
        {
            File.Delete(MegaserverEventTimeTable.StandardFilename);

            EventsService es = new EventsService();

            var sw = new Stopwatch();
            sw.Start();
            es.LoadTable(false);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(es.EventTimeTable);
            Assert.IsTrue(es.EventTimeTable.WorldEvents.Count > 0);
        }

        [TestMethod]
        public void EventsService_LoadTable_Standard_InvalidFile()
        {
            File.WriteAllText(MegaserverEventTimeTable.StandardFilename, "invalid contents");

            EventsService es = new EventsService();

            var sw = new Stopwatch();
            sw.Start();
            es.LoadTable(false);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(es.EventTimeTable);
            Assert.IsTrue(es.EventTimeTable.WorldEvents.Count > 0);
        }

        [TestMethod]
        public void EventsService_LoadTable_Adjusted_Success()
        {
            EventsService es = new EventsService();

            var sw = new Stopwatch();
            sw.Start();
            es.LoadTable(true);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(es.EventTimeTable);
            Assert.IsTrue(es.EventTimeTable.WorldEvents.Count > 0);
        }

        [TestMethod]
        public void EventsService_LoadTable_Adjusted_MissingFile()
        {
            File.Delete(MegaserverEventTimeTable.AdjustedFilename);

            EventsService es = new EventsService();

            var sw = new Stopwatch();
            sw.Start();
            es.LoadTable(true);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(es.EventTimeTable);
            Assert.IsTrue(es.EventTimeTable.WorldEvents.Count > 0);
        }

        [TestMethod]
        public void EventsService_LoadTable_Adjusted_InvalidFile()
        {
            File.WriteAllText(MegaserverEventTimeTable.AdjustedFilename, "invalid contents");

            EventsService es = new EventsService();

            var sw = new Stopwatch();
            sw.Start();
            es.LoadTable(true);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(es.EventTimeTable);
            Assert.IsTrue(es.EventTimeTable.WorldEvents.Count > 0);
        }

        [TestMethod]
        public void EventsService_GetState_Id_Success_Active()
        {
            var timeMock = new Mock<ITimeProvider>();

            EventsService es = new EventsService(timeMock.Object);
            es.LoadTable(false);

            var validEvent = es.EventTimeTable.WorldEvents[0];
            var activeTime = DateTimeOffset.UtcNow.Date.Add(validEvent.ActiveTimes[0].Time).AddMinutes(1);
            timeMock.Setup(t => t.CurrentTime).Returns(activeTime);

            var sw = new Stopwatch();
            sw.Start();
            var state = es.GetState(validEvent.ID);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.AreEqual(EventState.Active, state);
        }

        [TestMethod]
        public void EventsService_GetState_Id_Success_Warmup()
        {
            var timeMock = new Mock<ITimeProvider>();

            EventsService es = new EventsService(timeMock.Object);
            es.LoadTable(true);

            var validEvent = es.EventTimeTable.WorldEvents[0];
            var activeTime = DateTimeOffset.UtcNow.Date.Add(validEvent.ActiveTimes[0].Time).AddMinutes((validEvent.WarmupDuration.Time.TotalMinutes * - 1) + 1);
            timeMock.Setup(t => t.CurrentTime).Returns(activeTime);

            var sw = new Stopwatch();
            sw.Start();
            var state = es.GetState(validEvent.ID);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.AreEqual(EventState.Warmup, state);
        }

        [TestMethod]
        public void EventsService_GetState_Id_Success_Inactive()
        {
            var timeMock = new Mock<ITimeProvider>();

            EventsService es = new EventsService(timeMock.Object);
            es.LoadTable(false);

            var validEvent = es.EventTimeTable.WorldEvents[0];
            var activeTime = DateTimeOffset.UtcNow.Date.Add(validEvent.ActiveTimes[0].Time).AddMinutes(-10);
            timeMock.Setup(t => t.CurrentTime).Returns(activeTime);

            var sw = new Stopwatch();
            sw.Start();
            var state = es.GetState(validEvent.ID);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.AreEqual(EventState.Inactive, state);
        }

        [TestMethod]
        public void EventsService_GetState_Id_Invalid()
        {
            EventsService es = new EventsService();
            es.LoadTable(false);

            var sw = new Stopwatch();
            sw.Start();
            var state = es.GetState(Guid.NewGuid());
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.AreEqual(EventState.Unknown, state);
        }

        [TestMethod]
        public void EventsService_GetState_WorldEvent_Success_Active()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void EventsService_GetState_WorldEvent_Success_Warmup()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void EventsService_GetState_WorldEvent_Success_Inactive()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void EventsService_GetState_WorldEvent_Invalid()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void EventsService_GetState_GetTimeUntilActive_Success()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void EventsService_GetState_GetTimeUntilActive_Invalid()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void EventsService_GetState_GetTimeSinceActive_Success()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void EventsService_GetState_GetTimeSinceActive_Invalid()
        {
            throw new NotImplementedException();
        }
    }
}
