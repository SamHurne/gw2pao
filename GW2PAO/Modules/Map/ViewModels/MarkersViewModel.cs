using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.Modules.Map.Models;
using Microsoft.Practices.Prism.Commands;
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
        public ObservableCollection<MapMarker> Markers
        {
            get;
            private set;
        }

        /// <summary>
        /// The collection of map markers to show on the map
        /// </summary>
        public ObservableCollection<MapMarker> MarkerTemplates
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
            this.MarkerTemplates = new ObservableCollection<MapMarker>()
            {
                new MapMarker(this) { Icon = @"/Images/Map/Markers/miningNode.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/harvestingNode.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/loggingNode.png" },

                new MapMarker(this) { Icon = @"/Images/Map/Markers/activity.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/adventure.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/anvil.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/book.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/parchment.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/dragon.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/greenFlag.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/quaggan.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/trophy.png" },

                new MapMarker(this) { Icon = @"/Images/Map/Markers/pointA.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/pointB.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/pointC.png" },

                new MapMarker(this) { Icon = @"/Images/Map/Markers/orangeShield.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/redShield.png" },

                new MapMarker(this) { Icon = @"/Images/Map/Markers/blueStar.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/greenStar.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/yellowStar.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/yellowStar2.png" },

                new MapMarker(this) { Icon = @"/Images/Map/Markers/downedAlly.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/downedEnemy.png" },

                new MapMarker(this) { Icon = @"/Images/Map/Markers/blueSiege.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/redSiege.png" },
                new MapMarker(this) { Icon = @"/Images/Map/Markers/swords.png" },
            };
        }

        public void RemoveMarker(MapMarker marker)
        {
            this.Markers.Remove(marker);
        }

        private void Markers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (MapMarker newItem in e.NewItems)
                {
                    var template = this.MarkerTemplates.FirstOrDefault(m => m == newItem);
                    if (template != null)
                    {
                        var idx = this.MarkerTemplates.IndexOf(template);
                        this.MarkerTemplates.Remove(template);
                        template = new MapMarker(this) { Icon = newItem.Icon };
                        this.MarkerTemplates.Insert(idx, template);
                    }
                }
            }
        }
    }
}
