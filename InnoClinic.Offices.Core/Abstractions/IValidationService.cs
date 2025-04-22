using FluentValidation.Results;
using InnoClinic.Offices.Core.Models.OfficeModels;

namespace InnoClinic.Offices.Application.Services
{
    public interface IValidationService
    {
        List<ValidationFailure> Validation(OfficeEntity office);
    }
}