using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.Events.Interfaces
{
    public interface IEventsViewController
    {
        void Initialize();
        void Shutdown();

        void DisplayWorldBossTimers();
        bool CanDisplayWorldBossTimers();

        void DisplayMetaEventTimers();
        bool CanDisplayMetaEventTimers();

        void DisplayEventNotificationsWindow();
        bool CanDisplayEventNotificationsWindow();
    }
}
