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
using GW2PAO.Modules.Commerce.ViewModels;

namespace GW2PAO.Modules.Commerce.Views
{
    /// <summary>
    /// Interaction logic for CommerceSettingsView.xaml
    /// </summary>
    [Export(typeof(CommerceSettingsView))]
    public partial class CommerceSettingsView : UserControl
    {
        [Import]
        public CommerceSettingsViewModel ViewModel
        {
            get { return this.DataContext as CommerceSettingsViewModel; }
            set { this.DataContext = value; }
        }

        public CommerceSettingsView()
        {
            InitializeComponent();
        }
    }
}
