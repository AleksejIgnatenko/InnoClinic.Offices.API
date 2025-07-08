using AutoMapper;
using InnoClinic.Offices.Core.Models.OfficeModels;

namespace InnoClinic.Offices.Application.MapperProfiles;

/// <summary>
/// Mapper profiles for mapping between different office-related classes.
/// </summary>
public class OfficeMapperProfiles : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OfficeMapperProfiles"/> class.
    /// Configures mapping between <see cref="OfficeRequest"/> and <see cref="OfficeEntity"/>.
    /// </summary>
    public OfficeMapperProfiles()
    {
        CreateMap<OfficeRequest, OfficeEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<OfficeEntity, OfficeDto>();
    }
}