using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Modules.Commerce.ViewModels;
using GW2PAO.Views;

namespace GW2PAO.Modules.Commerce.Views
{
    /// <summary>
    /// Interaction logic for NewVersionNotificationView.xaml
    /// </summary>
    public partial class RebuildNamesDatabaseView : OverlayWindow
    {
        /// <summary>
        /// View model
        /// </summary>
        [Import]
        public RebuildNamesDatabaseViewModel ViewModel
        {
            get
            {
                return this.DataContext as RebuildNamesDatabaseViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        public RebuildNamesDatabaseView()
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
            this.Top = (screenHeight / 2) - (this.Height / 2);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ((RebuildNamesDatabaseViewModel)this.DataContext).CancelCommand.Execute(null);
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            e.Handled = true;
        }
    }
}
