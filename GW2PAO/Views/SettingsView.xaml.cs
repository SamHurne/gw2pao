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
using GW2PAO.Modules.Commerce.Views;
using GW2PAO.Modules.Dungeons.Views;
using GW2PAO.Modules.Events.Views;
using GW2PAO.Modules.WvW.Views;

namespace GW2PAO.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : OverlayWindow, IPartImportsSatisfiedNotification
    {
        [Import]
        private EventSettingsView EventSettings { get; set; }

        [Import]
        private DungeonSettingsView DungeonSettings { get; set; }

        [Import]
        private CommerceSettingsView CommerceSettings { get; set; }

        [Import]
        private WvWSettingsView WvWSettings { get; set; }

        [Import]
        private HotkeySettingsView HotkeySettings { get; set; }

        [Import]
        private GeneralSettingsView GeneralSettings { get; set; }

        public SettingsView()
        {
            InitializeComponent();
        }

        public void OnImportsSatisfied()
        {
            List<TabItem> tabItems = new List<TabItem>();
            tabItems.Add(this.CreateTabItem(this.GeneralSettings));
            tabItems.Add(this.CreateTabItem(this.HotkeySettings));
            tabItems.Add(this.CreateTabItem(this.EventSettings));
            tabItems.Add(this.CreateTabItem(this.DungeonSettings));
            tabItems.Add(this.CreateTabItem(this.CommerceSettings));
            tabItems.Add(this.CreateTabItem(this.WvWSettings));
            this.SettingsTabControl.ItemsSource = tabItems;
        }

        private TabItem CreateTabItem(UserControl view)
        {
            TabItem item = new TabItem();
            item.DataContext = view.DataContext;
            item.Content = view;
            return item;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // This prevents Aero snapping
            if (this.ResizeMode != System.Windows.ResizeMode.NoResize)
            {
                this.ResizeMode = System.Windows.ResizeMode.NoResize;
                this.UpdateLayout();
            }

            this.DragMove();
        }

        private void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ResizeMode == System.Windows.ResizeMode.NoResize)
            {
                // Restore resize grips (removed on mouse-down to prevent Aero snapping)
                this.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                this.UpdateLayout();
            }
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
