using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.TS3.Data
{
    public class TextMessageEventArgs : EventArgs
    {
        public uint ClientID { get; private set; }
        public string ClientName { get; private set; }
        public string Message { get; private set; }

        public TextMessageEventArgs(uint clientId, string clientName, string message)
        {
            this.ClientID = clientId;
            this.ClientName = clientName;
            this.Message = message;
        }
    }
}
