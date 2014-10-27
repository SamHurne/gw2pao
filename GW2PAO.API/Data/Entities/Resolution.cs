using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Data.Entities
{
    public class Resolution
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public override string ToString()
        {
            return this.Width + "x" + this.Height;
        }
    }
}
