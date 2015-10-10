using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.Map.Interfaces
{
    public interface IMapViewController
    {
        void Initialize();
        void Shutdown();

        void OpenMap();
        bool CanOpenMap();
    }
}
