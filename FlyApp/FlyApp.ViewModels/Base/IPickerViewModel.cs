namespace FlyApp.ViewModels.Base
{
    public interface IPickerViewModel<T>
        where T : ISelectableViewModel
    {
        T Current { get; set; }
    }
}