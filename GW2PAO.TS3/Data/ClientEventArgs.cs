using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.TS3.Data
{
    public class ClientEventArgs : EventArgs
    {
        public uint ClientID { get; private set; }
        public string ClientName { get; private set; }

        public ClientEventArgs(uint clientId, string clientName)
        {
            this.ClientID = clientId;
            this.ClientName = clientName;
        }
    }
}
