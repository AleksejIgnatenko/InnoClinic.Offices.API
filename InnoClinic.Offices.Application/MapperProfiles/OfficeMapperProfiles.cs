using AutoMapper;
using InnoClinic.Offices.Core.Models.OfficeModels;

namespace InnoClinic.Offices.Application.MapperProfiles;

public class OfficeMapperProfiles : Profile
{
    public OfficeMapperProfiles()
    {
        CreateMap<OfficeRequest, OfficeEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<OfficeEntity, OfficeDto>();
    }
}