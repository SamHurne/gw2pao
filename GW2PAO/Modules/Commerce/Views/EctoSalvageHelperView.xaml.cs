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
using GW2PAO.Views;
using NLog;
using System.ComponentModel.Composition;
using GW2PAO.Modules.Commerce.ViewModels;

namespace GW2PAO.Modules.Commerce.Views
{
    /// <summary>
    /// Interaction logic for DungeonTrackerView.xaml
    /// </summary>
    public partial class EctoSalvageHelperView : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// View model
        /// </summary>
        [Import]
        public EctoSalvageHelperViewModel ViewModel
        {
            get
            {
                return this.DataContext as EctoSalvageHelperViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public EctoSalvageHelperView()
        {
            logger.Debug("New EctoSalvageHelperView created");
            InitializeComponent();

            // Set the window size and location
            this.Closing += TPCalculatorView_Closing;
            this.Left = Properties.Settings.Default.EctoHelperX;
            this.Top = Properties.Settings.Default.TPCalculatorY;
        }

        private void TPCalculatorView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                Properties.Settings.Default.EctoHelperX = this.Left;
                Properties.Settings.Default.EctoHelperY = this.Top;
                Properties.Settings.Default.Save();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            logger.Debug("EctoSalvageHelperView closed");
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
            if (this.ThresholdDisplay.Visibility == System.Windows.Visibility.Visible)
            {
                this.ThresholdDisplay.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.ThresholdDisplay.Visibility = System.Windows.Visibility.Visible;
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
