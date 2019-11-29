using System.ComponentModel;

namespace FlyApp.ViewModels.Base
{
    /// <summary>
    /// Common interface for all view models which have a validator.
    /// Try to always implement this interface when you implement validation for a view model.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public interface IValidateableViewModel : INotifyPropertyChanged
    {
        IViewModelValidator Validator { get; }
    }
}