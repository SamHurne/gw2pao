using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.TS3.Data.Enums;

namespace GW2PAO.TS3.Data
{
    public class TalkStatusEventArgs : EventArgs
    {
        public uint ClientID { get; private set; }
        public string ClientName { get; private set; }
        public TalkStatus Status { get; private set; }
        public bool IsPrivate { get; private set; }

        public TalkStatusEventArgs(uint clientId, string clientName, TalkStatus status, bool isPrivate)
        {
            this.ClientID = clientId;
            this.ClientName = clientName;
            this.Status = status;
            this.IsPrivate = isPrivate;
        }
    }
}
