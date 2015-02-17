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

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + this.ID.GetHashCode();
                if (this.Name != null)
                {
                    hash = hash * 23 + this.Name.GetHashCode();
                }
                return hash;
            }
        }
    }
}
