using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Commerce.ViewModels
{
    [Export(typeof(CommerceSettingsViewModel))]
    public class CommerceSettingsViewModel : BindableBase, ISettingsViewModel
    {
        public string SettingsHeader
        {
            get { return "Commerce"; }
        }
    }
}
