using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2NET.Maps;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;

namespace GW2PAO.API.Util
{
    public static class MapsHelper
    {
        public static int WorldToMapRatio = 24;
        public static double MapToWorldRatio = 1.0 / WorldToMapRatio;

        /// <summary>
        /// Converts a point in the World coordinate system to one in the Maps coordinate system
        /// </summary>
        /// <param name="map">The map that the input is located in</param>
        /// <param name="input">The input point to convert, in World coordinates</param>
        /// <param name="zoom">Zoom level to consider when calculating the Map position</param>
        /// <returns>The input point converted to a Map position</returns>
        public static Point ConvertToMapPos(Map map, Point input, int zoom)
        {
            int factor = 1 << (map.Continent.MaximumZoom - zoom);
            Point scaledInput = new Point(input.X * factor, input.Y * factor);

            double mapPosX = (scaledInput.X - map.ContinentRectangle.X) * WorldToMapRatio + map.MapRectangle.X;
            double mapPosY = (map.MapRectangle.Y + map.MapRectangle.Height) - (scaledInput.Y - map.ContinentRectangle.Y) * WorldToMapRatio;

            return new Point(mapPosX, mapPosY, input.Z);
        }

        /// <summary>
        /// Converts a point in the World coordinate system to one in the Maps coordinate system
        /// </summary>
        /// <param name="map">The map that the input is located in</param>
        /// <param name="input">The input point to convert, in World coordinates</param>
        /// <returns>The input point converted to a Map position</returns>
        public static Point ConvertToMapPos(Map map, Point input)
        {
            double mapPosX = (input.X - map.ContinentRectangle.X) * WorldToMapRatio + map.MapRectangle.X;
            double mapPosY = (map.MapRectangle.Y + map.MapRectangle.Height) - (input.Y - map.ContinentRectangle.Y) * WorldToMapRatio;

            return new Point(mapPosX, mapPosY, input.Z);
        }

        /// <summary>
        /// Converts a point in the Map coordinate system to one in the World coordinate system
        /// </summary>
        /// <param name="map">The map that the input is located in</param>
        /// <param name="input">The input point to convert, in Map coordinates</param>
        /// <param name="zoom">Zoom level to consider when calculating the World position</param>
        /// <returns>The input point converted to a World position</returns>
        public static Point ConvertToWorldPos(Map map, Point input, int zoom)
        {
            int factor = 1 << (map.Continent.MaximumZoom - zoom);

            double worldPosX = map.ContinentRectangle.X + (input.X - map.MapRectangle.X) * MapToWorldRatio;
            double worldPosY = map.ContinentRectangle.Y + ((map.MapRectangle.Y + map.MapRectangle.Height) - input.Y) * MapToWorldRatio;

            Point scaledOutput = new Point(worldPosX / factor, worldPosY / factor, input.Z);
            return scaledOutput;
        }

        /// <summary>
        /// Converts a point in the Map coordinate system to one in the World coordinate system
        /// </summary>
        /// <param name="map">The map that the input is located in</param>
        /// <param name="input">The input point to convert, in Map coordinates</param>
        /// <returns>The input point converted to a World position</returns>
        public static Point ConvertToWorldPos(Map map, Point input)
        {
            double worldPosX = map.ContinentRectangle.X + (input.X - map.MapRectangle.X) * MapToWorldRatio;
            double worldPosY = map.ContinentRectangle.Y + ((map.MapRectangle.Y + map.MapRectangle.Height) - input.Y) * MapToWorldRatio;

            return new Point(worldPosX, worldPosY, input.Z);
        }
    }
}
