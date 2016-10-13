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
using GW2PAO.Modules.DayNight.ViewModels;

namespace GW2PAO.Modules.DayNight.Views
{
    /// <summary>
    /// Interaction logic for DayNightTimerView.xaml
    /// </summary>
    public partial class DayNightTimerView : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Height before collapsing the control
        /// </summary>
        private double beforeCollapseHeight;

        private const double MIN_WIDTH = 100;
        private const double MIN_HEIGHT = 75;

        /// <summary>
        /// View model
        /// </summary>
        [Import]
        public DayNightTimerViewModel ViewModel
        {
            get
            {
                return this.DataContext as DayNightTimerViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DayNightTimerView()
        {
            logger.Debug("New DayNightTimerView created");
            InitializeComponent();

            this.ResizeHelper.InitializeResizeElements(null, null, this.ResizeGripper);
            this.MinHeight = MIN_HEIGHT;
            this.MinWidth = MIN_WIDTH;

            // Set the window size and location
            this.Closing += DayNightTimerView_Closing;
            this.Left = Properties.Settings.Default.DayNightX;
            this.Top = Properties.Settings.Default.DayNightY;
            this.Height = Properties.Settings.Default.DayNightHeight;
            this.Width = Properties.Settings.Default.DayNightWidth;
            this.beforeCollapseHeight = this.Height;
        }

        private void DayNightTimerView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                if (this.OverlayContents.Visibility == System.Windows.Visibility.Visible)
                    Properties.Settings.Default.DayNightHeight = this.ActualHeight;
                else
                    Properties.Settings.Default.DayNightHeight = this.beforeCollapseHeight;

                Properties.Settings.Default.DayNightWidth = this.Width;
                Properties.Settings.Default.DayNightX = this.Left;
                Properties.Settings.Default.DayNightY = this.Top;
                Properties.Settings.Default.Save();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            logger.Debug("DayNightTimerView closed");
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
            if (this.OverlayContents.Visibility == System.Windows.Visibility.Visible)
            {
                this.beforeCollapseHeight = this.ActualHeight;
                this.MinHeight = this.TitleBar.ActualHeight;
                this.Height = this.TitleBar.ActualHeight;
                this.OverlayContents.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.OverlayContents.Visibility = System.Windows.Visibility.Visible;
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
