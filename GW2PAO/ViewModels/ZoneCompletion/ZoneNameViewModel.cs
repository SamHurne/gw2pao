using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.ViewModels.Interfaces;
using GW2PAO.PresentationCore;

namespace GW2PAO.ViewModels.ZoneCompletion
{
    /// <summary>
    /// Zone Name view model
    /// </summary>
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
