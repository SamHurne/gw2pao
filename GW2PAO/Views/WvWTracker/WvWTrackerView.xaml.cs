using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GW2PAO.API.Data.Enums;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.PresentationCore;
using GW2PAO.ViewModels.DungeonTracker;
using GW2PAO.ViewModels.Interfaces;
using GW2PAO.ViewModels.WvWTracker;
using NLog;

namespace GW2PAO.Views.WvWTracker
{
    /// <summary>
    /// Interaction logic for WvWTrackerView.xaml
    /// </summary>
    public partial class WvWTrackerView : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // Vertical Orientation Size Constants:
        private const double VERTICAL_MIN_HEIGHT = 58;
        private const double VERTICAL_MAX_HEIGHT = 627;
        private const double VERTICAL_DEF_HEIGHT = 250;

        private const double VERTICAL_MIN_WIDTH = 125;
        private const double VERTICAL_MAX_WIDTH = 350;
        private const double VERTICAL_DEF_WIDTH = 182;

        // Horizontal Orientation Size Constants:
        private const double HORIZONTAL_MIN_HEIGHT = 76;
        private const double HORIZONTAL_MAX_HEIGHT = 125;

        private const double HORIZONTAL_MIN_WIDTH = 190;
        private const double HORIZONTAL_MAX_WIDTH = 1124;
        private const double HORIZONTAL_DEF_WIDTH = 350;

        /// <summary>
        /// Height before collapsing the control
        /// </summary>
        private double beforeCollapseHeight;

        /// <summary>
        /// Count used for keeping track of when we need to adjust our
        /// maximum height/width if the number of visible objectives
        /// changes
        /// </summary>
        private int prevObjsCount = 0;

        /// <summary>
        /// WvW controller
        /// </summary>
        private IWvWController controller;

        /// <summary>
        /// WvW tracker view model
        /// </summary>
        private WvWTrackerViewModel viewModel;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dungeonsController">The dungeons controller</param>
        public WvWTrackerView(IWvWController wvwController, IHasWvWMap mapVm)
        {
            logger.Debug("New WvWTrackerView created");
            this.controller = wvwController;
            this.viewModel = new WvWTrackerViewModel(this.controller, mapVm);
            this.DataContext = this.viewModel;
            InitializeComponent();

            // Set initial height and widths
            if (this.viewModel.IsHorizontalOrientation)
            {
                // Horizontal Orientation
                this.MinHeight = HORIZONTAL_MIN_HEIGHT;
                this.MaxHeight = HORIZONTAL_MAX_HEIGHT;
                this.Height = this.MaxHeight;
                this.MinWidth = HORIZONTAL_MIN_WIDTH;
                this.MaxWidth = HORIZONTAL_MAX_WIDTH;
                this.Width = HORIZONTAL_DEF_WIDTH;
            }
            else
            {
                // Vertical Orientation
                this.MinHeight = VERTICAL_MIN_HEIGHT;
                this.MaxHeight = VERTICAL_MAX_HEIGHT;
                this.Height = VERTICAL_MAX_HEIGHT / 3;
                this.MinWidth = VERTICAL_MIN_WIDTH;
                this.MaxWidth = VERTICAL_MAX_WIDTH;
                this.Width = VERTICAL_DEF_WIDTH;
            }

            this.Closing += WvWTrackerView_Closing;
            if (Properties.Settings.Default.WvWTrackerHeight > 0)
                this.Height = Properties.Settings.Default.WvWTrackerHeight;
            if (Properties.Settings.Default.WvWTrackerWidth > 0)
                this.Width = Properties.Settings.Default.WvWTrackerWidth;

            this.beforeCollapseHeight = this.Height;

            this.ResizeHelper.InitializeResizeElements(this.ResizeHeight, this.ResizeWidth);
            this.Loaded += (o, e) =>
                {
                    this.RefreshWindowHeights(false);
                    this.RefreshWindowWidths(false);
                    this.RefreshResizeSnapping();

                    // Set the window size and location
                    this.viewModel.PropertyChanged += viewModel_PropertyChanged;
                    this.ObjectivesContainer.LayoutUpdated += ObjectivesContainer_LayoutUpdated;
                };
        }

        /// <summary>
        /// Refreshes the resize snap increments, etc
        /// </summary>
        private void RefreshResizeSnapping()
        {
            var obj = this.ObjectivesContainer.ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement;
            if (obj != null)
            {
                if (this.viewModel.IsHorizontalOrientation)
                {
                    this.ResizeHelper.SnappingHeightOffset = 0;
                    this.ResizeHelper.SnappingIncrementHeight = 1;
                    this.ResizeHelper.SnappingWidthOffset = 3;
                    this.ResizeHelper.SnappingIncrementWidth = (int)obj.ActualWidth;
                }
                else
                {
                    this.ResizeHelper.SnappingHeightOffset = 6;
                    this.ResizeHelper.SnappingIncrementHeight = (int)obj.ActualHeight;
                    this.ResizeHelper.SnappingWidthOffset = 0;
                    this.ResizeHelper.SnappingIncrementWidth = 1;
                }
            }
        }

        /// <summary>
        /// Refreshes the MinHeight, Height, and MaxHeight of the window
        /// based on orientation, collapsed status, and number of visible items
        /// </summary>
        private void RefreshWindowHeights(bool resetHeight)
        {
            var objsCount = this.viewModel.Objectives.Count();
            var objContainer = this.ObjectivesContainer.ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement;
            if (objContainer != null)
            {
                if (objContainer.ActualHeight == 0)
                    objContainer = this.ObjectivesContainer.ItemContainerGenerator.ContainerFromIndex(1) as FrameworkElement;

                var objHeight = (int)objContainer.ActualHeight;
                if (this.ObjectivesContainer.Visibility == System.Windows.Visibility.Visible)
                {
                    // Expanded
                    if (this.viewModel.IsHorizontalOrientation)
                    {
                        // Horizontal Orientation
                        this.MinHeight = HORIZONTAL_MIN_HEIGHT;
                        this.MaxHeight = this.TitleBar.ActualHeight + objHeight;

                        if (resetHeight)
                            this.Height = this.MaxHeight;
                    }
                    else
                    {
                        // Vertical Orientation
                        this.MinHeight = VERTICAL_MIN_HEIGHT;
                        this.MaxHeight = this.TitleBar.ActualHeight + (objHeight * objsCount) + 5;

                        if (resetHeight)
                            this.Height = this.TitleBar.ActualHeight + (objHeight * 5);
                    }
                }
                else
                {
                    // Collapsed, don't touch the height unless we are resetting it
                    if (resetHeight)
                    {
                        if (this.viewModel.IsHorizontalOrientation)
                            this.beforeCollapseHeight = this.TitleBar.ActualHeight + objHeight;
                        else
                            this.beforeCollapseHeight = this.TitleBar.ActualHeight + (objHeight * 5);
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the MinWidth, Width, and MaxWidth of the window
        /// based on collapsed status, and number of visible items
        /// </summary>
        private void RefreshWindowWidths(bool resetWidth)
        {
            var objsCount = this.viewModel.Objectives.Count();
            var objContainer = this.ObjectivesContainer.ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement;
            if (objContainer != null)
            {
                var objWidth = (int)objContainer.ActualWidth;
                if (this.viewModel.IsHorizontalOrientation)
                {
                    // Horizontal Orientation
                    this.MinWidth = HORIZONTAL_MIN_WIDTH;
                    this.MaxWidth = objWidth * objsCount;

                    if (resetWidth)
                        this.Width = objWidth * 5;
                }
                else
                {
                    // Vertical Orientation
                    this.MinWidth = VERTICAL_MIN_WIDTH;
                    this.MaxWidth = VERTICAL_MAX_WIDTH;

                    if (resetWidth)
                        this.Width = VERTICAL_DEF_WIDTH;
                }
            }
        }

        /// <summary>
        /// Event handler for the objective container's layout updated
        /// This is used for determining when the amount of visible objectives changes
        /// so that we can update our maximum height/widths accordingly
        /// </summary>
        private void ObjectivesContainer_LayoutUpdated(object sender, EventArgs e)
        {
            var objsCount = this.viewModel.Objectives.Count;
            if (prevObjsCount != objsCount)
            {
                prevObjsCount = objsCount;
                this.RefreshWindowHeights(false);
                this.RefreshWindowWidths(false);
            }
        }

        /// <summary>
        /// Event handler for the view model's property changed event. Current only used for setting orientation
        /// </summary>
        private void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsHorizontalOrientation")
            {
                Task.Factory.StartNew(() =>
                {
                    // Delayed refresh... terrible way to do this, but works for now
                    // TODO: Do this a better way... find a better event to do this
                    // The problem is that when this property is set, the actual itemscontrol
                    // elements haven't been regenerated for the orientation change, so we
                    // don't know what our max height/widths should be. Having a small
                    // sleep here means we'll do this after the items have been regenerated
                    System.Threading.Thread.Sleep(25);
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.RefreshWindowHeights(true);
                        this.RefreshWindowWidths(true);
                        this.RefreshResizeSnapping();
                    }));
                });
            }
        }

        private void WvWTrackerView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                Properties.Settings.Default.WvWTrackerHeight = this.Height;
                Properties.Settings.Default.WvWTrackerWidth = this.Width;
                Properties.Settings.Default.WvWTrackerX = this.Left;
                Properties.Settings.Default.WvWTrackerY = this.Top;
                Properties.Settings.Default.Save();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            logger.Debug("WvWTrackerView closed");
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // This prevents Aero snapping
            if (this.ResizeMode != System.Windows.ResizeMode.NoResize)
            {
                this.ResizeMode = System.Windows.ResizeMode.NoResize;
                this.UpdateLayout();
            }

            this.DragMove();
        }

        private void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ResizeMode == System.Windows.ResizeMode.NoResize)
            {
                // Restore resize grips (removed on mouse-down to prevent Aero snapping)
                this.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                this.UpdateLayout();
            }
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CollapseExpandButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ObjectivesContainer.Visibility == System.Windows.Visibility.Visible)
            {
                this.beforeCollapseHeight = this.Height;
                this.MinHeight = this.TitleBar.ActualHeight;
                this.MaxHeight = this.TitleBar.ActualHeight;
                this.Height = this.TitleBar.ActualHeight;
                this.ObjectivesContainer.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                var objsCount = this.viewModel.Objectives.Count();
                var objContainer = this.ObjectivesContainer.ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement;
                if (objContainer != null)
                {
                    var objHeight = (int)objContainer.ActualHeight;
                    if (this.viewModel.IsHorizontalOrientation)
                    {
                        // Horizontal Orientation
                        this.MinHeight = HORIZONTAL_MIN_HEIGHT;
                        this.MaxHeight = this.TitleBar.ActualHeight + objHeight;
                    }
                    else
                    {
                        // Vertical Orientation
                        this.MinHeight = VERTICAL_MIN_HEIGHT;
                        this.MaxHeight = this.TitleBar.ActualHeight + (objHeight * objsCount) + 5;
                    }
                }
                this.Height = this.beforeCollapseHeight;
                this.ObjectivesContainer.Visibility = System.Windows.Visibility.Visible;
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

        /// <summary>
        /// Handles the PreviewMouseWheel on the scrollviewer to enable scrolling the horizontal scrollbar via the mousewheel
        /// </summary>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            if (sv != null)
            {
                if (sv.ComputedHorizontalScrollBarVisibility == System.Windows.Visibility.Visible)
                {
                    if (e.Delta > 0)
                    {
                        sv.LineLeft();
                    }
                    else
                    {
                        sv.LineRight();
                    }
                    e.Handled = true;
                }
            }
        }
    }
}
