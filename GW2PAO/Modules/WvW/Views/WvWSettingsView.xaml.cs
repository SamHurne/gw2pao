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
using GW2PAO.Modules.WvW.ViewModels;

namespace GW2PAO.Modules.WvW.Views
{
    /// <summary>
    /// Interaction logic for WvWSettingsView.xaml
    /// </summary>
    [Export(typeof(WvWSettingsView))]
    public partial class WvWSettingsView : UserControl
    {
        [Import]
        public WvWSettingsViewModel ViewModel
        {
            get { return this.DataContext as WvWSettingsViewModel; }
            set { this.DataContext = value; }
        }

        public WvWSettingsView()
        {
            InitializeComponent();
        }
    }
}
