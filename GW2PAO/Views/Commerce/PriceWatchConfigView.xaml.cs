using System;
using System.Collections.Generic;
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
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.ViewModels;
using GW2PAO.ViewModels.Dungeon;
using GW2PAO.ViewModels.Commerce;
using NLog;

namespace GW2PAO.Views.Commerce
{
    /// <summary>
    /// Interaction logic for DungeonTrackerView.xaml
    /// </summary>
    public partial class PriceWatchConfigView : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The commerce controller
        /// </summary>
        private ICommerceController controller;

        /// <summary>
        /// View model object
        /// </summary>
        private PriceWatchConfigViewModel viewModel;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PriceWatchConfigView(ICommerceService commerceService, ICommerceController controller)
        {
            logger.Debug("New PriceWatchConfigView created");
            this.controller = controller;
            this.viewModel = new PriceWatchConfigViewModel(commerceService, controller);
            this.DataContext = this.viewModel;
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

        private void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
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
