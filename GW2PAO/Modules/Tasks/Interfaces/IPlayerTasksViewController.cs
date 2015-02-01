using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.Tasks.Interfaces
{
    public interface IPlayerTasksViewController
    {
        void Initialize();
        void Shutdown();

        void DisplayTaskTracker();
        bool CanDisplayTaskTracker();
    }
}
