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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GW2PAO.Modules.Dungeons.ViewModels;

namespace GW2PAO.Modules.Dungeons.Views
{
    /// <summary>
    /// Interaction logic for DungeonSettingsView.xaml
    /// </summary>
    [Export(typeof(DungeonSettingsView))]
    public partial class DungeonSettingsView : UserControl
    {
        [Import]
        public DungeonSettingsViewModel ViewModel
        {
            get { return this.DataContext as DungeonSettingsViewModel; }
            set { this.DataContext = value; }
        }

        public DungeonSettingsView()
        {
            InitializeComponent();
        }
    }
}
