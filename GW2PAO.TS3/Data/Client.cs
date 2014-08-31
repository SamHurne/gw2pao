using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.TS3.Data
{
    public class Client
    {
        public uint ID { get; private set; }
        public string Name { get; set; }
        public uint ChannelID { get; set; }

        public Client(uint id)
        {
            this.ID = id;
        }

        public Client(uint id, string name, uint channelID)
        {
            this.ID = id;
            this.Name = name;
            this.ChannelID = channelID;
        }

        public override bool Equals(object obj)
        {
            if (obj != null
                && obj is Client)
            {
                Client other = obj as Client;

                return other.ID == this.ID;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }
    }
}
