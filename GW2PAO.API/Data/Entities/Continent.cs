using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Data.Entities
{
    public class Continent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<int> FloorIds { get; set; }

        public double Height { get; set; }
        public double Width { get; set; }

        public int MaxZoom { get; set; }
        public int MinZoom { get; set; }

        public Continent(int id)
        {
            this.Id = id;
        }
    }
}
