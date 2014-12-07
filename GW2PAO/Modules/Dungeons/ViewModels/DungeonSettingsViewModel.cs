using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Properties;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Dungeons.ViewModels
{
    [Export(typeof(DungeonSettingsViewModel))]
    public class DungeonSettingsViewModel : BindableBase, ISettingsViewModel
    {
        /// <summary>
        /// Settings header
        /// </summary>
        public string SettingsHeader
        {
            get { return Resources.Dungeons; }
        }

        /// <summary>
        /// The dungeons user data/settings
        /// </summary>
        public DungeonsUserData UserData
        {
            get;
            private set;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        [ImportingConstructor]
        public DungeonSettingsViewModel(DungeonsUserData userData)
        {
            this.UserData = userData;
        }
    }
}
