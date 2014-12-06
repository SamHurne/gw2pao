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
using GW2PAO.Infrastructure;
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

        [ImportingConstructor]
        public ShellView(ShellViewModel vm, EventAggregator eventAggregator)
        {
            this.DataContext = vm;
            this.eventAggregator = eventAggregator;

            // Register all windows show/hide hotkey
            HotkeyCommands.ToggleAllWindowsCommand.RegisterCommand(new DelegateCommand(this.ToggleVisibility));

            // All overlay windows created will be children of this window
            OverlayWindow.OwnerWindow = this;

            InitializeComponent();

            this.Left = Properties.Settings.Default.OverlayIconX;
            this.Top = Properties.Settings.Default.OverlayIconY;

            this.Loaded += ShellView_Loaded;

            Commands.ApplicationShutdownCommand.RegisterCommand(new DelegateCommand(this.CleanupTrayIcon));

            this.eventAggregator.GetEvent<InsufficientPrivilegesEvent>().Subscribe((o) =>
                {
                    this.TrayIcon.ShowBalloonTip(Properties.Resources.Warning, Properties.Resources.NotRunningAsAdmin, BalloonIcon.Warning);
                });

            this.Closing += ShellView_Closing;
        }

        private void ShellView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.FirstTimeRun)
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
                // TODO: This probably belongs in a view model rather than here in the view...
                GW2PAO.Properties.Settings.Default.IsClickthroughEnabled = !GW2PAO.Properties.Settings.Default.IsClickthroughEnabled;
                GW2PAO.Properties.Settings.Default.Save();

                e.Handled = true;
            }
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            this.NowRunningPopup.StaysOpen = false;
            this.NowRunningPopup.IsOpen = false;
        }

        private void ShellView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                Properties.Settings.Default.OverlayIconX = this.Left;
                Properties.Settings.Default.OverlayIconY = this.Top;
                Properties.Settings.Default.Save();
            }
        }

        private void ToggleVisibility()
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
        }
    }
}
