using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Modules.Map.Models;
using GW2PAO.Modules.Tasks;
using GW2PAO.Modules.Tasks.Interfaces;
using GW2PAO.Modules.Tasks.ViewModels;
using GW2PAO.PresentationCore;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Map.ViewModels
{
    [Export(typeof(DrawingsViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DrawingsViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IZoneService zoneService;
        private IPlayerService playerService;
        private MapUserData userData;
        private int currentContinentId;
        private bool penEnabled;

        /// <summary>
        /// The collection of drawings to show on the map
        /// </summary>
        public ObservableCollection<DrawingViewModel> Drawings
        {
            get;
            private set;
        }

        /// <summary>
        /// View model of the new drawing
        /// </summary>
        public DrawingViewModel NewDrawing
        {
            get;
            private set;
        }

        /// <summary>
        /// True if the drawing pen is enabled, else false
        /// </summary>
        public bool PenEnabled
        {
            get { return this.penEnabled; }
            set { SetProperty(ref this.penEnabled, value); }
        }

        /// <summary>
        /// The list of color options for the drawing pen
        /// </summary>
        public List<string> PenColorOptions
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructs a new MarkersViewModel object
        /// </summary>
        [ImportingConstructor]
        public DrawingsViewModel(IZoneService zoneService, IPlayerService playerService, MapUserData userData)
        {
            this.zoneService = zoneService;
            this.playerService = playerService;
            this.userData = userData;

            if (this.playerService.HasValidMapId)
            {
                var continent = this.zoneService.GetContinentByMap(this.playerService.MapId);
                this.currentContinentId = continent.Id;
            }
            else
            {
                this.currentContinentId = 1;
            }

            this.NewDrawing = new DrawingViewModel(new Drawing(), this.currentContinentId);
            this.PenEnabled = true;

            this.Drawings = new ObservableCollection<DrawingViewModel>();
            this.Drawings.Add(this.NewDrawing);

            this.PenColorOptions = new List<string>()
            {
                "#FFFFFF",
                "#000000",
                "#DD2C00",
                "#6200EA",
                "#2962FF",
                "#00C853",
                "#FFEA00",
                "#FF6D00"
            };
        }

        public void OnContinentChanged(int currentContinentId)
        {
            this.currentContinentId = currentContinentId;
            foreach (var drawing in this.Drawings)
            {
                drawing.OnContinentChanged(currentContinentId);
            }
        }
    }
}
