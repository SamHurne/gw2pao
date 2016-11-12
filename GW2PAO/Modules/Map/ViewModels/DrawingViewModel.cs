using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.Modules.Map.Models;
using GW2PAO.PresentationCore;
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

        private MapUserData userData;
        private int currentContinentId;
        private PolyLine activePolyline;
        private bool isSelected;
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
        /// The drawing's name
        /// </summary>
        public string Name
        {
            get { return this.Drawing.Name; }
            set
            {
                if (this.Drawing.Name != value)
                {
                    this.Drawing.Name = value;
                    this.OnPropertyChanged(() => this.Name);
                }
            }
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
        /// True if this drawing is configured as visible, else false
        /// </summary>
        public bool IsConfiguredVisible
        {
            get { return !this.userData.HiddenDrawings.Contains(this.Drawing.ID); }
            set
            {
                if (value)
                {
                    if (this.userData.HiddenDrawings.Contains(this.Drawing.ID))
                    {
                        this.userData.HiddenDrawings.Remove(this.Drawing.ID);
                        this.RefreshVisibility();
                        this.OnPropertyChanged(() => this.IsConfiguredVisible);
                    }
                }
                else
                {
                    if (!this.userData.HiddenDrawings.Contains(this.Drawing.ID))
                    {
                        this.userData.HiddenDrawings.Add(this.Drawing.ID);
                        this.RefreshVisibility();
                        this.OnPropertyChanged(() => this.IsConfiguredVisible);
                    }
                }
            }
        }

        /// <summary>
        /// True if this drawing is set as visible, else false
        /// </summary>
        public bool IsVisible
        {
            get { return this.isVisible; }
            set { SetProperty(ref this.isVisible, value); }
        }

        /// <summary>
        /// True if this drawing is selected in the list of drawings, else false
        /// </summary>
        public bool IsSelected
        {
            get { return this.isSelected; }
            set { SetProperty(ref this.isSelected, value); }
        }

        /// <summary>
        /// Command to delete this drawing
        /// </summary>
        public ICommand DeleteCommand { get; private set; }

        public DrawingViewModel(Drawing drawing, int currentContinentId, MapUserData userData)
        {
            this.currentContinentId = currentContinentId;
            this.Drawing = drawing;
            this.userData = userData;

            this.DeleteCommand = new DelegateCommand(this.Delete);
            this.BeginNewPolyline();
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
            isVisible &= !this.userData.HiddenDrawings.Contains(this.Drawing.ID);

            this.IsVisible = isVisible;
        }

        private void Delete()
        {
            if (this.userData.Drawings.Contains(this.Drawing))
            {
                this.userData.Drawings.Remove(this.Drawing);
            }
        }
    }
}
