﻿using FluentValidation;
using InnoClinic.Offices.Core.Models.OfficeModels;

namespace InnoClinic.Offices.Application.Validators;

internal class OfficeRequestValidator : AbstractValidator<OfficeRequest>
{
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
