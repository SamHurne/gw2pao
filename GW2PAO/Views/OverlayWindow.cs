using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Blue.Private.Win32Imports;
using Blue.Windows;
using GW2PAO.Utility;

namespace GW2PAO.Views
{
    public class OverlayWindow : Window
    {
        /// <summary>
        /// Static owner window that all overlay windows live under. This reduces the amount of entries in the taskbar and in the alt-tab menu of windows
        /// </summary>
        public static Window OwnerWindow { get; set; }

        /// <summary>
        /// Set to true to never allow click-through, else false
        /// </summary>
        protected virtual bool NeverClickThrough { get { return false; } }

        /// <summary>
        /// Dependency property for IsClickthrough
        /// True if this window has click-through enabled, else false
        /// </summary>
        public static readonly DependencyProperty IsClickthroughProperty =
            DependencyProperty.Register("IsClickthrough",
            typeof(bool),
            typeof(OverlayWindow),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(IsClickthroughPropertyChanged)));

        /// <summary>
        /// True if this window has click-through enabled, else false
        /// </summary>
        public bool IsClickthrough
        {
            get { return (bool)GetValue(IsClickthroughProperty); }
            set { SetValue(IsClickthroughProperty, value); }
        }

        /// <summary>
        /// StickyWindow helper object
        /// </summary>
        private StickyWindow stickyWindow;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OverlayWindow()
        {
            this.Owner = OwnerWindow;
            this.Loaded += OverlayWindowBase_Loaded;
        }

        /// <summary>
        /// Method that handles making sure the overlay window is always shown
        /// </summary>
        private void TopMostThread()
        {
            while (this.IsVisible)
            {
                Threading.BeginInvokeOnUI(() => User32.SetTopMost(this));
                System.Threading.Thread.Sleep(10000);
            }
        }

        /// <summary>
        /// Event handler for the window Loaded event
        /// </summary>
        private void OverlayWindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            // For sticky window support
            this.stickyWindow = new StickyWindow(this);
            this.stickyWindow.StickGap = 10;
            this.stickyWindow.StickToScreen = true;
            this.stickyWindow.StickToOther = true;
            this.stickyWindow.StickOnResize = true;
            this.stickyWindow.StickOnMove = true;
            this.LocationChanged += OverlayWindowBase_LocationChanged;

            // Set up the click through binding
            if (!this.NeverClickThrough)
            {
                this.IsClickthrough = GW2PAO.Properties.Settings.Default.IsClickthroughEnabled;
                Binding clickthroughBinding = new Binding("IsClickthroughEnabled")
                    {
                        Source = GW2PAO.Properties.Settings.Default,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };
                BindingOperations.SetBinding(this, OverlayWindow.IsClickthroughProperty, clickthroughBinding);
            }

            // To make a window truely top-most, we have to periodically set the window as top-most using a User32 call
            // So, to do this, we'll create a thread to do it periodically, as long as the window isn't closed
            Task.Factory.StartNew(this.TopMostThread, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Event handler for the Location Changed event
        /// </summary>
        private void OverlayWindowBase_LocationChanged(object sender, EventArgs e)
        {
            if (this.IsMouseOver)
            {
                System.Windows.Point MousePoint = Mouse.GetPosition(this);
                System.Windows.Point ScreenPoint = this.PointToScreen(MousePoint);

                Win32.SendMessage(this.stickyWindow.Handle, Win32.WM.WM_NCLBUTTONDOWN, Win32.HT.HTCAPTION, Win32.MakeLParam(Convert.ToInt32(ScreenPoint.X), Convert.ToInt32(ScreenPoint.Y)));
                Win32.SendMessage(this.stickyWindow.Handle, Win32.WM.WM_MOUSEMOVE, Win32.HT.HTCAPTION, Win32.MakeLParam(Convert.ToInt32(MousePoint.X), Convert.ToInt32(MousePoint.Y)));
            }
        }

        /// <summary>
        /// Property Changed event handler for the IsClickthrough property
        /// </summary>
        private static void IsClickthroughPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            bool isClickThrough = (bool)e.NewValue;
            OverlayWindow window = (OverlayWindow)source;

            if (isClickThrough && !window.NeverClickThrough)
            {
                User32.SetWindowExTransparent(window, true);
            }
            else
            {
                User32.SetWindowExTransparent(window, false);
            }
        }
    }
}
