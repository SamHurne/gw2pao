using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.API.Data.Entities
{
    public class Item
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public Uri Icon { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }
        public ItemRarity Rarity { get; set; }
        public int SkinID { get; set; }
        public ItemFlags Flags { get; set; }
        public ItemGameTypes GameTypes { get; set; }
        public int LevelRequirement { get; set; }
        public ItemRestrictions Restrictions { get; set; }
        public int VenderValue { get; set; }
        public ItemPrices Prices { get; set; }
        public string ChatCode { get; set; }

        public Item(int id, string name)
        {
            this.ID = id;
            this.Name = name;
            this.Prices = new ItemPrices();
        }
    }
}
