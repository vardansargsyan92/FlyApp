using System;
using System.Diagnostics;
using PropertyChanged;

namespace FlyApp.ViewModels.Base.Implementation
{
    /// <summary>
    ///     Base class for all bind-able commands.
    /// </summary>
    /// <seealso cref="BaseBindableObject" />
    /// <seealso cref="IBindableCommand" />
    [AddINotifyPropertyChangedInterface]
    public abstract class BaseBindableCommand : BaseBindableObject, IBindableCommand
    {
        private bool _canExecute;

        protected BaseBindableCommand(bool canExecute = true)
        {
            _canExecute = canExecute;
            IsExecutable = canExecute;
        }

        protected string CommandName => GetType().Name;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public abstract void Execute(object parameter);

        public bool IsExecutable { get; private set; }

        /// <summary>
        ///     Sets the executable state of the command, and also <see cref="IsExecutable" /> property. In case there are
        ///     <see cref="CanExecuteChanged" /> subscribers
        ///     will fire the change event.
        /// </summary>
        /// <param name="canExecute">if set to <c>true</c> command will be marked as executable.</param>
        protected void SetCanExecute(bool canExecute)
        {
            _canExecute = IsExecutable = canExecute;
            // ISSUE: reference to a compiler-generated field
            var canExecuteChanged = CanExecuteChanged;
            if (canExecuteChanged == null) return;

            var empty = EventArgs.Empty;
            canExecuteChanged(this, empty);
        }

        protected void LogParameterMismatchError(Type expectedType, object parameter)
        {
            Debug.WriteLine("{Command} expected {Type} parameter but received {@Parameter}", CommandName,
                expectedType.Name, parameter);
        }

        protected void LogValidationError(IViewModelValidator validator)
        {
            Debug.WriteLine("Cancelling {Command} execution due to {ValidationErrors}", CommandName,
                validator.GetAllErrorsInString());
        }
    }
}