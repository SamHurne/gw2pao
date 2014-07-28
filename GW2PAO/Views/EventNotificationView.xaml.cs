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
using GW2PAO.ViewModels;

namespace GW2PAO.Views
{
    /// <summary>
    /// Interaction logic for EventNotificationView.xaml
    /// </summary>
    public partial class EventNotificationView : UserControl
    {
        public EventNotificationView(EventViewModel vm)
        {
            this.DataContext = vm;
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            App.TrayIcon.CloseCustomNotification();
        }
    }
}
