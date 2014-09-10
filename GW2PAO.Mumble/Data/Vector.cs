using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Mumble.Data
{
    public struct Vector
    {
        /// <summary>
        /// X-Axis Value
        /// </summary>
        public double X;

        /// <summary>
        /// Y-Axis Value
        /// </summary>
        public double Y;

        /// <summary>
        /// Z-Axis Value
        /// </summary>
        public double Z;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="X">X-Axis value</param>
        /// <param name="Y">Y-Axis value</param>
        /// <param name="Z">Z-Axis value</param>
        public Vector(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public override string ToString()
        {
            return string.Format("[{0},{1},{2}]", this.X, this.Y, this.Z);
        }
    }
}
