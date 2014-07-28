using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.ViewModels;

namespace GW2PAO.Controllers
{
    public class EventNotificationArgs : EventArgs
    {
        public EventViewModel EventData { get; private set; }

        public EventNotificationArgs(EventViewModel eventData)
        {
            this.EventData = eventData;
        }
    }
}
