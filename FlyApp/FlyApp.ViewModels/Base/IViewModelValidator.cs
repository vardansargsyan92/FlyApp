using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FlyApp.ViewModels.Base
{
    /// <summary>
    /// Validation is one of the core aspects of Enterprise mobile application development.
    /// <see cref="IViewModelValidator"/> is the gateway to validation which derives from WPF classics <see cref="INotifyDataErrorInfo"/> interface,
    /// and adds some extra features to make it easy tracking the valid state of the view model in the user interface easier.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyDataErrorInfo" />
    public interface IViewModelValidator : INotifyDataErrorInfo
    {
        /// <summary>
        /// Executes the validation rules and returns <code>true</code> if the validation was successful.
        /// When validation is executed, all of rules of all properties will be executed and the internal error state is updated. 
        /// Thus, all of the validation properties and methods will return the state of the last validation call.
        /// </summary>
        /// <returns><code>true</code> if there were no validation errors</returns>
        bool Validate();

        bool ValidateProperty(string propertyName);

        Task<bool> ValidateAsync();
        
        Task<bool> ValidatePropertyAsync(string property);
        
        /// <summary>
        /// Returns all underlying error objects for specified properties from the last validation call.
        /// If no property was specified, all errors from all properties will be returned. 
        /// </summary>
        /// <param name="propertyNames">The property names.</param>
        /// <returns>List of error options</returns>
        IList GetAllErrors(params string[] propertyNames);

        /// <summary>
        /// Gets all errors of all properties in a string list from the last validation call.
        /// </summary>
        /// <returns>List of string errors.</returns>
        List<string> GetAllErrorsInString();

        /// <summary>
        /// Gets the errors of a specific property from the last validation call.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>List of string errors</returns>
        List<string> GetErrorsInString(string propertyName);
    }
}