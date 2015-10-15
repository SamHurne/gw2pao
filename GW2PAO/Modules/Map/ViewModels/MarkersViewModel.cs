using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private MapMarker miningMarkerTemplate;
        private MapMarker harvestingMarkerTemplate;
        private MapMarker loggingMarkerTemplate;

        /// <summary>
        /// The collection of map markers to show on the map
        /// </summary>
        public ObservableCollection<MapMarker> Markers
        {
            get;
            private set;
        }

        /// <summary>
        /// The collection of map markers to show on the map
        /// </summary>
        public List<MapMarker> MarkerTemplates
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructs a new MarkersViewModel object
        /// </summary>
        public MarkersViewModel(MapUserData userData)
        {
            this.Markers = new ObservableCollection<MapMarker>();
            this.userData = userData;

            this.InitializeTemplates();

            this.Markers.CollectionChanged += Markers_CollectionChanged;
        }

        private void InitializeTemplates()
        {
            this.MarkerTemplates = new List<MapMarker>()
            {
                new MapMarker() { Icon = @"/Images/Map/Markers/miningNode.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/harvestingNode.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/loggingNode.png" },

                new MapMarker() { Icon = @"/Images/Map/Markers/activity.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/adventure.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/anvil.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/book.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/parchment.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/dragon.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/greenFlag.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/quaggan.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/trophy.png" },

                new MapMarker() { Icon = @"/Images/Map/Markers/pointA.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/pointB.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/pointC.png" },

                new MapMarker() { Icon = @"/Images/Map/Markers/orangeShield.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/redShield.png" },

                new MapMarker() { Icon = @"/Images/Map/Markers/blueStar.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/greenStar.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/yellowStar.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/yellowStar2.png" },

                new MapMarker() { Icon = @"/Images/Map/Markers/downedAlly.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/downedEnemy.png" },

                new MapMarker() { Icon = @"/Images/Map/Markers/blueSiege.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/redSiege.png" },
                new MapMarker() { Icon = @"/Images/Map/Markers/swords.png" },
            };
        }

        private void Markers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (MapMarker newItem in e.NewItems)
                {
                    var template = this.MarkerTemplates.FirstOrDefault(m => m == newItem);
                    if (template != null)
                        template = new MapMarker() { Icon = newItem.Icon };
                }
            }
        }
    }
}
