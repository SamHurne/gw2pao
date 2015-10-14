using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Entities;
using GW2PAO.Modules.ZoneCompletion.Models;
using GW2PAO.Modules.ZoneCompletion.ViewModels;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Map.ViewModels
{
    public class ZoneItemsViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private ZoneItemsStore zoneItemsStore;
        private int continentId;

        // TODO: Consider moving these to the UserData class
        private bool showWaypoints;
        private bool showPOIs;
        private bool showVistas;
        private bool showHeartQuests;
        private bool showHeroPoints;
        private bool showDungeons;

        /// <summary>
        /// Continent ID of the continent for which to show zone items
        /// </summary>
        public int ContinentID
        {
            get { return this.continentId; }
            set { SetProperty(ref this.continentId, value); }
        }

        /// <summary>
        /// Collection of Waypoints for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> Waypoints
        {
            get { return this.zoneItemsStore.Data[this.continentId].Waypoints; }
        }

        /// <summary>
        /// True if Waypoints should be shown on the map, else false
        /// </summary>
        public bool ShowWaypoints
        {
            get { return this.showWaypoints; }
            set { SetProperty(ref this.showWaypoints, value); }
        }

        /// <summary>
        /// Collection of Points of Interest for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> POIs
        {
            get { return this.zoneItemsStore.Data[this.continentId].POIs; }
        }

        /// <summary>
        /// True if POIs should be shown on the map, else false
        /// </summary>
        public bool ShowPOIs
        {
            get { return this.showPOIs; }
            set { SetProperty(ref this.showPOIs, value); }
        }

        /// <summary>
        /// Collection of Vistas for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> Vistas
        {
            get { return this.zoneItemsStore.Data[this.continentId].Vistas; }
        }

        /// <summary>
        /// True if Vistas should be shown on the map, else false
        /// </summary>
        public bool ShowVistas
        {
            get { return this.showVistas; }
            set { SetProperty(ref this.showVistas, value); }
        }

        /// <summary>
        /// Collection of Heart Quests for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> HeartQuests
        {
            get { return this.zoneItemsStore.Data[this.continentId].HeartQuests; }
        }

        /// <summary>
        /// True if Heart Quests should be shown on the map, else false
        /// </summary>
        public bool ShowHeartQuests
        {
            get { return this.showHeartQuests; }
            set { SetProperty(ref this.showHeartQuests, value); }
        }

        /// <summary>
        /// Collection of Hero Points for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> HeroPoints
        {
            get { return this.zoneItemsStore.Data[this.continentId].HeroPoints; }
        }

        /// <summary>
        /// True if Hero Points should be shown on the map, else false
        /// </summary>
        public bool ShowHeroPoints
        {
            get { return this.showHeroPoints; }
            set { SetProperty(ref this.showHeroPoints, value); }
        }

        /// <summary>
        /// Collection of Dungeons for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> Dungeons
        {
            get { return this.zoneItemsStore.Data[this.continentId].Dungeons; }
        }

        /// <summary>
        /// True if Dungeons should be shown on the map, else false
        /// </summary>
        public bool ShowDungeons
        {
            get { return this.showDungeons; }
            set { SetProperty(ref this.showDungeons, value); }
        }

        /// <summary>
        /// Constructs a new ZoneItemsViewModel object
        /// </summary>
        public ZoneItemsViewModel(ZoneItemsStore zoneItemsStore)
        {
            this.zoneItemsStore = zoneItemsStore;

            this.ShowHeartQuests = true;
            this.ShowHeroPoints = true;
            this.ShowPOIs = true;
            this.ShowVistas = true;
            this.ShowWaypoints = true;
            this.ShowDungeons = true;
        }
    }
}
