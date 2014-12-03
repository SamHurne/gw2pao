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
using GW2PAO.Modules.WvW.ViewModels.WvWNotification;
using GW2PAO.Views;
using NLog;

namespace GW2PAO.Modules.WvW.Views.WvWNotification
{
    /// <summary>
    /// Interaction logic for EventNotificationWindow.xaml
    /// </summary>
    public partial class WvWNotificationWindow : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// View model
        /// </summary>
        [Import]
        public WvWNotificationsWindowViewModel ViewModel
        {
            get
            {
                return this.DataContext as WvWNotificationsWindowViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        /// <summary>
        /// Notification windows are never sticky
        /// </summary>
        protected override bool NeverSticky
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="controller">The events controller</param>
        public WvWNotificationWindow()
        {
            InitializeComponent();
            this.Loaded += (o, e) => this.LoadWindowLocation();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            Properties.Settings.Default.WvWNotificationX = this.Left;
            Properties.Settings.Default.WvWNotificationY = this.Top;
            Properties.Settings.Default.Save();
        }

        private void LoadWindowLocation()
        {
            // Set the window location
            if (Properties.Settings.Default.WvWNotificationX == -1
                && Properties.Settings.Default.WvWNotificationY == -1)
            {
                // Use default location (bottom-right corner, without being on top of event notifications)
                this.Left = System.Windows.SystemParameters.WorkArea.Width - 5 - this.ActualWidth;
                this.Top = System.Windows.SystemParameters.WorkArea.Height - 5 - (this.ActualHeight * 2);
            }
            else
            {
                // Use saved location
                this.Left = Properties.Settings.Default.WvWNotificationX;
                this.Top = Properties.Settings.Default.WvWNotificationY;
            }
        }
    }
}
