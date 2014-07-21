using GW2PAO.PresentationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GW2PAO.PresentationCore
{
    public class DelegateCommand : IRefreshableCommand
    {
        private readonly Action action;
        private readonly Func<bool> canExecute;

        public DelegateCommand(Action action)
        {
            this.action = action;
            this.canExecute = () => true;
        }

        public DelegateCommand(Action action, Func<bool> canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (this.action != null)
                this.action();
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute();
        }

        public void RefreshCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(this, new EventArgs());
        }
    }

    public class DelegateCommand<T> : IRefreshableCommand
    {
        private readonly Action<T> action;
        private readonly Func<T, bool> canExecute;

        public DelegateCommand(Action<T> action)
        {
            this.action = action;
            this.canExecute = (e) => true;
        }

        public DelegateCommand(Action<T> action, Func<T, bool> canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (this.action != null)
                this.action((T)parameter);
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute((T)parameter);
        }

        public void RefreshCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(this, new EventArgs());
        }
    }
}
