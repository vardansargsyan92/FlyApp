using System.Collections.Generic;
using System.ComponentModel;

namespace FlyApp.ViewModels.Base
{
    /// <summary>
    /// In enterprise application development it's to understand whether user has made changes to the form or the form is in it's original state,
    /// to be able to warn the user about pending changes when they try to dismiss the screen. To make this process semi-automatic and to avoid spaghetti code,
    /// <see cref="IStaleMonitor"/> was creating to allow capturing the state of the view model when it's initialized and further check if the captured properties
    /// have been changed or not.
    /// </summary>
    public interface IStaleMonitor : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a value indicating whether the state is stale or no.
        /// Under the hood the property will execute a method to compare the previously captured state with the current state.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the state is stale; otherwise, <c>false</c>.
        /// </value>
        bool IsStale { get; }

        /// <summary>
        /// Here is where the concrete implementations should specify the name of the properties which should be tracked and captured.
        /// The list should be preferably set on time during object construction phase.
        /// </summary>
        /// <value>
        /// The property names that should be tracked.
        /// </value>
        IEnumerable<string> Properties { get; }

        /// <summary>
        ///  Here is where the concrete implementations should specify the name of the collection properties which should be tracked and captured.
        /// We have separated the collections from regular properties, because we need to check whether the previously captured state of the property
        /// is equal to current state or no. For singular properties we'll execute <see cref="ObjectExtensions.ObjectsEqual"/> method,
        /// but for collection properties we should be bit-by-bit comparison using <see cref="CollectionExtensions.EnumerableEqual"/> method.
        /// The list should be preferably set on time during object construction phase.
        /// </summary>
        /// <value>
        /// The collection property names that should be tracked.
        /// </value>
        IEnumerable<string> Collections { get; }

        /// <summary>
        /// Captures the sate of the properties and collection properties specified by <see cref="Properties"/> and <see cref="Collections"/>.
        /// </summary>
        void Capture();

        /// <summary>
        /// Captures the state of subset of properties. <paramref name="properties"/> should be a subset of <see cref="Properties"/>. Otherwise,
        /// elements not in <see cref="Properties"/> will be ignored.
        /// </summary>
        /// <param name="properties">The property names for which we need to capture state.</param>
        void CaptureProperties(params string[] properties);

        /// <summary>
        /// Captures the state of subset of collection properties. <paramref name="properties"/> should be a subset of <see cref="Collections"/>. Otherwise,
        /// elements not in <see cref="Collections"/> will be ignored.
        /// </summary>
        /// <param name="properties">The collection property names for which we need to capture state.</param>
        void CaptureCollectionProperties(params string[] properties);

        /// <summary>
        /// Gets a value indicating whether the projection of properties is stale.
        /// Under the hood the property will execute a method to compare the previously captured state of <paramref name="properties"/> with the current state.
        /// <paramref name="properties"/> should be a subset of <see cref="Properties"/>. Otherwise,
        /// elements not in <see cref="Properties"/> will be ignored.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the properties are stale; otherwise, <c>false</c>.
        /// </value>
        bool ArePropertiesStale(params string[] properties);

        /// <summary>
        /// Gets a value indicating whether the projection of collection properties is stale.
        /// Under the hood the property will execute a method to compare the previously captured state of collection <paramref name="properties"/> with the current state.
        /// <paramref name="properties"/> should be a subset of <see cref="Collections"/>. Otherwise,
        /// elements not in <see cref="Collections"/> will be ignored.
        /// </summary>
        /// <value>
        ///   <c>true</c> if properties are stale; otherwise, <c>false</c>.
        /// </value>
        bool AreCollectionsStale(params string[] properties);
    }
}