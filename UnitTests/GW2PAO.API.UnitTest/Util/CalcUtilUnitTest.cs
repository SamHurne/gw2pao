using System;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Enums;
using GW2PAO.API.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GW2PAO.API.UnitTest.Util
{
    [TestClass]
    public class CalcUtilUnitTest
    {
        private static Random random = new Random();

        [TestMethod]
        public void ConvertToMapPosition_Basic()
        {
            Point input = new Point(random.Next(), random.Next(), random.Next());
            Point output = CalcUtil.ConvertToMapPosition(input);

            Assert.AreEqual(input.X * CalcUtil.MapConversionFactor, output.X);
            Assert.AreEqual(input.Y * CalcUtil.MapConversionFactor, output.Y);
            Assert.AreEqual(input.Z * CalcUtil.MapConversionFactor, output.Z);
        }

        [TestMethod]
        public void CalculateDistance_NoUnits()
        {
            // Known, simple distance:
            Point ptA = new Point(0, 0);
            Point ptB = new Point(10, 0);

            double expDist = 10.0;
            double dist = CalcUtil.CalculateDistance(ptA, ptB);

            Assert.AreEqual(expDist, dist);

            // Random:
            ptA = new Point(random.Next(), random.Next());
            ptB = new Point(random.Next(), random.Next());

            expDist = Math.Sqrt(Math.Pow(Math.Abs((ptB.X - ptA.X)), 2) + Math.Pow(Math.Abs((ptB.Y - ptA.Y)), 2));
            dist = CalcUtil.CalculateDistance(ptA, ptB);

            Assert.AreEqual(expDist, dist);
        }

        [TestMethod]
        public void CalculateDistance_Feet()
        {
            // Known, simple distance:
            Point ptA = new Point(0, 0);
            Point ptB = new Point(0, -10);

            double expDist = 10.0 / 12.0;
            double dist = CalcUtil.CalculateDistance(ptA, ptB, Units.Feet);

            Assert.AreEqual(expDist, dist);

            // Random:
            ptA = new Point(random.Next(), random.Next());
            ptB = new Point(random.Next(), random.Next());

            expDist = (Math.Sqrt(Math.Pow(Math.Abs((ptB.X - ptA.X)), 2) + Math.Pow(Math.Abs((ptB.Y - ptA.Y)), 2))) / 12.0;
            dist = CalcUtil.CalculateDistance(ptA, ptB, Units.Feet);

            Assert.AreEqual(expDist, dist);
        }

        [TestMethod]
        public void CalculateDistance_Meters()
        {
            // Known, simple distance:
            Point ptA = new Point(0, 10);
            Point ptB = new Point(0, -10);

            double expDist = 20.0 / 39.3701;
            double dist = CalcUtil.CalculateDistance(ptA, ptB, Units.Meters);

            Assert.AreEqual(expDist, dist);

            // Random:
            ptA = new Point(random.Next(), random.Next());
            ptB = new Point(random.Next(), random.Next());

            expDist = (Math.Sqrt(Math.Pow(Math.Abs((ptB.X - ptA.X)), 2) + Math.Pow(Math.Abs((ptB.Y - ptA.Y)), 2))) / 39.3701;
            dist = CalcUtil.CalculateDistance(ptA, ptB, Units.Meters);

            Assert.AreEqual(expDist, dist);
        }

        [TestMethod]
        public void CalculateDistance_TimeDistance()
        {
            // Known, simple distance:
            Point ptA = new Point(-10, 0);
            Point ptB = new Point(10, 0);

            double expDist = 20.0 / CalcUtil.DistanceToTimeFactor;
            double dist = CalcUtil.CalculateDistance(ptA, ptB, Units.TimeDistance);

            Assert.AreEqual(expDist, dist);

            // Random:
            ptA = new Point(random.Next(), random.Next());
            ptB = new Point(random.Next(), random.Next());

            expDist = (Math.Sqrt(Math.Pow(Math.Abs((ptB.X - ptA.X)), 2) + Math.Pow(Math.Abs((ptB.Y - ptA.Y)), 2))) / CalcUtil.DistanceToTimeFactor;
            dist = CalcUtil.CalculateDistance(ptA, ptB, Units.TimeDistance);

            Assert.AreEqual(expDist, dist);
        }

        [TestMethod]
        public void CalculateTimeDistance_Basic()
        {
            double input = random.Next();
            double expOutput = input / CalcUtil.DistanceToTimeFactor;

            double output = CalcUtil.CalculateTimeDistance(input);

            Assert.AreEqual(expOutput, output);
        }

        [TestMethod]
        public void CalculateAngle_Basic()
        {
            // Known, simple angle:
            CalcUtil.Vector vA = new CalcUtil.Vector(1, 0, 0);
            CalcUtil.Vector vB = new CalcUtil.Vector(0, 1, 0);

            double expAngle = 90.0;
            double angle = CalcUtil.CalculateAngle(vA, vB);

            Assert.AreEqual(expAngle, angle);

            // Random:
            vA = new CalcUtil.Vector(random.Next(), random.Next(), random.Next());
            vB = new CalcUtil.Vector(random.Next(), random.Next(), random.Next());

            expAngle = System.Windows.Vector.AngleBetween(new System.Windows.Vector(vA.X, vA.Y), new System.Windows.Vector(vB.X, vB.Y));
            angle = CalcUtil.CalculateAngle(vA, vB);

            Assert.AreEqual(expAngle, angle, 0.0005);
        }
    }
}
