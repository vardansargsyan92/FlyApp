using System.Collections.ObjectModel;

namespace FlyApp.ViewModels.Base
{
    public interface IPersistentSelectionViewModel<T>
        where T : ISelectableViewModel
    {
        ObservableCollection<T> Items { get; }

        T Current { get; set; }

        string CurrentId { get; set; }
    }
}