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
using GW2PAO.ViewModels.Commerce.PriceNotification;
using NLog;

namespace GW2PAO.Views.Commerce.PriceNotification
{
    /// <summary>
    /// Interaction logic for EventNotificationWindow.xaml
    /// </summary>
    public partial class PriceNotificationWindow : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The commerce controller
        /// </summary>
        private ICommerceController controller;

        /// <summary>
        /// View model object
        /// </summary>
        private PriceNotificationsViewModel viewModel;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="controller">The commerce controller</param>
        public PriceNotificationWindow(ICommerceController controller)
        {
            this.controller = controller;
            this.viewModel = new PriceNotificationsViewModel(this.controller);
            this.DataContext = this.viewModel;
            InitializeComponent();
            this.Loaded += (o, e) => this.LoadWindowLocation();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            Properties.Settings.Default.PriceNotificationX = this.Left;
            Properties.Settings.Default.PriceNotificationY = this.Top;
            Properties.Settings.Default.Save();
        }

        private void LoadWindowLocation()
        {
            // Set the window location
            if (Properties.Settings.Default.PriceNotificationX == -1
                && Properties.Settings.Default.PriceNotificationY == -1)
            {
                // Use default location (bottom-right corner)
                this.Left = 0;
                this.Top = 200;
            }
            else
            {
                // Use saved location
                this.Left = Properties.Settings.Default.PriceNotificationX;
                this.Top = Properties.Settings.Default.PriceNotificationY;
            }
        }
    }
}
