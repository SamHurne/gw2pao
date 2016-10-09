using System;
using GW2NET.Common.Drawing;
using GW2NET.Maps;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Map = GW2NET.Maps.Map;
using Continent = GW2NET.Maps.Continent;
using Rectangle = GW2NET.Common.Drawing.Rectangle;

namespace GW2PAO.API.UnitTest.Util
{
    [TestClass]
    public class MapsHelperUnitTest
    {
        private static Random random = new Random();

        [TestMethod]
        public void ConvertToMapPos_ConvertToWorldPos_Equals_OriginalInput_NoZoom()
        {
            int x = random.Next();
            int y = random.Next();
            var mapRectangle = new Data.Entities.Rectangle() { X = x, Y = y, Width = 100, Height = 100 };
            x = random.Next();
            y = random.Next();
            var continentRectangle = new Data.Entities.Rectangle() { X = x, Y = y, Width = 100, Height = 100 };

            Point inputPoint = new Point(
                random.Next((int)continentRectangle.X, (int)(continentRectangle.X + continentRectangle.Width)),
                random.Next((int)continentRectangle.Y, (int)(continentRectangle.Y + continentRectangle.Height)));

            Point mapPos = MapsHelper.ConvertToMapPos(continentRectangle, mapRectangle, inputPoint);
            Point worldPos = MapsHelper.ConvertToWorldPos(continentRectangle, mapRectangle, mapPos);

            Assert.AreEqual(inputPoint.X, worldPos.X, 0.5);
            Assert.AreEqual(inputPoint.Y, worldPos.Y, 0.5);
            Assert.AreEqual(inputPoint.Z, worldPos.Z, 0.5);
        }

        [TestMethod]
        public void ConvertToMapPos_ConvertToWorldPos_Equals_OriginalInput_WithZoom()
        {
            int x = random.Next();
            int y = random.Next();
            var mapRectangle = new Data.Entities.Rectangle() { X = x, Y = y, Width = 100, Height = 100 };
            x = random.Next();
            y = random.Next();
            var continentRectangle = new Data.Entities.Rectangle() { X = x, Y = y, Width = 100, Height = 100 };

            int maxZoom = random.Next();

            Point inputPoint = new Point(
                random.Next((int)continentRectangle.X, (int)(continentRectangle.X + continentRectangle.Width)),
                random.Next((int)continentRectangle.Y, (int)(continentRectangle.Y + continentRectangle.Height)));

            Point mapPos = MapsHelper.ConvertToMapPos(continentRectangle, mapRectangle, inputPoint, maxZoom / 2, maxZoom);
            Point worldPos = MapsHelper.ConvertToWorldPos(continentRectangle, mapRectangle, mapPos, maxZoom / 2, maxZoom);

            Assert.AreEqual(inputPoint.X, worldPos.X, 0.5);
            Assert.AreEqual(inputPoint.Y, worldPos.Y, 0.5);
            Assert.AreEqual(inputPoint.Z, worldPos.Z, 0.5);
        }
    }
}
