using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using GW2PAO.Infrastructure;
using GW2PAO.Properties;
using GW2PAO.Utility;
using GW2PAO.ViewModels;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;

namespace GW2PAO.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    [Export]
    public partial class ShellView : OverlayWindow
    {
        private EventAggregator eventAggregator;
        private ProcessMonitor processMonitor;

        /// <summary>
        /// This window can never have click-through
        /// </summary>
        protected override bool NeverClickThrough
        {
            get
            {
                return true;
            }
        }

        protected override bool SetNoFocus
        {
            get
            {
                return false;
            }
        }

        [ImportingConstructor]
        public ShellView(ShellViewModel vm, EventAggregator eventAggregator, ProcessMonitor processMonitor)
        {
            this.DataContext = vm;
            this.eventAggregator = eventAggregator;
            this.processMonitor = processMonitor;

            // Register all windows show/hide hotkey
            HotkeyCommands.ToggleAllWindowsCommand.RegisterCommand(new DelegateCommand(this.ToggleVisibility));

            // Register for events that could make us show/hide all windows
            Properties.Settings.Default.PropertyChanged += this.OnSettingsPropertyChanged;
            this.eventAggregator.GetEvent<GW2ProcessStarted>().Subscribe(o => 
                {
                    if (Settings.Default.AutoHideAllWindowsWhenGw2NotRunning)
                        this.SetAllOverlayWindowsVisibility(Visibility.Visible);
                });
            this.eventAggregator.GetEvent<GW2ProcessClosed>().Subscribe(o =>
                {
                    if (Settings.Default.AutoHideAllWindowsWhenGw2NotRunning)
                        this.SetAllOverlayWindowsVisibility(Visibility.Hidden);
                });
            this.eventAggregator.GetEvent<GW2ProcessFocused>().Subscribe(o =>
                {
                    if (Settings.Default.AutoHideAllWindowsWhenGw2LosesFocus)
                        this.SetAllOverlayWindowsVisibility(Visibility.Visible);
                });
            this.eventAggregator.GetEvent<GW2ProcessLostFocus>().Subscribe(o =>
                {
                    if (Settings.Default.AutoHideAllWindowsWhenGw2LosesFocus)
                        this.SetAllOverlayWindowsVisibility(Visibility.Hidden);
                });

            // All overlay windows created will be children of this window
            OverlayWindow.OwnerWindow = this;

            InitializeComponent();

            this.Left = Properties.Settings.Default.OverlayIconX;
            this.Top = Properties.Settings.Default.OverlayIconY;

            this.Loaded += ShellView_Loaded;

            Commands.ApplicationShutdownCommand.RegisterCommand(new DelegateCommand(this.CleanupTrayIcon));
            Commands.CleanupTrayIcon.RegisterCommand(new DelegateCommand(this.CleanupTrayIcon));

            this.eventAggregator.GetEvent<InsufficientPrivilegesEvent>().Subscribe((o) =>
                {
                    this.TrayIcon.ShowBalloonTip(Properties.Resources.Warning, Properties.Resources.NotRunningAsAdmin, BalloonIcon.Warning);
                });

            this.Closing += ShellView_Closing;
        }

        private void ShellView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.FirstTimeRun
                && ((ShellViewModel)this.DataContext).IsOverlayMenuIconVisible)
            {
                Task.Factory.StartNew(() =>
                {
                    System.Threading.Thread.Sleep(500);
                    this.Dispatcher.Invoke(() => this.NowRunningPopup.IsOpen = true);
                });
            }
        }

        private void CleanupTrayIcon()
        {
            Threading.InvokeOnUI(() => this.TrayIcon.Dispose());
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    this.DragMove();
                    e.Handled = true;
                }
                else
                {
                    Image image = sender as Image;
                    ContextMenu contextMenu = image.ContextMenu;
                    contextMenu.PlacementTarget = image;
                    contextMenu.Visibility = System.Windows.Visibility.Visible;
                    contextMenu.IsOpen = true;
                    e.Handled = true;
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                Image image = sender as Image;
                ContextMenu contextMenu = image.ContextMenu;
                contextMenu.Visibility = System.Windows.Visibility.Collapsed;
                contextMenu.IsOpen = false;

                // Toggle global click-through
                HotkeyCommands.ToggleInteractiveWindowsCommand.Execute(null);

                e.Handled = true;
            }
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            this.NowRunningPopup.StaysOpen = false;
            this.NowRunningPopup.IsOpen = false;
        }

        private void ShellView_Closing(object sender, CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                Properties.Settings.Default.OverlayIconX = this.Left;
                Properties.Settings.Default.OverlayIconY = this.Top;
                Properties.Settings.Default.Save();
            }
        }

        private void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ReflectionUtility.GetPropertyName(() => Settings.Default.AutoHideAllWindowsWhenGw2NotRunning))
            {
                if (this.processMonitor.IsGw2Running)
                    this.SetAllOverlayWindowsVisibility(Visibility.Visible);
                else
                    this.SetAllOverlayWindowsVisibility(Visibility.Hidden);
            }
            else if (e.PropertyName == ReflectionUtility.GetPropertyName(() => Settings.Default.AutoHideAllWindowsWhenGw2LosesFocus))
            {
                if (this.processMonitor.DoesGw2HaveFocus)
                    this.SetAllOverlayWindowsVisibility(Visibility.Hidden);
                else
                    this.SetAllOverlayWindowsVisibility(Visibility.Hidden);
            }
        }

        private void ToggleVisibility()
        {
            Threading.InvokeOnUI(() =>
                {
                    foreach (Window window in this.OwnedWindows)
                    {
                        if (window != null)
                        {
                            if (window.IsVisible)
                                window.Hide();
                            else
                                window.Show();
                        }
                    }
                });
        }

        private void SetAllOverlayWindowsVisibility(Visibility visibility)
        {
            Threading.InvokeOnUI(() =>
                {
                    foreach (Window window in this.OwnedWindows)
                    {
                        if (window is OverlayWindow && ((OverlayWindow)window).SupportsAutoHide)
                        {
                            window.Visibility = visibility;
                        }
                    }
                });
        }
    }
}
