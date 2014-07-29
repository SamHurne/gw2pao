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
using NLog;

namespace GW2PAO.Views.EventNotification
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
        /// The events controller
        /// </summary>
        private IEventsController controller;

        /// <summary>
        /// View model object
        /// </summary>
        private EventNotificationsWindowViewModel viewModel;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="controller">The events controller</param>
        public EventNotificationWindow(IEventsController controller)
        {
            this.controller = controller;
            this.viewModel = new EventNotificationsWindowViewModel(this.controller);
            this.DataContext = this.viewModel;
            InitializeComponent();
            this.Loaded += (o, e) => this.LoadWindowLocation();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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
