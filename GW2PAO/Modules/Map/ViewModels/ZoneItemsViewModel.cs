using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Modules.ZoneCompletion.Interfaces;
using GW2PAO.Modules.ZoneCompletion.Models;
using GW2PAO.Modules.ZoneCompletion.ViewModels;
using GW2PAO.Utility;
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
        private MapUserData userData;
        private IZoneCompletionController zoneController;
        private int continentId;

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
            get
            {
                if (this.userData.ShowEntireContinent)
                {
                    return this.zoneItemsStore.Data[this.ContinentID].Waypoints;
                }
                else if (this.zoneController.ValidMapID && this.zoneItemsStore.Data[this.ContinentID].ZoneItemsByMap.ContainsKey(this.zoneController.CurrentMapID))
                {
                    return this.zoneItemsStore.Data[this.ContinentID].ZoneItemsByMap[this.zoneController.CurrentMapID].Waypoints;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// True if Waypoints should be shown on the map, else false
        /// </summary>
        public bool ShowWaypoints
        {
            get { return this.userData.AreWaypointsVisible; }
            set
            {
                if (this.userData.AreWaypointsVisible != value)
                {
                    this.userData.AreWaypointsVisible = value;
                    this.OnPropertyChanged(() => this.ShowWaypoints);
                }
            }
        }

        /// <summary>
        /// Collection of Points of Interest for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> POIs
        {
            get
            {
                if (this.userData.ShowEntireContinent)
                {
                    return this.zoneItemsStore.Data[this.ContinentID].POIs;
                }
                else if (this.zoneController.ValidMapID && this.zoneItemsStore.Data[this.ContinentID].ZoneItemsByMap.ContainsKey(this.zoneController.CurrentMapID))
                {
                    return this.zoneItemsStore.Data[this.ContinentID].ZoneItemsByMap[this.zoneController.CurrentMapID].POIs;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// True if POIs should be shown on the map, else false
        /// </summary>
        public bool ShowPOIs
        {
            get { return this.userData.ArePoisVisible; }
            set
            {
                if (this.userData.ArePoisVisible != value)
                {
                    this.userData.ArePoisVisible = value;
                    this.OnPropertyChanged(() => this.ShowPOIs);
                }
            }
        }

        /// <summary>
        /// Collection of Vistas for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> Vistas
        {
            get
            {
                if (this.userData.ShowEntireContinent)
                {
                    return this.zoneItemsStore.Data[this.ContinentID].Vistas;
                }
                else if (this.zoneController.ValidMapID && this.zoneItemsStore.Data[this.ContinentID].ZoneItemsByMap.ContainsKey(this.zoneController.CurrentMapID))
                {
                    return this.zoneItemsStore.Data[this.ContinentID].ZoneItemsByMap[this.zoneController.CurrentMapID].Vistas;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// True if Vistas should be shown on the map, else false
        /// </summary>
        public bool ShowVistas
        {
            get { return this.userData.AreVistasVisible; }
            set
            {
                if (this.userData.AreVistasVisible != value)
                {
                    this.userData.AreVistasVisible = value;
                    this.OnPropertyChanged(() => this.ShowVistas);
                }
            }
        }

        /// <summary>
        /// Collection of Heart Quests for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> HeartQuests
        {
            get
            {
                if (this.userData.ShowEntireContinent)
                {
                    return this.zoneItemsStore.Data[this.ContinentID].HeartQuests;
                }
                else if (this.zoneController.ValidMapID && this.zoneItemsStore.Data[this.ContinentID].ZoneItemsByMap.ContainsKey(this.zoneController.CurrentMapID))
                {
                    return this.zoneItemsStore.Data[this.ContinentID].ZoneItemsByMap[this.zoneController.CurrentMapID].HeartQuests;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// True if Heart Quests should be shown on the map, else false
        /// </summary>
        public bool ShowHeartQuests
        {
            get { return this.userData.AreHeartsVisible; }
            set
            {
                if (this.userData.AreHeartsVisible != value)
                {
                    this.userData.AreHeartsVisible = value;
                    this.OnPropertyChanged(() => this.ShowHeartQuests);
                }
            }
        }

        /// <summary>
        /// Collection of Hero Points for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> HeroPoints
        {
            get
            {
                if (this.userData.ShowEntireContinent)
                {
                    return this.zoneItemsStore.Data[this.ContinentID].HeroPoints;
                }
                else if (this.zoneController.ValidMapID && this.zoneItemsStore.Data[this.ContinentID].ZoneItemsByMap.ContainsKey(this.zoneController.CurrentMapID))
                {
                    return this.zoneItemsStore.Data[this.ContinentID].ZoneItemsByMap[this.zoneController.CurrentMapID].HeroPoints;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// True if Hero Points should be shown on the map, else false
        /// </summary>
        public bool ShowHeroPoints
        {
            get { return this.userData.AreHeroPointsVisible; }
            set
            {
                if (this.userData.AreHeroPointsVisible != value)
                {
                    this.userData.AreHeroPointsVisible = value;
                    this.OnPropertyChanged(() => this.ShowHeroPoints);
                }
            }
        }

        /// <summary>
        /// Collection of Dungeons for the current continent
        /// </summary>
        public ObservableCollection<ZoneItemViewModel> Dungeons
        {
            get
            {
                if (this.userData.ShowEntireContinent)
                {
                    return this.zoneItemsStore.Data[this.ContinentID].Dungeons;
                }
                else if (this.zoneController.ValidMapID && this.zoneItemsStore.Data[this.ContinentID].ZoneItemsByMap.ContainsKey(this.zoneController.CurrentMapID))
                {
                    return this.zoneItemsStore.Data[this.ContinentID].ZoneItemsByMap[this.zoneController.CurrentMapID].Dungeons;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// True if Dungeons should be shown on the map, else false
        /// </summary>
        public bool ShowDungeons
        {
            get { return this.userData.AreDungeonsVisible; }
            set
            {
                if (this.userData.AreDungeonsVisible != value)
                {
                    this.userData.AreDungeonsVisible = value;
                    this.OnPropertyChanged(() => this.ShowDungeons);
                }
            }
        }

        /// <summary>
        /// Constructs a new ZoneItemsViewModel object
        /// </summary>
        public ZoneItemsViewModel(ZoneItemsStore zoneItemsStore, IZoneCompletionController zoneController, MapUserData userData)
        {
            this.zoneItemsStore = zoneItemsStore;
            this.zoneController = zoneController;
            this.userData = userData;

            this.ShowHeartQuests = true;
            this.ShowHeroPoints = true;
            this.ShowPOIs = true;
            this.ShowVistas = true;
            this.ShowWaypoints = true;
            this.ShowDungeons = true;

            ((INotifyPropertyChanged)this.zoneController).PropertyChanged += (o, e) =>
            {
                if (!this.userData.ShowEntireContinent && e.PropertyName.ToLower().Contains("map"))
                {
                    this.OnPropertyChanged(() => this.Waypoints);
                    this.OnPropertyChanged(() => this.POIs);
                    this.OnPropertyChanged(() => this.Vistas);
                    this.OnPropertyChanged(() => this.HeartQuests);
                    this.OnPropertyChanged(() => this.HeroPoints);
                    this.OnPropertyChanged(() => this.Dungeons);
                }
            };

            this.zoneItemsStore.DataLoaded += (o, e) =>
            {
                this.OnPropertyChanged(() => this.Waypoints);
                this.OnPropertyChanged(() => this.POIs);
                this.OnPropertyChanged(() => this.Vistas);
                this.OnPropertyChanged(() => this.HeartQuests);
                this.OnPropertyChanged(() => this.HeroPoints);
                this.OnPropertyChanged(() => this.Dungeons);
            };
        }
    }
}
