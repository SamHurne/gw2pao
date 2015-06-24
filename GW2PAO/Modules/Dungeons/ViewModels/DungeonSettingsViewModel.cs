using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.API.Constants;
using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Modules.Dungeons.Interfaces;
using GW2PAO.Properties;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Dungeons.ViewModels
{
    [Export(typeof(DungeonSettingsViewModel))]
    public class DungeonSettingsViewModel : BindableBase, ISettingsViewModel
    {
        /// <summary>
        /// Dungeons controller
        /// </summary>
        private IDungeonsController controller;

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
        /// Full collection of dungeons data
        /// </summary>
        public ObservableCollection<DungeonViewModel> Dungeons
        {
            get;
            private set;
        }

        /// <summary>
        /// Command to reset all best path completion times
        /// </summary>
        public ICommand ResetAllTimesCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        [ImportingConstructor]
        public DungeonSettingsViewModel(DungeonsUserData userData, IDungeonsController controller)
        {
            this.UserData = userData;
            this.controller = controller;
            this.ResetAllTimesCommand = new DelegateCommand(this.ResetAllBestTimes);

            this.Dungeons = new ObservableCollection<DungeonViewModel>();
            foreach (var dungeon in this.controller.Dungeons)
            {
                // Exclude fractals
                if (dungeon.DungeonId != DungeonID.FractalsOfTheMists)
                    this.Dungeons.Add(dungeon);
            }
        }

        /// <summary>
        /// Resets all completion times for all dungeons
        /// TODO: Do we really want to keep this? Maybe provide a confirm dialog before doing this
        /// </summary>
        private void ResetAllBestTimes()
        {
            foreach (var pcd in this.UserData.PathCompletionData)
            {
                pcd.CompletionTimes.Clear();
            }
        }
    }
}
