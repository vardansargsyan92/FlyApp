using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FlyApp.ViewModels.Resources.Strings.Common;
using PropertyChanged;

namespace FlyApp.ViewModels.Base.Implementation
{
    /// <summary>
    ///     Base implementation of <see cref="IAsyncCommand" /> interface.
    /// </summary>
    /// <seealso cref="BaseBindableCommand" />
    /// <seealso cref="IAsyncCommand" />
    [AddINotifyPropertyChangedInterface]
    public abstract class AsyncCommand : BaseBindableCommand, IAsyncCommand
    {
        protected AsyncCommand(bool canExecute = true) : base(canExecute)
        {
        }

        public override async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public bool IsBusy { get; private set; }
        public string FailureMessage { get; protected set; }
        public bool IsSuccessful { get; private set; }

        /// <summary>
        ///     Base implementation of <see cref="IAsyncCommand.ExecuteAsync" /> method. Contains logic to handle the state of the
        ///     command.
        ///     Generally this method should not be overridden by child classes, instead, you should override
        ///     <see cref="ExecuteCoreAsync" />.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Asynchronous await-able task.</returns>
        public virtual async Task ExecuteAsync(object parameter, CancellationToken token = default)
        {
            if (IsBusy)
            {
                Debug.WriteLine("{Command} was already executing. Cancelling recurrent execution.", CommandName);
                return;
            }

            Debug.WriteLine("{Command} execution started", CommandName);
            //Reset the state of the command
            IsBusy = true;
            IsSuccessful = false;
            FailureMessage = null;
            try
            {
                IsSuccessful = await ExecuteCoreAsync(parameter, token);
                Debug.WriteLine("{Command} execution completed with result {Result}.", CommandName, IsSuccessful);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                //At the end set that execution has finished
                IsBusy = false;
            }
        }

        /// <summary>
        ///     Executes the core asynchronous operation. Child classes should override this method with their operation
        ///     implementation.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        ///     Child implementation should return <code>true</code> if the operation is considered successful.
        ///     The returned value will be set to <see cref="IsSuccessful" /> property.
        /// </returns>
        protected abstract Task<bool> ExecuteCoreAsync(object parameter, CancellationToken token = default);

        /// <summary>
        ///     Default exception handler. Sets the <see cref="FailureMessage" /> to
        ///     <see cref="CommonStrings.GeneralFailureMessage" />.
        ///     Override this method for custom exception handling and be sure to set the value of <see cref="FailureMessage" />
        ///     to user-friendly message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        protected virtual void HandleException(Exception exception)
        {
            FailureMessage = CommonStrings.GeneralFailureMessage;
            Debug.WriteLine(exception.Message, "{Command} execution failed with exception.", CommandName);
        }
    }
}