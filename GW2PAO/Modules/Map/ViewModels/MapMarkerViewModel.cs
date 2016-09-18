using System.Collections;
using System.Windows.Input;
using GW2PAO.Modules.Map.Interfaces;
using GW2PAO.Modules.Map.Models;
using GW2PAO.PresentationCore;
using MapControl;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Map.ViewModels
{
    public class MapMarkerViewModel : BindableBase, IHasMapLocation
    {
        private IList markersContainer;

        public MapMarker Model
        {
            get;
            private set;
        }

        /// <summary>
        /// Path to the icon image for the marker
        /// This can be either a path to an embedded image, or the path to an image on the web
        /// </summary>
        public string Icon
        {
            get { return this.Model.Icon; }
            set { SetProperty(ref this.Model.Icon, value); }
        }

        /// <summary>
        /// Name or Description to show for the marker
        /// </summary>
        public string Name
        {
            get { return this.Model.Name; }
            set { SetProperty(ref this.Model.Name, value); }
        }

        /// <summary>
        /// Location of the MapMarker on the Map
        /// </summary>
        public Location Location
        {
            get { return this.Model.Location; }
            set { SetProperty(ref this.Model.Location, value); }
        }

        /// <summary>
        /// Command to remove the map marker
        /// </summary>
        public ICommand RemoveMarkerCommand { get; private set; }

        /// <summary>
        /// Constructs a new MapMarker object with a parent view model
        /// </summary>
        /// <param name="parentViewModel">The parent view model of the marker</param>
        /// <remarks>
        /// I hate having this set up like this, but there was no other way
        ///  that I could make the Remove functionality work property
        /// </remarks>
        public MapMarkerViewModel(MapMarker marker, IList container)
        {
            this.markersContainer = container;
            this.Model = marker;
            this.RemoveMarkerCommand = new DelegateCommand(this.RemoveMarker);
        }

        /// <summary>
        /// Sets the map marker's container object
        /// </summary>
        /// <param name="container">The container to set</param>
        public void SetContainer(IList container)
        {
            this.markersContainer = container;
        }

        /// <summary>
        /// Removes the map marker
        /// </summary>
        public void RemoveMarker()
        {
            if (this.markersContainer != null)
                this.markersContainer.Remove(this);
        }
    }
}
