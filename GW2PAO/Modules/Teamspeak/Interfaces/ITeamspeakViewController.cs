using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.Teamspeak.Interfaces
{
    public interface ITeamspeakViewController
    {
        void Initialize();
        void Shutdown();

        void DisplayTeamspeakOverlay();
        bool CanDisplayTeamspeakOverlay();
    }
}
