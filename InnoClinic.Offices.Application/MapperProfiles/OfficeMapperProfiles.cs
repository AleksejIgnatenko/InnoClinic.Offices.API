using AutoMapper;
using InnoClinic.Offices.Core.Dto;
using InnoClinic.Offices.Core.Models;

namespace InnoClinic.Offices.Application.MapperProfiles
{
    public class OfficeMapperProfiles : Profile
    {
        public OfficeMapperProfiles()
        {
            CreateMap<OfficeModel, OfficeDto>();
        }
    }
}
