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
using GW2PAO.Modules.Teamspeak.ViewModels;
using GW2PAO.Views;
using NLog;

namespace GW2PAO.Modules.Teamspeak.Views
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

        /// <summary>
        /// Height before collapsing the control
        /// </summary>
        private double beforeCollapseHeight;

        /// <summary>
        /// Teamspeak overlay view model
        /// </summary>
        [Import(RequiredCreationPolicy=CreationPolicy.NonShared)]
        public TeamspeakViewModel ViewModel
        {
            get
            {
                return this.DataContext as TeamspeakViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TeamspeakView()
        {
            logger.Debug("New TeamspeakView created");
            InitializeComponent();

            this.ResizeHelper.InitializeResizeElements(this.ResizeHeight, null);

            // Save the height values for use when collapsing the window
            this.MinHeight = minHeight;
            this.MaxHeight = maxHeight;
            this.Height = Properties.Settings.Default.TeamspeakHeight;

            this.Closing += TeamspeakView_Closing;
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
            this.ViewModel.Shutdown();
            logger.Debug("TeamspeakView closed");
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
            this.ViewModel.SendMessageCommand.Execute(null);
        }

        private void ChannelTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.ChannelListPopup.IsOpen = true;
                e.Handled = true;
            }
        }

        private void Channel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var dataContext = ((FrameworkElement)sender).DataContext;
                var vm = dataContext as ChannelViewModel;
                if (vm != null)
                {
                    vm.SelectChannelCommand.Execute(null);
                }
                this.ChannelListPopup.IsOpen = false;
                e.Handled = true;
            }
        }

        private void ChatMessagesScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            this.ChatMessagesScrollViewer.ScrollToBottom();
        }

        private void PeopleIcon_Click(object sender, RoutedEventArgs e)
        {
            this.ClientListPopup.IsOpen = true;
            e.Handled = true;
        }
    }
}
