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
        public EventSettingsView EventSettings { get; private set; }

        [Import]
        public DungeonSettingsView DungeonSettings { get; private set; }

        [Import]
        public CommerceSettingsView CommerceSettings { get; private set; }

        [Import]
        public WvWSettingsView WvWSettings { get; private set; }

        [Import]
        public HotkeySettingsView HotkeySettings { get; private set; }

        [Import]
        public GeneralSettingsView GeneralSettings { get; private set; }

        protected override bool NeverClickThrough
        {
            get
            {
                return true;
            }
        }

        public override bool SupportsAutoHide { get { return false; } }

        public SettingsView()
        {
            InitializeComponent();
            this.CenterWindowOnScreen();
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

        /// <summary>
        /// Centers the window on the screen
        /// </summary>
        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.Left = (screenWidth / 2) - (this.Width / 2);
            this.Top = (screenHeight / 2) - (this.Height / 2);
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
            e.Handled = true;
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
