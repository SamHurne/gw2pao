using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.TS3.Data
{
    public class NewServerInfoEventArgs : EventArgs
    {
        public string ServerName { get; private set; }
        public string ServerAddress { get; private set; }

        public NewServerInfoEventArgs(string serverName, string serverAddress)
        {
            this.ServerName = serverName;
            this.ServerAddress = serverAddress;
        }
    }
}
