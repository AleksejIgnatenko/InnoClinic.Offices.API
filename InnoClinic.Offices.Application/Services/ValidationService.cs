using FluentValidation.Results;
using InnoClinic.Offices.Application.Validators;
using InnoClinic.Offices.Core.Models.OfficeModels;

namespace InnoClinic.Offices.Application.Services
{
    public class ValidationService : IValidationService
    {
        public Dictionary<string, string> Validation(OfficeEntity office)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();

            OfficeValidator validations = new OfficeValidator();
            ValidationResult validationResult = validations.Validate(office);
            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                {
                    errors[failure.PropertyName] = failure.ErrorMessage;
                }
            }

            return errors;
        }
    }
}
