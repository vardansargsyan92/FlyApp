// Interface IViewEventsListener 
// When certain events occur in View, there may be a need to process its effects
// in the ViewModel class. This interface provides a path from View to 
// its ViewModel. The interface methods are implemented in ViewModelBase class
// as virtual, to be overriden in the ViewModel class as required.

namespace FlyApp.ViewModels.Base
{
    public interface IViewEventsListener
    {
        void OnAppearing();

        void OnDisappearing();

        bool NavigateBack();

        void OnListItemSelection(object item);
    }
}