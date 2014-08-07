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
using GW2PAO.ViewModels.EventNotification;
using GW2PAO.ViewModels.WvWNotification;
using NLog;

namespace GW2PAO.Views.WvWNotification
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
        /// The WvW controller
        /// </summary>
        private IWvWController controller;

        /// <summary>
        /// View model object
        /// </summary>
        private WvWNotificationsWindowViewModel viewModel;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="controller">The events controller</param>
        public WvWNotificationWindow(IWvWController controller)
        {
            this.controller = controller;
            this.viewModel = new WvWNotificationsWindowViewModel(this.controller);
            this.DataContext = this.viewModel;
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
