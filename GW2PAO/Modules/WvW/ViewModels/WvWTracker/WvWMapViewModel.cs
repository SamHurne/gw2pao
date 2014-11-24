using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Enums;
using GW2PAO.Modules.WvW.Interfaces;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.WvW.ViewModels.WvWTracker
{
    /// <summary>
    /// WvWMap view model
    /// </summary>
    [Export(typeof(IHasWvWMap))]
    public class WvWMapViewModel : BindableBase, IHasWvWMap
    {
        private WvWMap map;

        /// <summary>
        /// WvW Map
        /// </summary>
        public WvWMap Map
        {
            get { return this.map; }
            set { SetProperty(ref this.map, value); }
        }
    }
}
