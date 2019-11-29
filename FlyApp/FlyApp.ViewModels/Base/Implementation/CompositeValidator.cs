using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FlyApp.Core.Extensions;

namespace FlyApp.ViewModels.Base.Implementation
{
    /// <summary>
    ///     Implements Composite design pattern for <see cref="IViewModelValidator" />.
    ///     There are some cases where view model is complex and requires splitting of validators into separate instances.
    ///     But still, you want to be able to track the validation state of the view model in a centralized fashion through a
    ///     single object.
    ///     That's where <see cref="CompositeValidator" /> will become handy. All of the validation methods will return the
    ///     combination of all
    ///     validation information from all internal validator instances.
    /// </summary>
    /// <seealso cref="IViewModelValidator" />
    public class CompositeValidator : IViewModelValidator
    {
        private readonly List<IViewModelValidator> _validators;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompositeValidator" /> class from specified instances of concrete
        ///     validators.
        ///     This is where the composition happens. Currently we don't allow adding new validators runtime, only during
        ///     initialization.
        /// </summary>
        /// <param name="validators">The validators.</param>
        public CompositeValidator(IEnumerable<IViewModelValidator> validators)
        {
            _validators = new List<IViewModelValidator>();
            if (validators != null)
                foreach (var validator in validators)
                {
                    _validators.Add(validator);
                    validator.ErrorsChanged += ValidatorOnErrorsChanged;
                }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (_validators == null) return null;

            var errors = new List<object>();
            foreach (var validator in _validators) errors.AddRange(validator.GetErrors(propertyName));

            return errors;
        }

        public bool HasErrors => _validators?.Any(validator => validator.HasErrors) ?? false;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool Validate()
        {
            foreach (var validator in _validators) validator.Validate();

            return !HasErrors;
        }

        public bool ValidateProperty(string propertyName)
        {
            var result = true;
            foreach (var validator in _validators) result &= validator.ValidateProperty(propertyName);

            return result;
        }

        public async Task<bool> ValidateAsync()
        {
            var result = true;
            foreach (var validator in _validators) result &= await validator.ValidateAsync();

            return result;
        }

        public async Task<bool> ValidatePropertyAsync(string property)
        {
            var result = true;
            foreach (var validator in _validators) result &= await validator.ValidatePropertyAsync(property);
            return result;
        }

        public IList GetAllErrors(params string[] propertyNames)
        {
            if (_validators == null) return null;

            var errors = new List<object>();
            foreach (var validator in _validators)
            {
                var allErrors = validator.GetAllErrors(propertyNames);
                if (allErrors != null) errors.AddRange(allErrors);
            }

            return errors;
        }

        public List<string> GetAllErrorsInString()
        {
            if (_validators == null) return null;

            var errors = new List<string>();
            foreach (var validator in _validators)
            {
                var allErrors = validator.GetAllErrorsInString();
                if (allErrors != null) errors.AddRange(allErrors);
            }

            return errors;
        }

        public List<string> GetErrorsInString(string propertyName)
        {
            if (_validators == null) return null;

            var errors = new List<string>();
            foreach (var validator in _validators)
            {
                var allErrors = validator.GetErrorsInString(propertyName);
                if (allErrors != null) errors.AddRange(allErrors);
            }

            return errors;
        }

        private void ValidatorOnErrorsChanged(object sender, DataErrorsChangedEventArgs args)
        {
            ErrorsChanged?.Invoke(sender, args);
        }
    }
}