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
using GW2PAO.Modules.Dungeons.ViewModels.DungeonTimer;
using GW2PAO.Views;

namespace GW2PAO.Modules.Dungeons.Views.DungeonTimer
{
    /// <summary>
    /// Interaction logic for DungeonTimerView.xaml
    /// </summary>
    public partial class DungeonTimerView : OverlayWindow
    {
        [Import]
        public DungeonTimerViewModel ViewModel
        {
            get { return this.DataContext as DungeonTimerViewModel; }
            set { this.DataContext = value; }
        }

        /// <summary>
        /// Height before collapsing the control
        /// </summary>
        private double beforeCollapseHeight;

        private const double MIN_WIDTH= 150;
        private const double MIN_HEIGHT = 110;

        public DungeonTimerView()
        {
            InitializeComponent();

            this.ResizeHelper.InitializeResizeElements(null, null, this.ResizeGripper);
            this.MinHeight = MIN_HEIGHT;
            this.MinWidth = MIN_WIDTH;
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            e.Handled = true;
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

        private void CollapseExpandButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.CurrentTimePanel.Visibility == System.Windows.Visibility.Visible)
            {
                this.beforeCollapseHeight = this.Height;
                this.MinHeight = this.TitleBar.ActualHeight;
                this.MaxHeight = this.TitleBar.ActualHeight;
                this.Height = this.TitleBar.ActualHeight;
                this.CurrentTimePanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.CurrentTimePanel.Visibility = System.Windows.Visibility.Visible;
                this.MinHeight = MIN_HEIGHT;
                this.MaxHeight = double.PositiveInfinity;
                this.Height = this.beforeCollapseHeight;
            }
        }

        private void OverlayWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                if (this.CurrentTimePanel.Visibility == System.Windows.Visibility.Visible)
                {
                    Properties.Settings.Default.DungeonTimerHeight = this.Height;
                }
                else
                {
                    Properties.Settings.Default.DungeonTimerHeight = this.beforeCollapseHeight;
                }
                Properties.Settings.Default.DungeonTimerWidth = this.Width;
                Properties.Settings.Default.DungeonTimerX = this.Left;
                Properties.Settings.Default.DungeonTimerY = this.Top;
                Properties.Settings.Default.Save();
            }
        }
    }
}
