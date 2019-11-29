namespace FlyApp.ViewModels.Base
{
    public interface ISaveableViewModel : IDiscardableViewModel, IValidateableViewModel
    {
        IAsyncCommand SaveCommand { get; }
    }
}