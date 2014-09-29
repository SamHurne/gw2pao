using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Data
{
    public class Guild
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }

        public Guild(Guid id)
        {
            this.ID = id;
        }
    }
}
