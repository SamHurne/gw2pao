using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Enums;
using GW2PAO.PresentationCore;
using GW2PAO.ViewModels.Interfaces;

namespace GW2PAO.ViewModels.WvW.WvWTracker
{
    /// <summary>
    /// WvWMap view model
    /// </summary>
    public class WvWMapViewModel : NotifyPropertyChangedBase, IHasWvWMap
    {
        private WvWMap map;

        /// <summary>
        /// WvW Map
        /// </summary>
        public WvWMap Map
        {
            get { return this.map; }
            set { SetField(ref this.map, value); }
        }
    }
}
