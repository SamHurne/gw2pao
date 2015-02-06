using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.PubSubEvents;

namespace GW2PAO.Infrastructure
{
    public class InsufficientPrivilegesEvent : PubSubEvent<object> { }

    public class GW2ProcessStarted : PubSubEvent<object> { }

    public class GW2ProcessFocused : PubSubEvent<object> { }

    public class GW2ProcessLostFocus : PubSubEvent<object> { }

    public class GW2ProcessClosed : PubSubEvent<object> { }

    public class PlayerEnteredPvE : PubSubEvent<object> { }

    public class PlayerEnteredWvW : PubSubEvent<object> { }
}
