using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.DayNight.Interfaces
{
    public interface IDayNightViewController
    {
        void Initialize();
        void Shutdown();

        void OpenDayNightTimer();
        bool CanOpenDayNightTimer();
    }
}
