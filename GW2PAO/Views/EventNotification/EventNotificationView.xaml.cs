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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GW2PAO.ViewModels;
using GW2PAO.ViewModels.EventNotification;
using NLog;

namespace GW2PAO.Views.EventNotification
{
    /// <summary>
    /// Interaction logic for EventNotificationView.xaml
    /// </summary>
    public partial class EventNotificationView : UserControl
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Default constructor
        /// </summary>
        public EventNotificationView()
        {
            InitializeComponent();
            
            this.Opacity = 0; // Faded in when loaded

            this.IsVisibleChanged += EventNotificationView_IsVisibleChanged;
        }

        private void EventNotificationView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // TODO: Currently, the fade-out does not work because the item is already removed from rendering by the time this is called
            // TODO: Instead of a fade, it'd be awesome to instead have this slide in from the nearest screen edge
            if (this.Visibility == Visibility.Visible)
            {
                var anim = new DoubleAnimation(1, (Duration)TimeSpan.FromSeconds(0.25));
                this.BeginAnimation(UIElement.OpacityProperty, anim);
            }
            else
            {
                var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.25));
                this.BeginAnimation(UIElement.OpacityProperty, anim);
            }
        }
    }
}
