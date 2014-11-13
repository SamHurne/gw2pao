using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.Dungeons.Interfaces
{
    public interface IDungeonsViewController
    {
        void Initialize();
        void Shutdown();

        void DisplayDungeonTracker();
        bool CanDisplayDungeonTracker();
    }
}
