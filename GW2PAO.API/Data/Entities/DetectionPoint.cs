using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Data.Entities
{
    /// <summary>
    /// Same as a <cref="Point">, but includes a radius
    /// </summary>
    public class DetectionPoint : Point
    {
        public double Radius { get; set; }

        public DetectionPoint()
        {
        }

        public DetectionPoint(double x, double y, double z = 0, double radius = 0)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Radius = radius;
        }

        public override string ToString()
        {
            return string.Format("[{0},{1},{2}] {3} Radius", Math.Round(this.X, 3), Math.Round(this.Y, 3), Math.Round(this.Z, 3), Math.Round(this.Radius, 3));
        }
    }
}
