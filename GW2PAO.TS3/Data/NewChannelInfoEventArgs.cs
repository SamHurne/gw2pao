using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.TS3.Data
{
    public class NewChannelInfoEventArgs
    {
        public Channel NewChannel { get; private set; }

        public NewChannelInfoEventArgs(Channel newChannel)
        {
            this.NewChannel = newChannel;
        }
    }
}
