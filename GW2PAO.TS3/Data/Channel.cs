using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.TS3.Data
{
    public class Channel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Channel(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }
    }
}
