using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.ZoneCompletion.Interfaces
{
    public interface IZoneCompletionViewController
    {
        void Initialize();
        void Shutdown();

        void DisplayZoneCompletionAssistant();
        bool CanDisplayZoneCompletionAssistant();
    }
}
