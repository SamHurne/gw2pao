using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Data
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Point()
        {
        }

        public Point(double x, double y, double z = 0)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public override string ToString()
        {
            return string.Format("[{0},{1},{2}]", Math.Round(this.X, 3), Math.Round(this.Y, 3), Math.Round(this.Z, 3));
        }
    }
}
