using System.ComponentModel;

namespace FlyApp.ViewModels.Base
{
    public interface INavigableViewModel : INotifyPropertyChanged
    {
        IAsyncCommand NavigateBackCommand { get; }
    }
}