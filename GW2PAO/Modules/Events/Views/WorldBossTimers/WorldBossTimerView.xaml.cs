using System.Windows.Controls;
using System.Windows.Input;

namespace GW2PAO.Views.Events.WorldBossTimers
{
    /// <summary>
    /// Interaction logic for WorldBossTimerView.xaml
    /// </summary>
    public partial class WorldBossTimerView : UserControl
    {
        public WorldBossTimerView()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                ((GW2PAO.Modules.Events.ViewModels.WorldBossTimers.WorldBossEventViewModel)this.DataContext).CopyDataCommand.Execute(null);
            }
        }
    }
}
