using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FlyApp.ViewModels.Base
{
    public interface IMultiSelectViewModel<T> where T : class
    {
        IEnumerable<T> Items { get; }
        ObservableCollection<T> SelectedItems { get; }
    }
}