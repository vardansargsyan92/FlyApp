using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FlyApp.ViewModels.Base
{
    /// <summary>
    ///     <see cref="IAsyncCommand" /> is extended bind-able command to allow tracking the state of asynchronous operations.
    ///     Every asynchronous operation, which is bound to view, should inform the view about it's state - whether it's still
    ///     performing background operation,
    ///     whether the last execution has failed or was successful.
    /// </summary>
    /// <seealso cref="IBindableCommand" />
    public interface IAsyncCommand : IBindableCommand
    {
        /// <summary>
        ///     Indicates  that the command is executing background operation.
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        ///     If not <code>null</code>, then the last execution of the command has failed.
        ///     The property will contain user-friendly error message.
        ///     The value of this property is reset every time command is executed, and will reflect only the last execution state.
        /// </summary>
        string FailureMessage { get; }

        /// <summary>
        ///     If set to <code>true</code>, the last execution of the command was successful.
        ///     The value of this property is reset every time command is executed, and will reflect only the last execution state.
        /// </summary>
        bool IsSuccessful { get; }

        /// <summary>
        ///     This method is a wrapper around <see cref="ICommand.Execute" /> method, to allow clients await the result, instead
        ///     of
        ///     firing and forgetting.
        /// </summary>
        /// <param name="param">Command parameter</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task ExecuteAsync(object param, CancellationToken token = default);
    }
}