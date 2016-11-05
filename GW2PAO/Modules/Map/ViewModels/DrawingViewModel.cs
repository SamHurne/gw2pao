using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Modules.Map.Models;
using MapControl;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Map.ViewModels
{
    public class DrawingViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private int currentContinentId;
        private PolyLine activePolyline;
        private bool isVisible;

        /// <summary>
        /// The backing drawing model data
        /// </summary>
        public Drawing Drawing
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of location objects making up the player trail
        /// </summary>
        public ObservableCollection<PolyLine> Polylines
        {
            get { return this.Drawing.Polylines; }
        }

        /// <summary>
        /// The active polyline that is being edited
        /// </summary>
        public PolyLine ActivePolyline
        {
            get { return this.activePolyline; }
            set { SetProperty(ref this.activePolyline, value); }
        }

        /// <summary>
        /// Color of the drawing
        /// </summary>
        public string Color
        {
            get { return this.Drawing.Color; }
            set
            {
                if (this.Drawing.Color != value)
                {
                    this.Drawing.Color = value;
                    this.OnPropertyChanged(() => this.Color);
                }
            }
        }

        /// <summary>
        /// True if this marker is set as visible, else false
        /// </summary>
        public bool IsVisible
        {
            get { return this.isVisible; }
            set { SetProperty(ref this.isVisible, value); }
        }

        public DrawingViewModel(Drawing drawing, int currentContinentId)
        {
            this.currentContinentId = currentContinentId;
            this.Drawing = drawing;

            this.RefreshVisibility();
        }

        public void BeginNewPolyline()
        {
            this.ActivePolyline = new PolyLine();
            this.Polylines.Add(this.ActivePolyline);
        }

        public void OnContinentChanged(int currentContinentId)
        {
            this.currentContinentId = currentContinentId;
            this.RefreshVisibility();
        }

        private void RefreshVisibility()
        {
            bool isVisible = true;

            isVisible &= this.Drawing.ContinentId == currentContinentId;

            this.IsVisible = isVisible;
        }
    }
}
