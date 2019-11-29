using System;
using System.Windows.Input;

namespace FlyApp.ViewModels.Base.Implementation
{
    public abstract class Command : ICommand
    {
        private bool _canExecute;

        public event EventHandler CanExecuteChanged;

        protected Command(bool canExecute = true)
        {
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public abstract void Execute(object parameter);

        protected void SetCanExecute(bool canExecute)
        {
            _canExecute = canExecute;
            // ISSUE: reference to a compiler-generated field
            EventHandler canExecuteChanged = CanExecuteChanged;
            if(canExecuteChanged == null)
            {
                return;
            }

            EventArgs empty = EventArgs.Empty;
            canExecuteChanged(this, empty);
        }
    }
}