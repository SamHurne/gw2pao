using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Data.Entities
{
    public class Map
    {
        public int Id { get; private set; }
        
        public Rectangle MapRectangle { get; set; }
        public Rectangle ContinentRectangle { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }

        public int DefaultFloor { get; set; }

        public int RegionId { get; set; }
        public string RegionName { get; set; }

        public int ContinentId { get; set; }

        public Map(int id)
        {
            this.Id = id;
            this.MapRectangle = new Rectangle();
            this.ContinentRectangle = new Rectangle();
        }
    }
}
