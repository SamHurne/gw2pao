using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapControl;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Map.Models
{
    [Serializable]
    public class PolyLine : BindableBase
    {
        private ObservableCollection<Location> locations = new ObservableCollection<Location>();
        public ObservableCollection<Location> Locations
        {
            get { return this.locations; }
        }
    }

    [Serializable]
    public class Drawing : BindableBase
    {
        private Guid id;
        private string name;
        private int continentId;
        private string color;
        private ObservableCollection<PolyLine> polylines = new ObservableCollection<PolyLine>();

        /// <summary>
        /// Unique ID of the drawing
        /// </summary>
        public Guid ID
        {
            get { return this.id; }
            set { SetProperty(ref this.id, value); }
        }

        /// <summary>
        /// Name of the drawing
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { SetProperty(ref this.name, value); }
        }

        /// <summary>
        /// The ID of the continent the drawing was made for
        /// </summary>
        public int ContinentId
        {
            get { return this.continentId; }
            set { SetProperty(ref this.continentId, value); }
        }

        /// <summary>
        /// The Color of the drawing
        /// </summary>
        public string Color
        {
            get { return this.color; }
            set { SetProperty(ref this.color, value); }
        }

        /// <summary>
        /// Collection of polylines making up the drawing
        /// </summary>
        public ObservableCollection<PolyLine> Polylines
        {
            get { return this.polylines; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Drawing()
        {
            this.ID = Guid.NewGuid();
            this.Color = "#000000";
        }
    }
}
