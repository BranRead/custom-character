using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Assignment9
{
    public class HotKey : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public HotKey(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _action();
        }

    }
}
