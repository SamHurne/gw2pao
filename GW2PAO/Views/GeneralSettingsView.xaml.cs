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
using GW2PAO.ViewModels;

namespace GW2PAO.Views
{
    /// <summary>
    /// Interaction logic for GeneralSettingsView.xaml
    /// </summary>
    [Export(typeof(GeneralSettingsView))]
    public partial class GeneralSettingsView : UserControl
    {
        [Import]
        public GeneralSettingsViewModel ViewModel
        {
            get { return this.DataContext as GeneralSettingsViewModel; }
            set { this.DataContext = value; }
        }

        public GeneralSettingsView()
        {
            InitializeComponent();
        }
    }
}
