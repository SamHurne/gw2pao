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
using GW2PAO.Modules.Events.Interfaces;
using GW2PAO.Modules.Events.ViewModels.EventNotification;
using GW2PAO.Views;
using NLog;

namespace GW2PAO.Modules.Events.Views.EventNotification
{
    /// <summary>
    /// Interaction logic for EventNotificationWindow.xaml
    /// </summary>
    public partial class EventNotificationWindow : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// View model
        /// </summary>
        [Import]
        public EventNotificationsWindowViewModel ViewModel
        {
            get
            {
                return this.DataContext as EventNotificationsWindowViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public EventNotificationWindow()
        {
            InitializeComponent();
            this.Loaded += (o, e) =>
            {
                Utility.User32.HideFromTaskbar(this);
                this.LoadWindowLocation();
            };
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            e.Handled = true;
            Properties.Settings.Default.EventNotificationX = this.Left;
            Properties.Settings.Default.EventNotificationY = this.Top;
            Properties.Settings.Default.Save();
        }

        private void LoadWindowLocation()
        {
            // Set the window location
            if (Properties.Settings.Default.EventNotificationX == -1
                && Properties.Settings.Default.EventNotificationY == -1)
            {
                // Use default location (bottom-right corner)
                this.Left = System.Windows.SystemParameters.WorkArea.Width - 5 - this.ActualWidth;
                this.Top = System.Windows.SystemParameters.WorkArea.Height - 5 - this.ActualHeight;
            }
            else
            {
                // Use saved location
                this.Left = Properties.Settings.Default.EventNotificationX;
                this.Top = Properties.Settings.Default.EventNotificationY;
            }
        }
    }
}
