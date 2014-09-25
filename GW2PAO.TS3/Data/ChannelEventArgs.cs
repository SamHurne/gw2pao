using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.TS3.Data
{
    public class ChannelEventArgs : EventArgs
    {
        public Channel Channel { get; private set; }

        public ChannelEventArgs(Channel channel)
        {
            this.Channel = channel;
        }
    }
}
