using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.TS3.Data
{
    public class Channel
    {
        public uint ID { get; set; }
        public uint ParentID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public uint Order { get; set; }
        public uint ClientsCount { get; set; }
        public bool IsSpacer { get; set; }

        public Channel(uint id, string name, bool isSpacer = false)
        {
            this.ID = id;
            this.Name = name;
            this.IsSpacer = isSpacer;
        }
    }
}
