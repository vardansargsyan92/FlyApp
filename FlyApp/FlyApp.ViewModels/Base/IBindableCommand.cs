using System.ComponentModel;
using System.Windows.Input;

namespace FlyApp.ViewModels.Base
{
    /// <summary>
    /// <see cref="IBindableCommand" /> is extension to <see cref="ICommand" /> to allow binding support, which is, of course done through
    /// <see cref="INotifyPropertyChanged" /> interface. Any command, that will require state binding should implement this interface.
    /// </summary>
    /// <seealso cref="System.Windows.Input.ICommand" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public interface IBindableCommand : ICommand, INotifyPropertyChanged
    {
        /// <summary>
        ///  <see cref="IsExecutable"/> reflects the state of <see cref="ICommand.CanExecute"/> method,
        /// and allows to bind to the executable state of the command. 
        /// </summary>
        bool IsExecutable { get; }
    }
}