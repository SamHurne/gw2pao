using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Modules.ZoneCompletion.Interfaces;
using GW2PAO.PresentationCore;

namespace GW2PAO.Modules.ZoneCompletion.ViewModels
{
    /// <summary>
    /// Zone Name view model
    /// </summary>
    [Export(typeof(IHasZoneName))]
    public class ZoneNameViewModel : NotifyPropertyChangedBase, IHasZoneName
    {
        private string zoneName;

        /// <summary>
        /// Zone name
        /// </summary>
        public string ZoneName
        {
            get { return this.zoneName; }
            set { SetField(ref this.zoneName, value); }
        }
    }
}
