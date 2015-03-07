using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for NewVersionNotificationView.xaml
    /// </summary>
    public partial class NewVersionNotificationView : OverlayWindow
    {
        private NewVersionNotificationViewModel viewModel;

        public NewVersionNotificationView(NewVersionNotificationViewModel vm)
        {
            this.viewModel = vm;
            this.DataContext = this.viewModel;
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
            this.Top = (screenHeight / 2) - (this.Height / 2);
        }

        private void Link_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.viewModel.OpenDownloadPageCommad.Execute(null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            e.Handled = true;
        }
    }
}
