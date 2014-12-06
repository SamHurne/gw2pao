using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Dungeons.ViewModels
{
    [Export(typeof(DungeonSettingsViewModel))]
    public class DungeonSettingsViewModel : BindableBase, ISettingsViewModel
    {
        public string SettingsHeader
        {
            get { return "Dungeons"; }
        }
    }
}
