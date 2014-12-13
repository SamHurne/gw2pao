using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Data.Entities
{
    public class World
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as World;
            if (other == null)
                return false;

            return other.ID == this.ID
                && other.Name == this.Name;
        }
    }
}
