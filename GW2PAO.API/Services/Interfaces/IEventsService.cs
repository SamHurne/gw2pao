using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.API.Services.Interfaces
{
    public interface IEventsService
    {
        MegaserverEventTimeTable EventTimeTable { get; }
        void LoadTable();
        Data.Enums.EventState GetState(Guid id);
        Data.Enums.EventState GetState(WorldEvent evt);
        TimeSpan GetTimeUntilActive(WorldEvent evt);
        TimeSpan GetTimeSinceActive(WorldEvent evt);
    }
}
