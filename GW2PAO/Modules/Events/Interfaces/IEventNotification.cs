using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.Events.Interfaces
{
    public interface IEventNotification
    {
        /// <summary>
        /// GUID for the event
        /// </summary>
        Guid EventId { get; }

        /// <summary>
        /// Name of the event notification
        /// </summary>
        string EventName { get; }

        /// <summary>
        /// True if the notification for this event has already been shown, else false
        /// </summary>
        bool WasNotificationShown { get; set; }

        /// <summary>
        /// True if the notification for this event is about to be removed, else false
        /// TODO: I hate having this here, but due to a limitation in WPF, there's no reasonable way around this at this time
        /// </summary>
        bool IsRemovingNotification { get; set; }
    }
}
