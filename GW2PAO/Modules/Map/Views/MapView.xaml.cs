using GW2PAO.Modules.Map.ViewModels;
using GW2PAO.Views;
using GW2PAO.Views.Events.EventTracker;
using NLog;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        /// Actual height of an event in the list
        /// </summary>
        private double eventHeight;

        /// <summary>
        /// Height before collapsing the control
        /// </summary>
        private double beforeCollapseHeight;

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

            this.eventHeight = new WorldEventView().Height;

            this.ResizeHelper.InitializeResizeElements(null, null, this.ResizeGripper);
            this.Loaded += EventTrackerView_Loaded;
        }

        private void EventTrackerView_Loaded(object sender, RoutedEventArgs e)
        {
            // Save the height values for use when collapsing the window
            this.Height = GW2PAO.Properties.Settings.Default.EventTrackerHeight;

            this.Closing += EventTrackerView_Closing;
            this.beforeCollapseHeight = this.Height;
        }

        private void EventTrackerView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                if (this.MapContainer.Visibility == System.Windows.Visibility.Visible)
                {
                    Properties.Settings.Default.MapViewHeight = this.Height;
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
            if (this.MapContainer.Visibility == System.Windows.Visibility.Visible)
            {
                this.beforeCollapseHeight = this.ActualHeight;
                this.MinHeight = this.TitleBar.ActualHeight;
                this.Height = this.TitleBar.ActualHeight;
                this.MapContainer.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.MapContainer.Visibility = System.Windows.Visibility.Visible;
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
            if (e.ClickCount == 2)
            {
                this.Map.ZoomMap(e.GetPosition(this.Map), Math.Floor(this.Map.ZoomLevel + 1.5));
            }
        }

        private void TestMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
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
        }

        private void MapMouseLeave(object sender, MouseEventArgs e)
        {
        }

        private void MapManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 0.0001;
        }

        private void MapItemTouchDown(object sender, TouchEventArgs e)
        {
        }
    }
}

