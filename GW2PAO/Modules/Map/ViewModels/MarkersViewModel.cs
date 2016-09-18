using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using GW2PAO.Modules.Map.Models;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Map.ViewModels
{
    public class MarkersViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private MapUserData userData;

        /// <summary>
        /// The collection of map markers to show on the map
        /// </summary>
        public ObservableCollection<MapMarkerViewModel> Markers
        {
            get;
            private set;
        }

        /// <summary>
        /// The collection of map markers to show on the map
        /// </summary>
        public ObservableCollection<MapMarkerViewModel> MarkerTemplates
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructs a new MarkersViewModel object
        /// </summary>
        public MarkersViewModel(MapUserData userData)
        {
            this.userData = userData;

            this.InitializeTemplates();

            // Initialize our collection of markers
            this.Markers = new ObservableCollection<MapMarkerViewModel>();
            foreach (var marker in this.userData.MapMarkers)
            {
                this.Markers.Add(new MapMarkerViewModel(marker, this.Markers));
            }

            this.Markers.CollectionChanged += Markers_CollectionChanged;
        }

        private void InitializeTemplates()
        {
            this.MarkerTemplates = new ObservableCollection<MapMarkerViewModel>()
            {
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/miningNode.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/harvestingNode.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/loggingNode.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/activity.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/adventure.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/anvil.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/book.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/parchment.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/dragon.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/greenFlag.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/quaggan.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/trophy.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/pointA.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/pointB.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/pointC.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/orangeShield.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/redShield.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/blueStar.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/greenStar.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/yellowStar.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/yellowStar2.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/downedAlly.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/downedEnemy.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/blueSiege.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/redSiege.png" }, this.Markers),
                new MapMarkerViewModel(new MapMarker() { Icon = @"/Images/Map/Markers/swords.png" }, this.Markers)
            };
        }      

        private void Markers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (MapMarkerViewModel newItem in e.NewItems)
                {
                    newItem.SetContainer(this.Markers);

                    // Reset the template
                    var template = this.MarkerTemplates.FirstOrDefault(m => m == newItem);
                    if (template != null)
                    {
                        var idx = this.MarkerTemplates.IndexOf(template);
                        this.MarkerTemplates.Remove(template);
                        template = new MapMarkerViewModel(new MapMarker() { Icon = newItem.Icon }, null);
                        this.MarkerTemplates.Insert(idx, template);
                    }

                    // Add it to the user data
                    this.userData.MapMarkers.Add(newItem.Model);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (MapMarkerViewModel newItem in e.OldItems)
                {
                    // Remove it from the user data
                    this.userData.MapMarkers.Remove(newItem.Model);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.userData.MapMarkers.Clear();
            }
        }
    }
}
