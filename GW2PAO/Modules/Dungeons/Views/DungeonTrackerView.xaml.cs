using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using GW2PAO.Modules.Dungeons.Interfaces;
using GW2PAO.Modules.Dungeons.ViewModels;
using GW2PAO.Views;
using NLog;

namespace GW2PAO.Modules.Dungeons.Views
{
    /// <summary>
    /// Interaction logic for DungeonTrackerView.xaml
    /// </summary>
    public partial class DungeonTrackerView : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Actual height of a dungeon in the list
        /// </summary>
        private double dungeonHeight;

        /// <summary>
        /// Count used for keeping track of when we need to adjust our
        /// maximum height/width if the number of visible dungeons
        /// changes
        /// </summary>
        private int prevVisibleDungeonsCount = 0;

        /// <summary>
        /// Height before collapsing the control
        /// </summary>
        private double beforeCollapseHeight;

        /// <summary>
        /// Dungeon tracker view model
        /// </summary>
        [Import]
        public DungeonTrackerViewModel ViewModel
        {
            get
            {
                return this.DataContext as DungeonTrackerViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DungeonTrackerView()
        {
            logger.Debug("New DungeonTrackerView created");
            InitializeComponent();

            this.dungeonHeight = new DungeonView().Height;

            this.ResizeHelper.InitializeResizeElements(this.ResizeHeight, null);
            this.Loaded += DungeonTrackerView_Loaded;
        }

        private void DungeonTrackerView_Loaded(object sender, RoutedEventArgs e)
        {
            // Set up resize snapping
            this.ResizeHelper.SnappingHeightOffset = 12;
            this.ResizeHelper.SnappingThresholdHeight = (int)this.TitleBar.ActualHeight;
            this.ResizeHelper.SnappingIncrementHeight = (int)this.dungeonHeight;

            // Save the height values for use when collapsing the window
            this.RefreshWindowHeights();
            this.Height = Properties.Settings.Default.DungeonTrackerHeight;

            this.DungeonsContainer.LayoutUpdated += DungeonsContainer_LayoutUpdated;
            this.Closing += DungeonTrackerView_Closing;
            this.beforeCollapseHeight = this.Height;
        }

        /// <summary>
        /// Refreshes the MinHeight and MaxHeight of the window
        /// based on collapsed status and number of visible items
        /// </summary>
        private void RefreshWindowHeights()
        {
            var visibleObjsCount = this.ViewModel.Dungeons.Count(o => o.IsVisible);
            if (this.DungeonsContainer.Visibility == System.Windows.Visibility.Visible)
            {
                // Expanded
                this.MinHeight = dungeonHeight + this.TitleBar.ActualHeight; // minimum of 1 dungeon
                this.MaxHeight = (visibleObjsCount * dungeonHeight) + this.TitleBar.ActualHeight + 2;
            }
            else
            {
                // Collapsed, don't touch the height
            }
        }

        private void DungeonsContainer_LayoutUpdated(object sender, EventArgs e)
        {
            var visibleObjsCount = this.ViewModel.Dungeons.Count(o => o.IsVisible);
            if (prevVisibleDungeonsCount != visibleObjsCount)
            {
                prevVisibleDungeonsCount = visibleObjsCount;
                this.RefreshWindowHeights();
            }
        }

        private void DungeonTrackerView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                Properties.Settings.Default.DungeonTrackerHeight = this.Height;
                Properties.Settings.Default.DungeonTrackerWidth = this.Width;
                Properties.Settings.Default.DungeonTrackerX = this.Left;
                Properties.Settings.Default.DungeonTrackerY = this.Top;
                Properties.Settings.Default.Save();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            logger.Debug("DungeonTrackerView closed");
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
            e.Handled = true;
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
            if (this.DungeonsContainer.Visibility == System.Windows.Visibility.Visible)
            {
                this.beforeCollapseHeight = this.Height;
                this.MinHeight = this.TitleBar.ActualHeight;
                this.MaxHeight = this.TitleBar.ActualHeight;
                this.Height = this.TitleBar.ActualHeight;
                this.DungeonsContainer.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.DungeonsContainer.Visibility = System.Windows.Visibility.Visible;
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
