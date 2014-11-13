using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.WvW.Interfaces
{
    public interface IWvWViewController
    {
        void Initialize();
        void Shutdown();

        void DisplayWvWTracker();
        bool CanDisplayWvWTracker();

        void DisplayWvWNotificationsWindow();
        bool CanDisplayWvWNotificationsWindow();
    }
}
