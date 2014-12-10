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
using GW2PAO.ViewModels;

namespace GW2PAO.Views
{
    /// <summary>
    /// Interaction logic for RestartPromptView.xaml
    /// </summary>
    public partial class RestartPromptView : OverlayWindow
    {
        [Import]
        public RestartPromptViewModel ViewModel
        {
            get { return this.DataContext as RestartPromptViewModel; }
            set { this.DataContext = value; }
        }

        public RestartPromptView()
        {
            InitializeComponent();
            this.CenterWindowOnScreen();
        }

        /// <summary>
        /// Centers the window on the screen
        /// </summary>
        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.Left = (screenWidth / 2) - (this.Width / 2);
            this.Top = (screenHeight / 2) - (this.MinHeight / 2);
        }

        private void LaterButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
