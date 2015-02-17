using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Infrastructure.Hotkeys.Interfaces
{
    public interface IHotkeyManager
    {
        bool CanRegister(Hotkey hotkey);
        bool IsRegistered(Hotkey hotkey);
        void PauseHotkeys();
        bool Register(Hotkey hotkey);
        void ResumeHotkeys();
        bool Unregister(Hotkey hotkey);
    }
}
