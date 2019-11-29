using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlyApp.ViewModels.Base.Implementation
{
    internal abstract class TimeoutAsyncCommand : AsyncCommand
    {
        private CancellationTokenSource _timeoutToken;
        private CancellationTokenSource _token;

        protected virtual TimeSpan Timeout { get; } = TimeSpan.FromMilliseconds(300);

        public override async Task ExecuteAsync(object parameter, CancellationToken token = new CancellationToken())
        {
            _timeoutToken?.Cancel();
            _timeoutToken = new CancellationTokenSource();
            _token = CancellationTokenSource.CreateLinkedTokenSource(_timeoutToken.Token, token);
            try
            {
                await Task.Delay(Timeout, _token.Token);
                await base.ExecuteAsync(parameter, _token.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}