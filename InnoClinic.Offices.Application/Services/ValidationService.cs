using FluentValidation;
using FluentValidation.Results;
using InnoClinic.Offices.Application.Validators;
using InnoClinic.Offices.Core.Models.OfficeModels;

namespace InnoClinic.Offices.Application.Services
{
    public class ValidationService : IValidationService
    {
        public List<ValidationFailure> Validation(OfficeEntity entity)
        {
            var validator = new OfficeValidator();
            return Validate(entity, validator);
        }

        private List<ValidationFailure> Validate<T>(T model, IValidator<T> validator)
        {
            var validationFailures = new List<ValidationFailure>();
            ValidationResult validationResult = validator.Validate(model);
            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                {
                    validationFailures.Add(new ValidationFailure(failure.PropertyName, failure.ErrorMessage));
                }
            }

            return validationFailures;
        }
    }
}
