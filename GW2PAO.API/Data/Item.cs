using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Data
{
    public class Item
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public Uri Icon { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Rarity { get; set; }
        public int LevelRequirement { get; set; }
        public int VenderValue { get; set; }
        public ItemPrices Prices { get; set; }

        public Item(int id, string name)
        {
            this.ID = id;
            this.Name = name;
            this.Prices = new ItemPrices();
        }
    }
}
