using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.ViewModels
{
    [Export(typeof(HotkeySettingsViewModel))]
    public class HotkeySettingsViewModel : BindableBase, ISettingsViewModel
    {
        public string SettingsHeader
        {
            get { return "Hotkeys"; }
        }
    }
}
