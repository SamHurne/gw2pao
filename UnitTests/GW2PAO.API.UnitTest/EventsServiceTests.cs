using System;
using System.Diagnostics;
using System.IO;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;
using GW2PAO.API.Providers;
using GW2PAO.API.Services;
using GW2PAO.API.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GW2PAO.API.UnitTest
{
    [TestClass]
    public class EventsServiceTests
    {
        [TestMethod]
        public void EventsService_LoadTable_Standard_Success()
        {
            EventsService es = new EventsService();

            var sw = new Stopwatch();
            sw.Start();
            es.LoadTables(false);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(es.WorldBossEventTimeTable);
            Assert.IsTrue(es.WorldBossEventTimeTable.WorldEvents.Count > 0);
        }

        [TestMethod]
        public void EventsService_LoadTable_Standard_MissingFile()
        {
            File.Delete(WorldBossEventTimeTable.StandardFilename);

            EventsService es = new EventsService();

            var sw = new Stopwatch();
            sw.Start();
            es.LoadTables(false);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(es.WorldBossEventTimeTable);
            Assert.IsTrue(es.WorldBossEventTimeTable.WorldEvents.Count > 0);
        }

        [TestMethod]
        public void EventsService_LoadTable_Standard_InvalidFile()
        {
            File.WriteAllText(WorldBossEventTimeTable.StandardFilename, "invalid contents");

            EventsService es = new EventsService();

            var sw = new Stopwatch();
            sw.Start();
            es.LoadTables(false);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(es.WorldBossEventTimeTable);
            Assert.IsTrue(es.WorldBossEventTimeTable.WorldEvents.Count > 0);
        }

        [TestMethod]
        public void EventsService_LoadTable_Adjusted_Success()
        {
            EventsService es = new EventsService();

            var sw = new Stopwatch();
            sw.Start();
            es.LoadTables(true);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(es.WorldBossEventTimeTable);
            Assert.IsTrue(es.WorldBossEventTimeTable.WorldEvents.Count > 0);
        }

        [TestMethod]
        public void EventsService_LoadTable_Adjusted_MissingFile()
        {
            File.Delete(WorldBossEventTimeTable.AdjustedFilename);

            EventsService es = new EventsService();

            var sw = new Stopwatch();
            sw.Start();
            es.LoadTables(true);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(es.WorldBossEventTimeTable);
            Assert.IsTrue(es.WorldBossEventTimeTable.WorldEvents.Count > 0);
        }

        [TestMethod]
        public void EventsService_LoadTable_Adjusted_InvalidFile()
        {
            File.WriteAllText(WorldBossEventTimeTable.AdjustedFilename, "invalid contents");

            EventsService es = new EventsService();

            var sw = new Stopwatch();
            sw.Start();
            es.LoadTables(true);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.IsNotNull(es.WorldBossEventTimeTable);
            Assert.IsTrue(es.WorldBossEventTimeTable.WorldEvents.Count > 0);
        }

        [TestMethod]
        public void EventsService_GetState_Id_Success_Active()
        {
            var timeMock = new Mock<ITimeProvider>();

            EventsService es = new EventsService(null, timeMock.Object);
            es.LoadTables(false);

            var validEvent = es.WorldBossEventTimeTable.WorldEvents[0];
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

            EventsService es = new EventsService(null, timeMock.Object);
            es.LoadTables(true);

            var validEvent = es.WorldBossEventTimeTable.WorldEvents[0];
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

            EventsService es = new EventsService(null, timeMock.Object);
            es.LoadTables(false);

            var validEvent = es.WorldBossEventTimeTable.WorldEvents[0];
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
            es.LoadTables(false);

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
            var timeMock = new Mock<ITimeProvider>();

            EventsService es = new EventsService(null, timeMock.Object);
            es.LoadTables(false);

            var validEvent = es.WorldBossEventTimeTable.WorldEvents[0];
            var activeTime = DateTimeOffset.UtcNow.Date.Add(validEvent.ActiveTimes[0].Time).AddMinutes(1);
            timeMock.Setup(t => t.CurrentTime).Returns(activeTime);

            var sw = new Stopwatch();
            sw.Start();
            var state = es.GetState(validEvent);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.AreEqual(EventState.Active, state);
        }

        [TestMethod]
        public void EventsService_GetState_WorldEvent_Success_Warmup()
        {
            var timeMock = new Mock<ITimeProvider>();

            EventsService es = new EventsService(null, timeMock.Object);
            es.LoadTables(true);

            var validEvent = es.WorldBossEventTimeTable.WorldEvents[0];
            var activeTime = DateTimeOffset.UtcNow.Date.Add(validEvent.ActiveTimes[0].Time).AddMinutes((validEvent.WarmupDuration.Time.TotalMinutes * -1) + 1);
            timeMock.Setup(t => t.CurrentTime).Returns(activeTime);

            var sw = new Stopwatch();
            sw.Start();
            var state = es.GetState(validEvent);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.AreEqual(EventState.Warmup, state);
        }

        [TestMethod]
        public void EventsService_GetState_WorldEvent_Success_Inactive()
        {
            var timeMock = new Mock<ITimeProvider>();

            EventsService es = new EventsService(null, timeMock.Object);
            es.LoadTables(false);

            var validEvent = es.WorldBossEventTimeTable.WorldEvents[0];
            var activeTime = DateTimeOffset.UtcNow.Date.Add(validEvent.ActiveTimes[0].Time).AddMinutes(-10);
            timeMock.Setup(t => t.CurrentTime).Returns(activeTime);

            var sw = new Stopwatch();
            sw.Start();
            var state = es.GetState(validEvent);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.AreEqual(EventState.Inactive, state);
        }

        [TestMethod]
        public void EventsService_GetState_WorldEvent_Null_Invalid()
        {
            EventsService es = new EventsService();
            es.LoadTables(false);

            var sw = new Stopwatch();
            sw.Start();
            var state = es.GetState(null);
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.AreEqual(EventState.Unknown, state);
        }

        [TestMethod]
        public void EventsService_GetState_WorldEvent_Blank_Inactive()
        {
            EventsService es = new EventsService();
            es.LoadTables(false);

            var sw = new Stopwatch();
            sw.Start();
            var state = es.GetState(new WorldBossEvent());
            sw.Stop();
            Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);

            Assert.AreEqual(EventState.Inactive, state);
        }

        [TestMethod]
        public void EventsService_GetState_GetTimeUntilActive_Success()
        {
            var testEvent = new WorldBossEvent();
            testEvent.ActiveTimes.Add(new SerializableTimespan(2, 0, 0));

            var timeMock = new Mock<ITimeProvider>();
            timeMock.Setup(t => t.CurrentTime).Returns(new DateTimeOffset(new DateTime(2014, 10, 22, 0, 0, 0)));
            EventsService es = new EventsService(null, timeMock.Object);

            var timeUntilActive = es.GetTimeUntilActive(testEvent);

            Assert.AreEqual(new TimeSpan(2, 0, 0), timeUntilActive);
        }

        [TestMethod]
        public void EventsService_GetState_GetTimeUntilActive_Success_RollOver()
        {
            var testEvent = new WorldBossEvent();
            testEvent.ActiveTimes.Add(new SerializableTimespan(2, 0, 0));

            var timeMock = new Mock<ITimeProvider>();
            timeMock.Setup(t => t.CurrentTime).Returns(new DateTimeOffset(new DateTime(2014, 10, 22, 3, 0, 0)));
            EventsService es = new EventsService(null, timeMock.Object);

            var timeUntilActive = es.GetTimeUntilActive(testEvent);

            Assert.AreEqual(new TimeSpan(23, 0, 0), timeUntilActive);
        }

        [TestMethod]
        public void EventsService_GetState_GetTimeUntilActive_Null_Invalid()
        {
            EventsService es = new EventsService();

            var timeUntilActive = es.GetTimeUntilActive(null);

            Assert.AreEqual(TimeSpan.MinValue, timeUntilActive);
        }

        [TestMethod]
        public void EventsService_GetState_GetTimeUntilActive_Blank_Invalid()
        {
            EventsService es = new EventsService();

            var timeUntilActive = es.GetTimeUntilActive(new WorldBossEvent());

            Assert.AreEqual(TimeSpan.MinValue, timeUntilActive);
        }

        [TestMethod]
        public void EventsService_GetState_GetTimeSinceActive_Success()
        {
            var testEvent = new WorldBossEvent();
            testEvent.ActiveTimes.Add(new SerializableTimespan(2, 0, 0));

            var timeMock = new Mock<ITimeProvider>();
            timeMock.Setup(t => t.CurrentTime).Returns(new DateTimeOffset(new DateTime(2014, 10, 22, 4, 0, 0)));
            EventsService es = new EventsService(null, timeMock.Object);

            var timeUntilActive = es.GetTimeSinceActive(testEvent);

            Assert.AreEqual(new TimeSpan(2, 0, 0), timeUntilActive);
        }

        [TestMethod]
        public void EventsService_GetState_GetTimeSinceActive_Success_RollOver()
        {
            var testEvent = new WorldBossEvent();
            testEvent.ActiveTimes.Add(new SerializableTimespan(1, 0, 0));

            var timeMock = new Mock<ITimeProvider>();
            timeMock.Setup(t => t.CurrentTime).Returns(new DateTimeOffset(new DateTime(2014, 10, 22, 23, 0, 0)));
            EventsService es = new EventsService(null, timeMock.Object);

            var timeUntilActive = es.GetTimeSinceActive(testEvent);

            Assert.AreEqual(new TimeSpan(22, 0, 0), timeUntilActive);
        }

        [TestMethod]
        public void EventsService_GetState_GetTimeSinceActive_Null_Invalid()
        {
            EventsService es = new EventsService();

            var timeUntilActive = es.GetTimeSinceActive(null);

            Assert.AreEqual(TimeSpan.MinValue, timeUntilActive);
        }

        [TestMethod]
        public void EventsService_GetState_GetTimeSinceActive_Blank_Invalid()
        {
            EventsService es = new EventsService();

            var timeUntilActive = es.GetTimeSinceActive(new WorldBossEvent());

            Assert.AreEqual(TimeSpan.MinValue, timeUntilActive);
        }
    }
}
