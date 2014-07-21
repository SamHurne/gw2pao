using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Util
{
    /// <summary>
    /// Collection of calculation utility methods
    /// </summary>
    public static class CalcUtil
    {
        public static double MapConversionFactor = 39.3701;

        /// <summary>
        /// Converts a given mumble-link Point to a map position.
        /// Note: mumble-link is in meters, while map position is in inches
        /// </summary>
        /// <param name="mumbleLinkPoint">The mumble link point to convert</param>
        /// <returns>The point in Map-Coordinates</returns>
        public static API.Data.Point ConvertToMapPosition(API.Data.Point mumbleLinkPoint)
        {
            return new API.Data.Point(mumbleLinkPoint.X * MapConversionFactor,
                                      mumbleLinkPoint.Y * MapConversionFactor,
                                      mumbleLinkPoint.Z * MapConversionFactor);
        }

        /// <summary>
        /// Calculates the distance between 2 points, using x, y, and z coordinates
        /// </summary>
        /// <param name="ptA">The first point</param>
        /// <param name="ptB">The second point</param>
        /// <returns>The distance between ptA and ptB, using x, y, and z coordinates</returns>
        public static double CalculateDistance(API.Data.Point ptA, API.Data.Point ptB)
        {
            // Note: Removing inclusion of the Z component, since it seems like the resulting distance isn't accurate in the game (might be a problem with the Z axis reported by the game)
            //return Math.Sqrt(Math.Pow(Math.Abs((ptB.X - ptA.X)), 2) + Math.Pow(Math.Abs((ptB.Y - ptA.Y)), 2) + Math.Pow(Math.Abs((ptB.Z - ptA.Z)), 2));
            return Math.Sqrt(Math.Pow(Math.Abs((ptB.X - ptA.X)), 2) + Math.Pow(Math.Abs((ptB.Y - ptA.Y)), 2));
        }

        /// <summary>
        /// Calculates the angle between 2 vectors
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>The angle between the 2 vectors, in degrees</returns>
        public static double CalculateAngle(Vector v1, Vector v2)
        {
            double angle1 = Math.Atan2(v1.X, v1.Y);
            double angle2 = Math.Atan2(v2.X, v2.Y);
            double angleRad = angle1 - angle2;
            return angleRad * (180 / Math.PI);
        }

        /// <summary>
        /// Helper class for representing a vector
        /// </summary>
        public class Vector
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }

            public Vector(double x, double y, double z)
            {
                this.X = x;
                this.Y = y;
            }

            public static Vector CreateVector(API.Data.Point pt1, API.Data.Point pt2)
            {
                return new Vector(pt2.X - pt1.X, pt2.Y - pt1.Y, pt2.Z - pt1.Z);
            }
        }
    }
}
