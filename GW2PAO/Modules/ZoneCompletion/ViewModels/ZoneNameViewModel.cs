using GW2PAO.Modules.ZoneCompletion.Interfaces;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.ZoneCompletion.ViewModels
{
    /// <summary>
    /// Zone Name view model
    /// </summary>
    [Export(typeof(IHasZoneName))]
    public class ZoneNameViewModel : BindableBase, IHasZoneName
    {
        private string zoneName;

        /// <summary>
        /// Zone name
        /// </summary>
        public string ZoneName
        {
            get { return this.zoneName; }
            set { SetProperty(ref this.zoneName, value); }
        }
    }
}
