using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.API.Data.Entities
{
    public class ZoneItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ZoneItemType Type { get; set; }
        public int MapId { get; set; }
        public string MapName { get; set; }
        public int Level { get; set; }
        public Point ContinentLocation { get; set; }
        public Point Location { get; set; }
        public string ChatCode { get; set; }

        public ZoneItem()
        {
            this.ContinentLocation = new Point();
            this.Location = new Point();
        }

        public override bool Equals(object obj)
        {
            if (obj != null
                && obj is ZoneItem)
            {
                ZoneItem other = obj as ZoneItem;

                return (other.ID == this.ID)
                    && (other.Name == this.Name)
                    && (other.Type == this.Type)
                    && (other.MapId == this.MapId)
                    && (other.MapName == this.MapName)
                    && (other.Level == this.Level)
                    && (other.ContinentLocation.X == this.ContinentLocation.X)
                    && (other.ContinentLocation.Y == this.ContinentLocation.Y)
                    && (other.ContinentLocation.Z == this.ContinentLocation.Z)
                    && (other.Location.X == this.Location.X)
                    && (other.Location.Y == this.Location.Y)
                    && (other.Location.Z == this.Location.Z);
            }
            else
            {
                return false;
            }
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
                hash = hash * 23 + this.Type.GetHashCode();
                hash = hash * 23 + this.MapId.GetHashCode();
                if (this.MapName != null)
                {
                    hash = hash * 23 + this.MapName.GetHashCode();
                }
                if (this.ContinentLocation != null)
                {
                    hash = hash * 23 + this.ContinentLocation.X.GetHashCode();
                    hash = hash * 23 + this.ContinentLocation.Y.GetHashCode();
                    hash = hash * 23 + this.ContinentLocation.Z.GetHashCode();
                }
                if (this.Location != null)
                {
                    hash = hash * 23 + this.Location.X.GetHashCode();
                    hash = hash * 23 + this.Location.Y.GetHashCode();
                    hash = hash * 23 + this.Location.Z.GetHashCode();
                }
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}_{1}", this.ID, this.Name);
        }
    }
}
