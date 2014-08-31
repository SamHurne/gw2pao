using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.TS3.Data
{
    public class ChannelEventArgs : EventArgs
    {
        public uint ClientID { get; private set; }
        public string ClientName { get; private set; }

        public ChannelEventArgs(uint clientId, string clientName)
        {
            this.ClientID = clientId;
            this.ClientName = clientName;
        }
    }
}
