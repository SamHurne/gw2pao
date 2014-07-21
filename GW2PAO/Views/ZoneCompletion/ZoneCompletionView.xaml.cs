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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Blue.Private.Win32Imports;
using Blue.Windows;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.ViewModels.Interfaces;
using GW2PAO.ViewModels.ZoneCompletion;
using NLog;

namespace GW2PAO.Views.ZoneCompletion
{
    /// <summary>
    /// Interaction logic for ZoneCompletionView.xaml
    /// </summary>
    public partial class ZoneCompletionView : Window
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// StickyWindow helper object
        /// </summary>
        private StickyWindow stickyWindow;

        /// <summary>
        /// True if the user is resizing the window, else false
        /// </summary>
        private bool resizeInProcess = false;

        /// <summary>
        /// The zone completion controller
        /// </summary>
        private IZoneCompletionController controller;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="zoneItemsController">The zone completion controller</param>
        /// <param name="zoneName">The zone name view model</param>
        public ZoneCompletionView(IZoneCompletionController zoneItemsController, IHasZoneName zoneName)
        {
            logger.Debug("New ZoneCompletionView created");
            this.controller = zoneItemsController;
            this.DataContext = new ZoneCompletionViewModel(this.controller, zoneName);
            InitializeComponent();

            // Set the window size and location
            this.Closing += ZoneCompletionView_Closing;
            if (Properties.Settings.Default.ZoneAssistantHeight > 0)
                this.Height = Properties.Settings.Default.ZoneAssistantHeight;
            if (Properties.Settings.Default.ZoneAssistantWidth > 0)
                this.Width = Properties.Settings.Default.ZoneAssistantWidth;
            this.Left = Properties.Settings.Default.ZoneAssistantX;
            this.Top = Properties.Settings.Default.ZoneAssistantY;

            // For sticky window support
            this.Loaded += (o, e) =>
            {
                this.stickyWindow = new StickyWindow(this);
                this.stickyWindow.StickGap = 10;
                this.stickyWindow.StickToScreen = true;
                this.stickyWindow.StickToOther = true;
                this.stickyWindow.StickOnResize = true;
                this.stickyWindow.StickOnMove = true;
            };
            this.LocationChanged += (o, e) =>
            {
                System.Windows.Point MousePoint = Mouse.GetPosition(this);
                System.Windows.Point ScreenPoint = this.PointToScreen(MousePoint);

                Win32.SendMessage(this.stickyWindow.Handle, Win32.WM.WM_NCLBUTTONDOWN, Win32.HT.HTCAPTION, Win32.MakeLParam(Convert.ToInt32(ScreenPoint.X), Convert.ToInt32(ScreenPoint.Y)));
                Win32.SendMessage(this.stickyWindow.Handle, Win32.WM.WM_MOUSEMOVE, Win32.HT.HTCAPTION, Win32.MakeLParam(Convert.ToInt32(MousePoint.X), Convert.ToInt32(MousePoint.Y)));
            };
        }

        private void ZoneCompletionView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                Properties.Settings.Default.ZoneAssistantHeight = this.Height;
                Properties.Settings.Default.ZoneAssistantWidth = this.Width;
                Properties.Settings.Default.ZoneAssistantX = this.Left;
                Properties.Settings.Default.ZoneAssistantY = this.Top;
                Properties.Settings.Default.Save();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.controller.Stop();
            logger.Debug("ZoneCompletionView closed");
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
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
