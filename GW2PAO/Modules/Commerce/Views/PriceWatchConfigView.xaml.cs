using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GW2PAO.Modules.Commerce.ViewModels;
using GW2PAO.Views;
using NLog;

namespace GW2PAO.Modules.Commerce.Views
{
    /// <summary>
    /// Interaction logic for DungeonTrackerView.xaml
    /// </summary>
    public partial class PriceWatchConfigView : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// View model
        /// </summary>
        [Import]
        public MonitoredItemsConfigViewModel ViewModel
        {
            get
            {
                return this.DataContext as MonitoredItemsConfigViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PriceWatchConfigView()
        {
            logger.Debug("New PriceWatchConfigView created");
            InitializeComponent();

            // Set the window size and location
            this.Closing += TPCalculatorView_Closing;
        }

        private void TPCalculatorView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                Properties.Settings.Default.TPCalculatorX = this.Left;
                Properties.Settings.Default.TPCalculatorY = this.Top;
                Properties.Settings.Default.Save();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            logger.Debug("TPCalculatorView closed");
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CollapseExpandButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.MainContent.Visibility == System.Windows.Visibility.Visible)
            {
                this.MainContent.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.MainContent.Visibility = System.Windows.Visibility.Visible;
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
