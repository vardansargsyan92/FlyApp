using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FlyApp.ViewModels.Base.Implementation
{
    /// <summary>
    /// Base class to implement <see cref="INotifyPropertyChanged" /> interface. All of the object which allow binding behavior should derive from this class.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class BaseBindableObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}