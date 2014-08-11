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
using GW2PAO.ViewModels.DungeonTracker;
using GW2PAO.ViewModels.TradingPost;
using NLog;

namespace GW2PAO.Views.TradingPost
{
    /// <summary>
    /// Interaction logic for DungeonTrackerView.xaml
    /// </summary>
    public partial class TPCalculatorView : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Default constructor
        /// </summary>
        public TPCalculatorView()
        {
            logger.Debug("New TPCalculatorView created");
            InitializeComponent();

            // Set the window size and location
            // TODO
            //this.Closing += DungeonTrackerView_Closing;
            //if (Properties.Settings.Default.DungeonTrackerHeight > 0)
            //    this.Height = Properties.Settings.Default.DungeonTrackerHeight;
            //if (Properties.Settings.Default.DungeonTrackerWidth > 0)
            //    this.Width = Properties.Settings.Default.DungeonTrackerWidth;
            //this.Left = Properties.Settings.Default.DungeonTrackerX;
            //this.Top = Properties.Settings.Default.DungeonTrackerY;
        }

        private void DungeonTrackerView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                // TODO
                //Properties.Settings.Default.DungeonTrackerHeight = this.Height;
                //Properties.Settings.Default.DungeonTrackerWidth = this.Width;
                //Properties.Settings.Default.DungeonTrackerX = this.Left;
                //Properties.Settings.Default.DungeonTrackerY = this.Top;
                //Properties.Settings.Default.Save();
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

        private void MinimizeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
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

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void TextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }
    }
}
