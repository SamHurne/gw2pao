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
using GW2PAO.Controllers.Interfaces;
using GW2PAO.ViewModels.Teamspeak;
using NLog;

namespace GW2PAO.Views.Teamspeak
{
    /// <summary>
    /// Interaction logic for TeamspeakView.xaml
    /// </summary>
    public partial class TeamspeakView : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private const double minHeight = 84;
        private const double maxHeight = 500;
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
        /// Dungeon tracker view model
        /// </summary>
        private TeamspeakViewModel viewModel;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dungeonsController">The dungeons controller</param>
        public TeamspeakView(TeamspeakViewModel vm)
        {
            logger.Debug("New TeamspeakView created");
            this.viewModel = vm;
            this.DataContext = this.viewModel;
            InitializeComponent();

            // Set the window size and location
            this.MinHeight = minHeight;
            this.MaxHeight = maxHeight;
            this.Height = initialHeight;

            this.Closing += TeamspeakView_Closing;
            if (Properties.Settings.Default.TeamspeakHeight > 0)
                this.Height = Properties.Settings.Default.TeamspeakHeight;
            if (Properties.Settings.Default.TeamspeakWidth > 0)
                this.Width = Properties.Settings.Default.TeamspeakWidth;
            this.Left = Properties.Settings.Default.TeamspeakX;
            this.Top = Properties.Settings.Default.TeamspeakY;

            this.beforeCollapseHeight = this.Height;
        }

        private void TeamspeakView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                Properties.Settings.Default.TeamspeakHeight = this.Height;
                Properties.Settings.Default.TeamspeakWidth = this.Width;
                Properties.Settings.Default.TeamspeakX = this.Left;
                Properties.Settings.Default.TeamspeakY = this.Top;
                Properties.Settings.Default.Save();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Shutdown the teamspeak viewmodel when the teamspeak window is closed
            this.viewModel.Shutdown();
            logger.Debug("TeamspeakView closed");
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
            if (this.NotificationsContainer.Visibility == System.Windows.Visibility.Visible)
            {
                this.beforeCollapseHeight = this.Height;
                this.MinHeight = this.TitleBar.ActualHeight;
                this.MaxHeight = this.TitleBar.ActualHeight;
                this.Height = this.TitleBar.ActualHeight;
                this.NotificationsContainer.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.MinHeight = minHeight;
                this.MaxHeight = maxHeight;
                this.Height = this.beforeCollapseHeight;
                this.NotificationsContainer.Visibility = System.Windows.Visibility.Visible;
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

        private void MessageEntryBox_KeyDown(object sender, KeyEventArgs e)
        {
            // if user didn't press Enter, do nothing
            if (!e.Key.Equals(Key.Enter))
                return;

            // User pressed enter - execute the send command
            this.viewModel.SendMessageCommand.Execute(null);
        }
    }
}
