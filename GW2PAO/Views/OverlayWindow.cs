using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using Blue.Private.Win32Imports;
using Blue.Windows;
using GW2PAO.Utility;
using GW2PAO.Utility.Interfaces;

namespace GW2PAO.Views
{
    public class OverlayWindow : Window
    {
        /// <summary>
        /// Static owner window that all overlay windows live under. This reduces the amount of entries in the taskbar and in the alt-tab menu of windows
        /// </summary>
        public static Window OwnerWindow { get; set; }

        /// <summary>
        /// Process Monitor object that monitors the focus/lost focus state of the GW2 process
        /// </summary>
        public static IProcessMonitor ProcessMonitor { get; set; }

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
        /// Dependency property for IsSticky
        /// True if this window has click-through enabled, else false
        /// </summary>
        public static readonly DependencyProperty IsStickyProperty =
            DependencyProperty.Register("IsSticky",
            typeof(bool),
            typeof(OverlayWindow),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(IsStickyPropertyChanged)));

        /// <summary>
        /// True if this window has click-through enabled, else false
        /// </summary>
        public bool IsClickthrough
        {
            get { return (bool)GetValue(IsClickthroughProperty); }
            set { SetValue(IsClickthroughProperty, value); }
        }

        /// <summary>
        /// True if the window should be sticky (stick to edges, other windows, etc), else false
        /// </summary>
        public bool IsSticky
        {
            get { return (bool)GetValue(IsStickyProperty); }
            set { SetValue(IsStickyProperty, value); }
        }

        /// <summary>
        /// StickyWindow helper object
        /// </summary>
        public StickyWindow StickyHelper { get; private set; }

        /// <summary>
        /// Helper for snapping any resizing
        /// </summary>
        protected ResizeSnapHelper ResizeHelper;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OverlayWindow()
        {
            this.Owner = OwnerWindow;
            this.Loaded += OverlayWindowBase_Loaded;
            this.ResizeHelper = new ResizeSnapHelper(this);

            if (ProcessMonitor != null)
            {
                ProcessMonitor.GW2Focused += (o, e) => Threading.BeginInvokeOnUI(() => User32.SetTopMost(this, true));
            }
        }

        /// <summary>
        /// Event handler for the window Loaded event
        /// </summary>
        private void OverlayWindowBase_Loaded(object sender, RoutedEventArgs e)
        {
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

            // For sticky window support:
            this.StickyHelper = new StickyWindow(this);
            this.StickyHelper.StickGap = 10;
            this.LocationChanged += OverlayWindowBase_LocationChanged;

            this.IsSticky = GW2PAO.Properties.Settings.Default.AreWindowsSticky;
            
            this.StickyHelper.StickToScreen = this.IsSticky;
            this.StickyHelper.StickToOther = this.IsSticky;
            this.StickyHelper.StickOnResize = this.IsSticky;
            this.StickyHelper.StickOnMove = this.IsSticky;

            // Set up the IsSticky binding
            Binding isStickyBinding = new Binding("AreWindowsSticky")
            {
                Source = GW2PAO.Properties.Settings.Default,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(this, OverlayWindow.IsStickyProperty, isStickyBinding);
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

                Win32.SendMessage(this.StickyHelper.Handle, Win32.WM.WM_NCLBUTTONDOWN, Win32.HT.HTCAPTION, Win32.MakeLParam(Convert.ToInt32(ScreenPoint.X), Convert.ToInt32(ScreenPoint.Y)));
                Win32.SendMessage(this.StickyHelper.Handle, Win32.WM.WM_MOUSEMOVE, Win32.HT.HTCAPTION, Win32.MakeLParam(Convert.ToInt32(MousePoint.X), Convert.ToInt32(MousePoint.Y)));
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

        /// <summary>
        /// Property Changed event handler for the IsSticky property
        /// </summary>
        private static void IsStickyPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            bool isSticky = (bool)e.NewValue;
            OverlayWindow window = (OverlayWindow)source;
            window.StickyHelper.StickToScreen = isSticky;
            window.StickyHelper.StickToOther = isSticky;
            window.StickyHelper.StickOnResize = isSticky;
            window.StickyHelper.StickOnMove = isSticky;
        }
    }
}
