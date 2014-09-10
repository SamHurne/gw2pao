using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2DotNET.Entities.Maps;
using GW2PAO.API.Data;

namespace GW2PAO.API.Util
{
    public static class MapsHelper
    {
        public static int PixelToWorldPosRatio = 24;
        public static double WorldPosToPixelRatio = 1.0 / PixelToWorldPosRatio;

        public static Point PixelToWorldPos(Map map, Point input, int zoom)
        {
            int factor = 1 << (map.Continent.MaximumZoom - zoom);
            Point scaledInput = new Point(input.X * factor, input.Y * factor);
            double worldPosX = (scaledInput.X - map.ContinentRectangle.X) * PixelToWorldPosRatio + map.MapRectangle.X;
            double worldPosY = (map.MapRectangle.Y + map.MapRectangle.Height) - (scaledInput.Y - map.ContinentRectangle.Y) * PixelToWorldPosRatio;
            return new Point(worldPosX, worldPosY);
        }
    }
}
