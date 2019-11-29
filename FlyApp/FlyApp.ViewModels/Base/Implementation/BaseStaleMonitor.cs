using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using FlyApp.Core.Extensions;
using PropertyChanged;

namespace FlyApp.ViewModels.Base.Implementation
{
    /// <summary>
    ///     Base implementation of <see cref="IStaleMonitor" />.
    ///     The implementation uses reflections to fetch the values of the specified properties and collection properties when
    ///     they are captured.
    ///     Child classes should generally only override <see cref="Properties" /> and <see cref="Collections" /> and specify
    ///     which properties or collection properties
    ///     they would like to track.
    /// </summary>
    /// <seealso cref="IStaleMonitor" />
    [AddINotifyPropertyChangedInterface]
    internal class BaseStaleMonitor : BaseBindableObject, IStaleMonitor
    {
        private readonly List<object> _internalSubscriptions = new List<object>();
        private readonly Dictionary<string, bool> _staleProperties = new Dictionary<string, bool>();
        private readonly Dictionary<string, object> _subscriptions = new Dictionary<string, object>();
        private readonly INotifyPropertyChanged _viewModel;
        private List<PropertyInfo> _collectionProperties;
        private Dictionary<string, object[]> _originalCollectionValues;
        private Dictionary<string, object> _originalPropertyValues;
        private List<PropertyInfo> _properties;

        public BaseStaleMonitor(INotifyPropertyChanged viewModel)
        {
            _viewModel = viewModel;
            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        public bool IsStale => _staleProperties.Values.Any(item => item);

        public virtual IEnumerable<string> Properties => null;
        public virtual IEnumerable<string> Collections => null;

        public void Capture()
        {
            CaptureProperties();
            CaptureCollectionProperties();
        }

        public void CaptureProperties(params string[] properties)
        {
            if (Properties?.Any() != true) return;

            //If no property was specified that means we want to capture all properties
            if (properties?.Any() != true)
            {
                _properties = _viewModel.GetType().GetRuntimeProperties().Where(p => Properties.Contains(p.Name))
                    .ToList();
            }
            else
            {
                //Otherwise we need to add the specified list of properties to the existing list of properties
                _properties = _properties ?? new List<PropertyInfo>();
                _properties.AddRange(_viewModel.GetType().GetRuntimeProperties().Where(p =>
                    properties.Any(prop => prop == p.Name) &&
                    _properties.All(prop => prop.Name != p.Name) &&
                    Properties.Contains(p.Name)));
            }

            foreach (var property in _properties) CaptureProperty(property.Name);
        }

        public void CaptureCollectionProperties(params string[] properties)
        {
            if (Collections?.Any() != true) return;

            //If no property was specified that means we want to capture all properties
            if (properties?.Any() != true)
            {
                var enumerableInfo = typeof(IEnumerable).GetTypeInfo();

                _collectionProperties = _viewModel.GetType().GetRuntimeProperties()
                    .Where(p => Collections?.Contains(p.Name) == true &&
                                enumerableInfo.IsAssignableFrom(p.PropertyType.GetTypeInfo()))
                    .ToList();
            }
            else
            {
                _collectionProperties = _collectionProperties ?? new List<PropertyInfo>();
                _collectionProperties.AddRange(_viewModel.GetType().GetRuntimeProperties().Where(p =>
                    properties.Any(prop => prop == p.Name) &&
                    _collectionProperties.All(prop => prop.Name != p.Name) &&
                    Collections.Contains(p.Name)));
            }

            foreach (var property in _collectionProperties) CaptureProperty(property.Name, true);
        }

        public bool ArePropertiesStale(params string[] properties)
        {
            var propertiesToCheck = properties;
            if (propertiesToCheck?.Any() != true)
                propertiesToCheck = _properties?.Select(property => property.Name).ToArray();

            if (propertiesToCheck == null) return false;

            foreach (var property in propertiesToCheck)
                if (CheckPropertyIsStale(property))
                    return true;

            return false;
        }

        public bool AreCollectionsStale(params string[] properties)
        {
            var propertiesToCheck = properties;
            if (propertiesToCheck?.Any() != true)
                propertiesToCheck = _collectionProperties?.Select(property => property.Name).ToArray();

            if (propertiesToCheck == null) return false;

            foreach (var property in propertiesToCheck)
                if (CheckCollectionIsState(property))
                    return true;

            return false;
        }

        private void CaptureProperty(string property, bool isCollection = false)
        {
            if (_staleProperties.ContainsKey(property))
            {
                _staleProperties.Remove(property);
                OnPropertyChanged(nameof(IsStale));
            }

            if (isCollection)
            {
                var enumerable = (IEnumerable) _viewModel.GetPropertyValue(property);
                var objects = enumerable?.ToArray();

                if (enumerable is INotifyCollectionChanged collectionChanged &&
                    !_subscriptions.Values.Contains(enumerable))
                    collectionChanged.CollectionChanged += OnCollectionChanged;

                _subscriptions[property] = enumerable;
                _originalCollectionValues = _originalCollectionValues ?? new Dictionary<string, object[]>();
                CaptureInternalStaleMonitors(enumerable);
                _originalCollectionValues[property] = objects;
            }
            else
            {
                _originalPropertyValues = _originalPropertyValues ?? new Dictionary<string, object>();
                var propertyValue = _viewModel.GetPropertyValue(property);
                _originalPropertyValues[property] = propertyValue;
                if (propertyValue != null && propertyValue is IStaleMonitorViewModel viewModel)
                {
                    viewModel.StaleMonitor?.Capture();
                    if (!_subscriptions.Values.Contains(viewModel) && viewModel.StaleMonitor != null)
                    {
                        viewModel.StaleMonitor.PropertyChanged +=
                            (sender, args) => HandlePropertyChanged(viewModel, property);
                        _subscriptions[property] = viewModel;
                    }
                }
            }
        }

        private bool CheckPropertyIsStale(string property)
        {
            if (_originalPropertyValues?.ContainsKey(property) != true) return false;

            object oldValue = null;
            var newValue = _viewModel?.GetPropertyValue(property);
            var isStale = false;
            if (_originalPropertyValues?.TryGetValue(property, out oldValue) == true)
                isStale = !newValue.ObjectsEqual(oldValue);

            if (!isStale && newValue != null && newValue is IStaleMonitorViewModel viewModel)
                isStale = viewModel.StaleMonitor.IsStale;

            return isStale;
        }

        private bool CheckCollectionIsState(string property)
        {
            if (_originalCollectionValues?.ContainsKey(property) != true) return false;

            object[] oldCollection = null;
            if (_originalCollectionValues?.TryGetValue(property, out oldCollection) == true)
            {
                var newCollection = (IEnumerable) _viewModel.GetPropertyValue(property);
                if (!oldCollection.EnumerableEqual(newCollection)) return true;

                if (IsCollectionStale(newCollection)) return true;

                return false;
            }

            return false;
        }

        private void CaptureInternalStaleMonitors(IEnumerable objects)
        {
            foreach (var obj in objects)
                if (obj is IStaleMonitorViewModel viewModel && viewModel.StaleMonitor != null)
                {
                    viewModel.StaleMonitor.Capture();
                    if (!_internalSubscriptions.Contains(viewModel))
                    {
                        viewModel.StaleMonitor.PropertyChanged += (sender, args) => HandleCollectionChanged(objects);
                        _internalSubscriptions.Add(viewModel);
                    }
                }
        }

        private bool IsCollectionStale(IEnumerable enumerable)
        {
            foreach (var item in enumerable)
                if (item is IStaleMonitorViewModel viewModel)
                    if (viewModel.StaleMonitor.IsStale)
                        return true;

            return false;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            HandlePropertyChanged(sender, e.PropertyName);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HandleCollectionChanged(sender);
        }

        private void HandlePropertyChanged(object sender, string propertyName)
        {
            if (ReferenceEquals(sender, _viewModel))
            {
                _staleProperties[propertyName] = CheckPropertyIsStale(propertyName);
            }
            else if (_subscriptions.Values.Contains(sender))
            {
                var subscription = _subscriptions.FirstOrDefault(item => ReferenceEquals(item.Value, sender));
                if (_originalCollectionValues?.ContainsKey(subscription.Key) == true)
                    _staleProperties[subscription.Key] = CheckCollectionIsState(subscription.Key);
                else if (_originalPropertyValues?.ContainsKey(subscription.Key) == true)
                    _staleProperties[subscription.Key] = CheckPropertyIsStale(subscription.Key);
            }

            OnPropertyChanged(nameof(IsStale));
        }

        private void HandleCollectionChanged(object sender)
        {
            if (_subscriptions?.Values.Contains(sender) == true)
            {
                var subscription = _subscriptions.FirstOrDefault(item => ReferenceEquals(item.Value, sender));
                _staleProperties[subscription.Key] = CheckCollectionIsState(subscription.Key);
            }

            OnPropertyChanged(nameof(IsStale));
        }
    }
}