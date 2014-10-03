using System;
using GW2DotNET.Entities.Maps;
using GW2PAO.API.Data;
using GW2PAO.API.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GW2PAO.API.UnitTest.Util
{
    [TestClass]
    public class MapsHelperUnitTest
    {
        private static Random random = new Random();

        [TestMethod]
        public void ConvertToMapPos_ConvertToWorldPos_Equals_OriginalInput_NoZoom()
        {
            Map testMap = new Map();
            int x = random.Next();
            int y = random.Next();
            testMap.MapRectangle = new Rectangle(new Point2D(x, y), new Point2D(random.Next(x, int.MaxValue), random.Next(y, int.MaxValue)));
            x = random.Next();
            y = random.Next();
            testMap.ContinentRectangle = new Rectangle(new Point2D(x, y), new Point2D(random.Next(x, int.MaxValue), random.Next(y, int.MaxValue)));
            testMap.Continent = new Continent();

            Point inputPoint = new Point(
                random.Next((int)testMap.ContinentRectangle.X, (int)(testMap.ContinentRectangle.X + testMap.ContinentRectangle.Width)),
                random.Next((int)testMap.ContinentRectangle.Y, (int)(testMap.ContinentRectangle.Y + testMap.ContinentRectangle.Height)));

            Point mapPos = MapsHelper.ConvertToMapPos(testMap, inputPoint);
            Point worldPos = MapsHelper.ConvertToWorldPos(testMap, mapPos);

            Assert.AreEqual(inputPoint.X, worldPos.X, 0.5);
            Assert.AreEqual(inputPoint.Y, worldPos.Y, 0.5);
            Assert.AreEqual(inputPoint.Z, worldPos.Z, 0.5);
        }

        [TestMethod]
        public void ConvertToMapPos_ConvertToWorldPos_Equals_OriginalInput_WithZoom()
        {
            Map testMap = new Map();
            int x = random.Next();
            int y = random.Next();
            testMap.MapRectangle = new Rectangle(new Point2D(x, y), new Point2D(random.Next(x, int.MaxValue), random.Next(y, int.MaxValue)));
            x = random.Next();
            y = random.Next();
            testMap.ContinentRectangle = new Rectangle(new Point2D(x, y), new Point2D(random.Next(x, int.MaxValue), random.Next(y, int.MaxValue)));
            testMap.Continent = new Continent();

            testMap.MaximumLevel = random.Next();

            Point inputPoint = new Point(
                random.Next((int)testMap.ContinentRectangle.X, (int)(testMap.ContinentRectangle.X + testMap.ContinentRectangle.Width)),
                random.Next((int)testMap.ContinentRectangle.Y, (int)(testMap.ContinentRectangle.Y + testMap.ContinentRectangle.Height)));

            Point mapPos = MapsHelper.ConvertToMapPos(testMap, inputPoint, testMap.MaximumLevel / 2);
            Point worldPos = MapsHelper.ConvertToWorldPos(testMap, mapPos, testMap.MaximumLevel / 2);

            Assert.AreEqual(inputPoint.X, worldPos.X, 0.5);
            Assert.AreEqual(inputPoint.Y, worldPos.Y, 0.5);
            Assert.AreEqual(inputPoint.Z, worldPos.Z, 0.5);
        }
    }
}
