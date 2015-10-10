using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2NET.Maps;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;
using Map = GW2NET.Maps.Map;

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
        public static Point ConvertToMapPos(Rectangle continentRectangle, Rectangle mapRectangle, Point input, int zoom, int maxZoom)
        {
            int factor = 1 << (maxZoom - zoom);
            Point scaledInput = new Point(input.X * factor, input.Y * factor);

            double mapPosX = (scaledInput.X - continentRectangle.X) * WorldToMapRatio + mapRectangle.X;
            double mapPosY = (mapRectangle.Y + mapRectangle.Height) - (scaledInput.Y - continentRectangle.Y) * WorldToMapRatio;

            return new Point(mapPosX, mapPosY, input.Z);
        }

        /// <summary>
        /// Converts a point in the World coordinate system to one in the Maps coordinate system
        /// </summary>
        /// <param name="map">The map that the input is located in</param>
        /// <param name="input">The input point to convert, in World coordinates</param>
        /// <returns>The input point converted to a Map position</returns>
        public static Point ConvertToMapPos(Rectangle continentRectangle, Rectangle mapRectangle, Point input)
        {
            double mapPosX = (input.X - continentRectangle.X) * WorldToMapRatio + mapRectangle.X;
            double mapPosY = (mapRectangle.Y + mapRectangle.Height) - (input.Y - continentRectangle.Y) * WorldToMapRatio;

            return new Point(mapPosX, mapPosY, input.Z);
        }

        /// <summary>
        /// Converts a point in the Map coordinate system to one in the World coordinate system
        /// </summary>
        /// <param name="map">The map that the input is located in</param>
        /// <param name="input">The input point to convert, in Map coordinates</param>
        /// <param name="zoom">Zoom level to consider when calculating the World position</param>
        /// <returns>The input point converted to a World position</returns>
        public static Point ConvertToWorldPos(Rectangle continentRectangle, Rectangle mapRectangle, Point input, int zoom, int maxZoom)
        {
            int factor = 1 << (maxZoom - zoom);

            double worldPosX = continentRectangle.X + (input.X - mapRectangle.X) * MapToWorldRatio;
            double worldPosY = continentRectangle.Y + ((mapRectangle.Y + mapRectangle.Height) - input.Y) * MapToWorldRatio;

            Point scaledOutput = new Point(worldPosX / factor, worldPosY / factor, input.Z);
            return scaledOutput;
        }

        /// <summary>
        /// Converts a point in the Map coordinate system to one in the World coordinate system
        /// </summary>
        /// <param name="map">The map that the input is located in</param>
        /// <param name="input">The input point to convert, in Map coordinates</param>
        /// <returns>The input point converted to a World position</returns>
        public static Point ConvertToWorldPos(Rectangle continentRectangle, Rectangle mapRectangle, Point input)
        {
            double worldPosX = continentRectangle.X + (input.X - mapRectangle.X) * MapToWorldRatio;
            double worldPosY = continentRectangle.Y + ((mapRectangle.Y + mapRectangle.Height) - input.Y) * MapToWorldRatio;

            return new Point(worldPosX, worldPosY, input.Z);
        }
    }
}
