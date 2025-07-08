using FluentValidation;
using InnoClinic.Offices.Core.Models.OfficeModels;

namespace InnoClinic.Offices.Application.Validators;

/// <summary>
/// Validator for validating <see cref="OfficeRequest"/> objects.
/// </summary>
internal class OfficeRequestValidator : AbstractValidator<OfficeRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OfficeRequestValidator"/> class.
    /// Configures validation rules for the <see cref="OfficeRequest"/> object.
    /// </summary>
    public OfficeRequestValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City must not be empty.");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street must not be empty.");

        RuleFor(x => x.HouseNumber)
            .NotEmpty().WithMessage("House Number must not be empty.");

        RuleFor(office => office.RegistryPhoneNumber)
            .NotEmpty().WithMessage("Registry Phone Number must not be empty.")
            .Matches(@"^\+").WithMessage("Registry Phone Number must start with '+'.");
    }
}