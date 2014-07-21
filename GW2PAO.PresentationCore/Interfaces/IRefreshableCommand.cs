using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GW2PAO.PresentationCore.Interfaces
{
    public interface IRefreshableCommand : ICommand
    {
        void RefreshCanExecuteChanged();
    }
}
