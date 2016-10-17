using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GW2PAO.Modules.Events.ViewModels.MetaEventTimers;
using GW2PAO.Views;
using GW2PAO.Views.Events.MetaEventTimers;
using NLog;

namespace GW2PAO.Modules.Events.Views.MetaEventTimers
{
    /// <summary>
    /// Interaction logic for MetaEventTimersView.xaml
    /// </summary>
    public partial class MetaEventTimersView : OverlayWindow
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
        /// Count used for keeping track of when we need to adjust our
        /// maximum height/width if the number of visible events
        /// changes
        /// </summary>
        private int prevVisibleEventsCount = 0;

        /// <summary>
        /// Height before collapsing the control
        /// </summary>
        private double beforeCollapseHeight;

        /// <summary>
        /// View model
        /// </summary>
        [Import]
        public MetaEventListViewModel ViewModel
        {
            get
            {
                return this.DataContext as MetaEventListViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MetaEventTimersView()
        {
            logger.Debug("New MetaEventTimersView created");
            InitializeComponent();

            this.eventHeight = new MetaEventTimerView().Height;

            this.ResizeHelper.InitializeResizeElements(null, null, this.ResizeGripper);
            this.Loaded += EventTrackerView_Loaded;
        }

        private void EventTrackerView_Loaded(object sender, RoutedEventArgs e)
        {
            // Set up resize snapping
            this.ResizeHelper.SnappingHeightOffset = 12;
            this.ResizeHelper.SnappingThresholdHeight = (int)this.TitleBar.ActualHeight;
            this.ResizeHelper.SnappingIncrementHeight = (int)this.eventHeight;

            // Save the height values for use when collapsing the window
            this.RefreshWindowHeights();
            this.Height = GW2PAO.Properties.Settings.Default.MetaEventTimersHeight;

            this.EventsContainer.LayoutUpdated += EventsContainer_LayoutUpdated;
            this.Closing += EventTrackerView_Closing;
            this.beforeCollapseHeight = this.Height;
        }

        /// <summary>
        /// Refreshes the MinHeight and MaxHeight of the window
        /// based on collapsed status and number of visible items
        /// </summary>
        private void RefreshWindowHeights()
        {
            var visibleObjsCount = this.ViewModel.MetaEvents.Count(o => o.IsVisible);
            if (this.EventsContainer.Visibility == System.Windows.Visibility.Visible)
            {
                // Expanded
                this.MinHeight = eventHeight * 2 + this.TitleBar.ActualHeight; // Minimum of 2 events
                if (visibleObjsCount < 2)
                    this.MaxHeight = this.MinHeight;
                else
                    this.MaxHeight = (visibleObjsCount * eventHeight) + this.TitleBar.ActualHeight + 2;
            }
            else
            {
                // Collapsed, don't touch the height
            }
        }

        private void EventsContainer_LayoutUpdated(object sender, EventArgs e)
        {
            var visibleObjsCount = this.ViewModel.MetaEvents.Count(o => o.IsVisible);
            if (prevVisibleEventsCount != visibleObjsCount)
            {
                prevVisibleEventsCount = visibleObjsCount;
                this.RefreshWindowHeights();
            }
        }

        private void EventTrackerView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                if (this.EventsContainer.Visibility == System.Windows.Visibility.Visible)
                {
                    Properties.Settings.Default.MetaEventTimersHeight = this.Height;
                }
                else
                {
                    Properties.Settings.Default.MetaEventTimersHeight = this.beforeCollapseHeight;
                }
                Properties.Settings.Default.MetaEventTimersWidth = this.Width;
                Properties.Settings.Default.MetaEventTimersX = this.Left;
                Properties.Settings.Default.MetaEventTimersY = this.Top;
                Properties.Settings.Default.Save();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            logger.Debug("MetaEventTimersView closed");
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
            if (this.EventsContainer.Visibility == System.Windows.Visibility.Visible)
            {
                this.beforeCollapseHeight = this.Height;
                this.MinHeight = this.TitleBar.ActualHeight;
                this.MaxHeight = this.TitleBar.ActualHeight;
                this.Height = this.TitleBar.ActualHeight;
                this.EventsContainer.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.EventsContainer.Visibility = System.Windows.Visibility.Visible;
                this.RefreshWindowHeights();
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
    }
}

