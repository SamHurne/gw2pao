using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GW2PAO.Modules.Commerce.ViewModels.PriceTracker;
using GW2PAO.Views;
using NLog;

namespace GW2PAO.Modules.Commerce.Views.PriceTracker
{
    /// <summary>
    /// Interaction logic for PriceTrackerView.xaml
    /// </summary>
    public partial class PriceTrackerView : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Actual height of an item in the list
        /// </summary>
        private double priceHeight;

        /// <summary>
        /// Height before collapsing the control
        /// </summary>
        private double beforeCollapseHeight;

        /// <summary>
        /// View model
        /// </summary>
        [Import]
        public PriceListViewModel ViewModel
        {
            get
            {
                return this.DataContext as PriceListViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="commerceController">The commerce controller</param>
        public PriceTrackerView()
        {
            logger.Debug("New PriceTrackerView created");
            InitializeComponent();

            this.priceHeight = new ItemPriceView().Height;

            this.ResizeHelper.InitializeResizeElements(this.ResizeHeight, null);
            this.Loaded += PriceTrackerView_Loaded;
        }

        private void PriceTrackerView_Loaded(object sender, RoutedEventArgs e)
        {
            // Set up resize snapping
            this.ResizeHelper.SnappingHeightOffset = -12;
            this.ResizeHelper.SnappingThresholdHeight = (int)this.TitleBar.ActualHeight;
            this.ResizeHelper.SnappingIncrementHeight = (int)this.priceHeight;

            // Save the height values for use when collapsing the window
            this.RefreshWindowHeights();
            this.Height = GW2PAO.Properties.Settings.Default.PriceTrackerHeight;

            this.Closing += PriceTrackerView_Closing;
            this.beforeCollapseHeight = this.Height;
        }

        /// <summary>
        /// Refreshes the MinHeight and MaxHeight of the window
        /// based on collapsed status
        /// </summary>
        private void RefreshWindowHeights()
        {
            if (this.ItemsContainer.Visibility == System.Windows.Visibility.Visible)
            {
                // Expanded
                this.MinHeight = priceHeight + this.TitleBar.ActualHeight + 1; // Minimum of 1 items
            }
            else
            {
                // Collapsed, don't touch the height
            }
        }

        private void PriceTrackerView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                Properties.Settings.Default.PriceTrackerHeight = this.Height;
                Properties.Settings.Default.PriceTrackerWidth = this.Width;
                Properties.Settings.Default.PriceTrackerX = this.Left;
                Properties.Settings.Default.PriceTrackerY = this.Top;
                Properties.Settings.Default.Save();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            logger.Debug("PriceTrackerView closed");
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
            if (this.ItemsContainer.Visibility == System.Windows.Visibility.Visible)
            {
                this.beforeCollapseHeight = this.Height;
                this.MinHeight = this.TitleBar.ActualHeight;
                this.MaxHeight = this.TitleBar.ActualHeight;
                this.Height = this.TitleBar.ActualHeight;
                this.ItemsContainer.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.ItemsContainer.Visibility = System.Windows.Visibility.Visible;
                this.MaxHeight = 5000;
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

