using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.Map.ViewModels
{
    [Export(typeof(MapViewModel))]
    public class MapViewModel : BindableBase
    {
        private int continentId;
        private int floorId;

        public int ContinentId
        {
            get { return this.continentId; }
            set
            {
                if (SetProperty(ref this.continentId, value))
                {
                    this.OnPropertyChanged(() => this.MapTileSourceString);
                }
            }
        }

        public int FloorId
        {
            get { return this.floorId; }
            set
            {
                if (SetProperty(ref this.floorId, value))
                {
                    this.OnPropertyChanged(() => this.MapTileSourceString);
                }
            }
        }

        public string MapTileSourceString
        {
            get
            {
                return string.Format("https://tiles.guildwars2.com/{0}/{1}/{2}.jpg", this.ContinentId, this.FloorId, "{z}/{x}/{y}");
            }
        }

        public MapViewModel()
        {
            this.ContinentId = 1;
            this.FloorId = 1;
        }
    }
}
