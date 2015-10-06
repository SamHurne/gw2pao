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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GW2PAO.Modules.ZoneCompletion.ViewModels;
using GW2PAO.Views;
using NLog;

namespace GW2PAO.Modules.ZoneCompletion.Views
{
    /// <summary>
    /// Interaction logic for ZoneCompletionView.xaml
    /// </summary>
    public partial class ZoneCompletionView : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private const double MINIMUM_HEIGHT = 78;

        /// <summary>
        /// Height before collapsing the control
        /// </summary>
        private double beforeCollapseHeight;

        /// <summary>
        /// Zone Completion Assistant view model
        /// </summary>
        [Import]
        public ZoneCompletionViewModel ViewModel
        {
            get
            {
                return this.DataContext as ZoneCompletionViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="zoneItemsController">The zone completion controller</param>
        /// <param name="zoneName">The zone name view model</param>
        public ZoneCompletionView()
        {
            logger.Debug("New ZoneCompletionView created");
            InitializeComponent();

            this.ResizeHelper.InitializeResizeElements(this.ResizeHeight, null);

            // Save the height values for use when collapsing the window
            this.MinHeight = MINIMUM_HEIGHT;
            this.Height = Properties.Settings.Default.ZoneAssistantHeight;

            this.Closing += ZoneCompletionView_Closing;
            this.beforeCollapseHeight = this.Height;
        }

        private void ZoneCompletionView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                if (this.ItemsContainer.Visibility == System.Windows.Visibility.Visible)
                {
                    Properties.Settings.Default.ZoneAssistantHeight = this.Height;
                }
                else
                {
                    Properties.Settings.Default.ZoneAssistantHeight = this.beforeCollapseHeight;
                }
                Properties.Settings.Default.ZoneAssistantWidth = this.Width;
                Properties.Settings.Default.ZoneAssistantX = this.Left;
                Properties.Settings.Default.ZoneAssistantY = this.Top;
                Properties.Settings.Default.Save();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            logger.Debug("ZoneCompletionView closed");
            this.ViewModel.Shutdown();
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
                this.Height = this.TitleBar.ActualHeight;
                this.ItemsContainer.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.MinHeight = MINIMUM_HEIGHT;
                this.Height = this.beforeCollapseHeight;
                this.ItemsContainer.Visibility = System.Windows.Visibility.Visible;
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
