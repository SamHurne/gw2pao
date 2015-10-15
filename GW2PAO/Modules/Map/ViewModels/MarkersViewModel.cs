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
        /// Template MapMarker for the Mining Marker
        /// </summary>
        public MapMarker MiningMarkerTemplate
        {
            get { return this.miningMarkerTemplate; }
            set { SetProperty(ref this.miningMarkerTemplate, value); }
        }

        /// <summary>
        /// Template MapMarker for the Harvesting Marker
        /// </summary>
        public MapMarker HarvestingMarkerTemplate
        {
            get { return this.harvestingMarkerTemplate; }
            set { SetProperty(ref this.harvestingMarkerTemplate, value); }
        }

        /// <summary>
        /// Template MapMarker for the Logging Marker
        /// </summary>
        public MapMarker LoggingMarkerTemplate
        {
            get { return this.loggingMarkerTemplate; }
            set { SetProperty(ref this.loggingMarkerTemplate, value); }
        }

        /// <summary>
        /// Constructs a new MarkersViewModel object
        /// </summary>
        public MarkersViewModel(MapUserData userData)
        {
            this.Markers = new ObservableCollection<MapMarker>();
            this.userData = userData;

            this.MiningMarkerTemplate = new MapMarker()
            {
                Icon = @"/Images/Map/Markers/miningNode.png"
            };

            this.HarvestingMarkerTemplate = new MapMarker()
            {
                Icon = @"/Images/Map/Markers/harvestingNode.png"
            };

            this.LoggingMarkerTemplate = new MapMarker()
            {
                Icon = @"/Images/Map/Markers/loggingNode.png"
            };

            this.Markers.CollectionChanged += Markers_CollectionChanged;
        }

        private void Markers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems)
                {
                    if (newItem == this.MiningMarkerTemplate)
                    {
                        this.MiningMarkerTemplate = new MapMarker()
                        {
                            Icon = @"/Images/Map/Markers/miningNode.png"
                        };
                    }
                    else if (newItem == this.HarvestingMarkerTemplate)
                    {
                        this.HarvestingMarkerTemplate = new MapMarker()
                        {
                            Icon = @"/Images/Map/Markers/harvestingNode.png"
                        };
                    }
                    else if (newItem == this.LoggingMarkerTemplate)
                    {
                        this.LoggingMarkerTemplate = new MapMarker()
                        {
                            Icon = @"/Images/Map/Markers/loggingNode.png"
                        };
                    }
                }
            }
        }
    }
}
