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

namespace GW2PAO.Modules.Commerce.Views
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

        protected override bool SetNoFocus { get { return false; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TPCalculatorView()
        {
            logger.Debug("New TPCalculatorView created");
            InitializeComponent();

            // Set the window size and location
            this.Closing += TPCalculatorView_Closing;
            this.Left = Properties.Settings.Default.TPCalculatorX;
            this.Top = Properties.Settings.Default.TPCalculatorY;
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
            e.Handled = true;
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CollapseExpandButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Calculator.Visibility == System.Windows.Visibility.Visible)
            {
                this.Calculator.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.Calculator.Visibility = System.Windows.Visibility.Visible;
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

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void TextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var tb = (TextBox)sender;

            if (tb.IsMouseOver)
            {
                int currentVal;
                if (int.TryParse(tb.Text, out currentVal))
                {
                    if (e.Delta > 0)
                    {
                        // Increment
                        tb.Text = (currentVal + 1).ToString();
                    }
                    else
                    {
                        if (currentVal > 0)
                        {
                            // Decrement
                            tb.Text = (currentVal - 1).ToString();
                        }
                    }
                }
            }
        }
    }
}
