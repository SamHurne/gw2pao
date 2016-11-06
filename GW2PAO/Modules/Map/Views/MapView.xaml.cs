using GW2PAO.Modules.Map.ViewModels;
using GW2PAO.Views;
using NLog;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GW2PAO.PresentationCore.DragDrop;
using GW2PAO.Modules.Map.Interfaces;
using MapControl;

namespace GW2PAO.Modules.Map.Views
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Height before collapsing the control
        /// </summary>
        private double beforeCollapseHeight;

        private bool neverClickThrough = false;
        protected override bool NeverClickThrough { get { return this.neverClickThrough; } }
        protected override bool SetNoFocus { get { return false; } }

        /// <summary>
        /// View model
        /// </summary>
        [Import]
        public MapViewModel ViewModel
        {
            get
            {
                return this.DataContext as MapViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MapView()
        {
            logger.Debug("New MapView created");
            InitializeComponent();

            this.ResizeHelper.InitializeResizeElements(null, null, this.ResizeGripper);
            this.Loaded += EventTrackerView_Loaded;
        }

        private void EventTrackerView_Loaded(object sender, RoutedEventArgs e)
        {
            // Save the height values for use when collapsing the window
            this.Closing += EventTrackerView_Closing;
            this.beforeCollapseHeight = this.Height;
        }

        private void EventTrackerView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                if (this.ControlsPanel.Visibility == System.Windows.Visibility.Visible)
                {
                    Properties.Settings.Default.MapViewHeight = this.ActualHeight;
                }
                else
                {
                    Properties.Settings.Default.MapViewHeight = this.beforeCollapseHeight;
                }
                Properties.Settings.Default.MapViewWidth = this.Width;
                Properties.Settings.Default.MapViewX = this.Left;
                Properties.Settings.Default.MapViewY = this.Top;
                Properties.Settings.Default.Save();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            logger.Debug("MapView closed");
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            e.Handled = true;
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CollapseExpandButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ControlsPanel.Visibility == System.Windows.Visibility.Visible)
            {
                this.beforeCollapseHeight = this.ActualHeight;
                this.MinHeight = this.TitleBar.ActualHeight;
                this.Height = this.TitleBar.ActualHeight;
                this.ControlsPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.ControlsPanel.Visibility = System.Windows.Visibility.Visible;
                this.Height = this.beforeCollapseHeight;
            }
        }

        private void TitleImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Image image = sender as Image;
                ContextMenu contextMenu = image.ContextMenu;
                contextMenu.PlacementTarget = image;
                contextMenu.IsOpen = true;
                e.Handled = true;
            }
        }

        private void MapMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.ViewModel.Drawings.PenEnabled)
            {
                this.ViewModel.Drawings.NewDrawing.BeginNewPolyline();
                this.ViewModel.Drawings.NewDrawing.ActivePolyline.Add(this.Map.ViewportPointToLocation(e.GetPosition(this.Map)));
            }
            else if (e.ClickCount == 2)
            {
                this.Map.ZoomMap(e.GetPosition(this.Map), Math.Floor(this.Map.ZoomLevel + 1.5));
            }
        }

        private void MapMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                this.Map.ZoomMap(e.GetPosition(this.Map), Math.Ceiling(this.Map.ZoomLevel - 1.5));
            }
        }

        private void MapMouseMove(object sender, MouseEventArgs e)
        {
            if (this.ViewModel.Drawings.PenEnabled && e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousePosition = e.GetPosition(this.Map);
                this.ViewModel.Drawings.NewDrawing.ActivePolyline.Add(this.Map.ViewportPointToLocation(mousePosition));
            }
        }

        private void MapManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 0.0001;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            this.Topmost = false;
            this.neverClickThrough = true;
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            this.neverClickThrough = false;
            this.WindowState = WindowState.Normal;
        }

        private void Map_OnDrop(object sender, RoutedEventArgs e)
        {
            // Calculate and set the dropped item's location on the actual map
            var dropArgs = e as OnDropEventArgs;
            if (dropArgs != null)
            {
                var locationHolder = dropArgs.Data as IHasMapLocation;
                if (locationHolder != null)
                {
                    var location = this.Map.ViewportPointToLocation(dropArgs.GetPosition(this.Map));
                    locationHolder.Location = new Location(location.Latitude, Location.NormalizeLongitude(location.Longitude));
                }

                // TODO:
                // Open pop-up or other controls to let a user enter a name/description
                //  - Maybe make this just bound to a bool on the view model of the object, something like "IsEditingName"?
            }
        }

        private void CollapseExpandDrawingPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.DrawingPanelContentsOpenIndicator.Visibility == Visibility.Visible)
                this.DrawingPanelContentsOpenIndicator.Visibility = Visibility.Collapsed;
            else
                this.DrawingPanelContentsOpenIndicator.Visibility = Visibility.Visible;
        }
    }
}

