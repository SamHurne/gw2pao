using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using GW2PAO.Controllers.Interfaces;
using GW2PAO.ViewModels.EventTracker;
using NLog;

namespace GW2PAO.Views.EventTracker
{
    /// <summary>
    /// Interaction logic for EventTrackerView.xaml
    /// </summary>
    public partial class EventTrackerView : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private const double minHeight = 84;
        private const double maxHeight = 380;
        private const double initialHeight = 125;

        /// <summary>
        /// Height before collapsing the control
        /// </summary>
        private double beforeCollapseHeight;

        /// <summary>
        /// True if the user is resizing the window, else false
        /// </summary>
        private bool resizeInProcess = false;

        /// <summary>
        /// Event tracker controller
        /// </summary>
        private IEventsController controller;

        /// <summary>
        /// Event tracker view model
        /// </summary>
        private EventTrackerViewModel viewModel;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="eventListController">The event tracker controller</param>
        public EventTrackerView(IEventsController eventListController)
        {
            logger.Debug("New EventTrackerView created");
            this.controller = eventListController;
            this.viewModel = new EventTrackerViewModel(this.controller);
            this.DataContext = this.viewModel;
            InitializeComponent();

            // Set the window size and location
            this.MinHeight = minHeight;
            this.MaxHeight = maxHeight;
            this.Height = initialHeight;

            this.Closing += EventTrackerView_Closing;
            if (Properties.Settings.Default.EventTrackerHeight > 0)
                this.Height = Properties.Settings.Default.EventTrackerHeight;
            if (Properties.Settings.Default.EventTrackerWidth > 0)
                this.Width = Properties.Settings.Default.EventTrackerWidth;
            this.Left = Properties.Settings.Default.EventTrackerX;
            this.Top = Properties.Settings.Default.EventTrackerY;

            this.beforeCollapseHeight = this.Height;
        }

        private void EventTrackerView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                Properties.Settings.Default.EventTrackerHeight = this.Height;
                Properties.Settings.Default.EventTrackerWidth = this.Width;
                Properties.Settings.Default.EventTrackerX = this.Left;
                Properties.Settings.Default.EventTrackerY = this.Top;
                Properties.Settings.Default.Save();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.controller.Stop();
            logger.Debug("EventTrackerView closed");
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
                this.MinHeight = minHeight;
                this.MaxHeight = maxHeight;
                this.Height = this.beforeCollapseHeight;
                this.EventsContainer.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Resize_Init(object sender, MouseButtonEventArgs e)
        {
            Grid senderRect = sender as Grid;

            if (senderRect != null)
            {
                resizeInProcess = true;
                senderRect.CaptureMouse();
            }
        }

        private void Resize_End(object sender, MouseButtonEventArgs e)
        {
            Grid senderRect = sender as Grid;
            if (senderRect != null)
            {
                resizeInProcess = false;
                senderRect.ReleaseMouseCapture();
            }
        }

        private void Resizeing_Form(object sender, MouseEventArgs e)
        {
            if (resizeInProcess)
            {
                Grid senderRect = sender as Grid;
                if (senderRect != null)
                {
                    double width = e.GetPosition(this).X;
                    double height = e.GetPosition(this).Y;
                    senderRect.CaptureMouse();
                    if (senderRect.Name == "ResizeWidth")
                    {
                        width += 1;
                        if (width > 0 && width < this.MaxWidth && width > this.MinWidth)
                        {
                            this.Width = width;
                        }
                    }
                    else if (senderRect.Name == "ResizeHeight")
                    {
                        height += 1;
                        if (height > 0 && height < this.MaxHeight && height > this.MinHeight)
                        {
                            this.Height = height;
                        }
                    }
                }
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

