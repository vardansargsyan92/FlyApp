namespace FlyApp.ViewModels.Base
{
    public interface IDiscardableViewModel : IStaleMonitorViewModel
    {
        IAsyncCommand DiscardCommand { get; }
    }
}