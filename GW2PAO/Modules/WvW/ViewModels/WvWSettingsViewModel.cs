using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.WvW.ViewModels
{
    [Export(typeof(WvWSettingsViewModel))]
    public class WvWSettingsViewModel : BindableBase, ISettingsViewModel
    {
        public string SettingsHeader
        {
            get { return Properties.Resources.WorldvsWorld; }
        }

        /// <summary>
        /// The event user data
        /// </summary>
        public WvWUserData UserData
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of possible NA worlds
        /// </summary>
        public ObservableCollection<World> PossibleWorlds
        {
            get;
            private set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="userData">The WvW user data</param>
        [ImportingConstructor]
        public WvWSettingsViewModel(WvWUserData userData, IWvWService wvwService)
        {
            this.UserData = userData;
            this.PossibleWorlds = new ObservableCollection<World>();

            if (wvwService.Worlds == null)
                wvwService.LoadData();
            foreach (var world in wvwService.Worlds.OrderBy(wld => wld.Name))
            {
                this.PossibleWorlds.Add(world);
            }
        }
    }
}
