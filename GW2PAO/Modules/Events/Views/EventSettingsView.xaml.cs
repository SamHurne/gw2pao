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
using GW2PAO.Modules.Events.ViewModels;

namespace GW2PAO.Modules.Events.Views
{
    /// <summary>
    /// Interaction logic for EventSettings.xaml
    /// </summary>
    [Export(typeof(EventSettingsView))]
    public partial class EventSettingsView : UserControl
    {
        [Import]
        public EventSettingsViewModel ViewModel
        {
            get { return this.DataContext as EventSettingsViewModel; }
            set { this.DataContext = value; }
        }

        public EventSettingsView()
        {
            InitializeComponent();
        }
    }
}
