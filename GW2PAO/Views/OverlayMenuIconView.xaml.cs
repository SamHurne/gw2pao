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
using GW2PAO.ViewModels.TrayIcon;

namespace GW2PAO.Views
{
    /// <summary>
    /// Interaction logic for OverlayMenuIconView.xaml
    /// </summary>
    public partial class OverlayMenuIconView : OverlayWindow
    {
        public OverlayMenuIconView(TrayIconViewModel trayIconVm)
        {
            this.DataContext = trayIconVm;
            InitializeComponent();
            this.MainMenu.ItemsSource = trayIconVm.MenuItems;

            this.Left = Properties.Settings.Default.OverlayIconX;
            this.Top = Properties.Settings.Default.OverlayIconY;
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
                    contextMenu.IsOpen = true;
                    e.Handled = true;
                }
            }
        }

        private void OverlayMenuIconView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                Properties.Settings.Default.OverlayIconX = this.Left;
                Properties.Settings.Default.OverlayIconY = this.Top;
                Properties.Settings.Default.Save();
            }
        }
    }
}
