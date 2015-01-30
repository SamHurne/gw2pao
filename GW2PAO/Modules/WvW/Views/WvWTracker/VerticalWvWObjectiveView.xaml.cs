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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GW2PAO.Modules.WvW.Views.WvWTracker
{
    /// <summary>
    /// Interaction logic for VerticalWvWObjectiveView.xaml
    /// </summary>
    public partial class VerticalWvWObjectiveView : UserControl
    {
        public VerticalWvWObjectiveView()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                ((GW2PAO.Modules.WvW.ViewModels.WvWObjectiveViewModel)this.DataContext).CopyGeneralDataCommand.Execute(null);
            }
        }
    }
}
