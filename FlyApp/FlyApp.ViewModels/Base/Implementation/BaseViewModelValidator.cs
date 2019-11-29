using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using PropertyChanged;

namespace FlyApp.ViewModels.Base.Implementation
{
    /// <summary>
    /// Base implementation of <see cref="IViewModelValidator"/> using <see cref="FluentValidation"/> as a validation core.
    /// As you can see we restrict the type of <typeparam name="T"/> to only <see cref="INotifyPropertyChanged"/> interface.
    /// The reason is that we use <see cref="INotifyPropertyChanged.PropertyChanged"/> event to track the changes in properties
    /// and trigger validation logic when a specific property has been changed. Thus, user interface can track the errors with property-level or class-level
    /// granularity and update the user interface to react to validation failures and show error messages.
    /// </summary>
    /// <typeparam name="T">The type of the view model under validation</typeparam>
    /// <seealso cref="FluentValidation.AbstractValidator{T}" />
    /// <seealso cref="QQPad.Mobile.ViewModels.Base.IViewModelValidator" />
    [AddINotifyPropertyChangedInterface]
    public abstract class BaseViewModelValidator<T> : AbstractValidator<T>, IViewModelValidator, INotifyPropertyChanged
        where T : INotifyPropertyChanged
    {
        private readonly T _viewModel;
        private Dictionary<string, IList<ValidationFailure>> _errors;

        protected BaseViewModelValidator(T viewModel)
        {
            _viewModel = viewModel;
            _viewModel.PropertyChanged += OnValidatorPropertyChanged;
        }

        public bool Validate()
        {
            ValidationResult result = Validate(_viewModel);
            return CreateValidationResult(result);
        }
        
        public bool ValidateProperty(string propertyName)
        {
            ValidationContext<T> context = GetValidationContextForProperty(propertyName);
            ValidationResult result = Validate(context);
            return CreateValidationResultForProperty(propertyName, result);
        }
        
        
        public async Task<bool> ValidateAsync()
        {
            ValidationResult result = await ValidateAsync(_viewModel);
            return CreateValidationResult(result);
        }

        public async Task<bool> ValidatePropertyAsync(string property)
        {
            ValidationResult result = await ValidateAsync(GetValidationContextForProperty(property));
            return CreateValidationResultForProperty(property, result);
        }

        public IList GetAllErrors(string[] propertyNames = null)
        {
            if(propertyNames?.Any() == true)
            {
                return _errors?.Where(error => propertyNames.Contains(error.Key)).SelectMany(error => error.Value).ToList();
            }

            return _errors?.SelectMany(error => error.Value).ToList();
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if(_errors?.Any() != true)
            {
                return null;
            }

            return _errors.TryGetValue(propertyName, out IList<ValidationFailure> errors) ? errors : null;
        }

        public List<string> GetAllErrorsInString()
        {
            if(_errors?.Any() != true)
            {
                return null;
            }

            return _errors.SelectMany(error => error.Value).Select(error => error.ErrorMessage).ToList();
        }

        public List<string> GetErrorsInString(string propertyName)
        {
            if(_errors?.Any() != true)
            {
                return null;
            }

            if(!_errors.TryGetValue(propertyName, out IList<ValidationFailure> errors))
            {
                return null;
            }

            return errors.Select(error => error.ErrorMessage).ToList();
        }

        public bool HasErrors { get; private set; }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
        private void OnValidatorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(ShouldValidate(e.PropertyName))
            {
                ValidateProperty(e.PropertyName);
            }
        }

        protected virtual bool ShouldValidate(string propertyName)
        {
            foreach(IValidationRule rule in this)
            {
                var property = rule as PropertyRule;
                if(property?.PropertyName == propertyName)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CreateValidationResult(ValidationResult result)
        {
            HasErrors = !result.IsValid;
            if(result.IsValid)
            {
                _errors = null;
            }
            else
            {
                _errors = result.Errors
                                .GroupBy(error => error.PropertyName, error => error)
                                .ToDictionary(errors => errors.Key, errors => errors.ToList() as IList<ValidationFailure>);
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null));
            return result.IsValid;
        }

        private ValidationContext<T> GetValidationContextForProperty(string propertyName)
        {
            string[] properties = {propertyName};
            ValidationContext<T> context = new ValidationContext<T>(_viewModel, new PropertyChain(), new MemberNameValidatorSelector(properties));
            return context;
        }
        
        private bool CreateValidationResultForProperty(string propertyName, ValidationResult result)
        {
            if(result.IsValid)
            {
                if(_errors?.ContainsKey(propertyName) == true)
                {
                    _errors.Remove(propertyName);
                }
            }
            else
            {
                _errors = _errors ?? new Dictionary<string, IList<ValidationFailure>>();
                _errors[propertyName] = result.Errors;
            }

            HasErrors = _errors?.Any() == true;
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            return result.IsValid;
        }
    }
}