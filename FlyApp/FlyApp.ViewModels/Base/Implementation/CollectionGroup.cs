using System.Collections.Generic;
using System.Collections.ObjectModel;
using PropertyChanged;

namespace FlyApp.ViewModels.Base.Implementation
{
    [AddINotifyPropertyChangedInterface]
    public class CollectionGroup<TGroup, TItem> : ObservableCollection<TItem>
    {
        public CollectionGroup()
        {
        }

        public CollectionGroup(TGroup group, IEnumerable<TItem> items) : base(items)
        {
            Group = group;
        }

        public TGroup Group { get; set; }
    }
}