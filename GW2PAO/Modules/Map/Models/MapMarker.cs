using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.Modules.Map.Interfaces;
using MapControl;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Map.Models
{
    public class MapMarker : BindableBase, IHasMapLocation
    {
        private ViewModels.MarkersViewModel parentViewModel;
        private string icon;
        private string name;
        private Location location;

        /// <summary>
        /// Path to the icon image for the marker
        /// This can be either a path to an embedded image, or the path to an image on the web
        /// </summary>
        public string Icon
        {
            get { return this.icon; }
            set { SetProperty(ref this.icon, value); }
        }

        /// <summary>
        /// Name or Description to show for the marker
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { SetProperty(ref this.name, value); }
        }

        /// <summary>
        /// Location of the MapMarker on the Map
        /// </summary>
        public Location Location
        {
            get { return this.location; }
            set { SetProperty(ref this.location, value); }
        }

        /// <summary>
        /// Command to remove a map marker from the Markers collection
        /// Expects a MapMarker input parameter
        /// </summary>
        public ICommand RemoveMarkerCommand { get; private set; }

        /// <summary>
        /// Constructs a new MapMarker object
        /// </summary>
        public MapMarker()
        {
            this.RemoveMarkerCommand = new DelegateCommand(() =>
            {
                if (this.parentViewModel != null)
                    this.parentViewModel.RemoveMarker(this);
            });
        }

        /// <summary>
        /// Constructs a new MapMarker object with a parent view model
        /// </summary>
        /// <param name="parentViewModel">The parent view model of the marker</param>
        /// <remarks>
        /// I hate having this set up like this, but there was no other way
        ///  that I could make the Remove functionality work property
        /// </remarks>
        public MapMarker(ViewModels.MarkersViewModel parentViewModel)
            : this()
        {
            this.parentViewModel = parentViewModel;
        }
    }
}
