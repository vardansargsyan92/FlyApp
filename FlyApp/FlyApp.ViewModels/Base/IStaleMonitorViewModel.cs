using System.ComponentModel;

namespace FlyApp.ViewModels.Base
{
    /// <summary>
    /// Common interface for all view models which have a stale monitor.
    /// Try to always implement this interface when you implement stale monitoring for a view model.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public interface IStaleMonitorViewModel : INotifyPropertyChanged
    {
        IStaleMonitor StaleMonitor { get; }
    }
}