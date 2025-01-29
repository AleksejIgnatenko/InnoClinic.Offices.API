using InnoClinic.Offices.API.Contracts;
using InnoClinic.Offices.Application.Services;
using InnoClinic.Offices.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace InnoClinic.Offices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeController
    {
        private readonly IOfficeService _officeService;

        public OfficeController(IOfficeService officeService)
        {
            _officeService = officeService;
        }

        [HttpPost]
        public async Task CreateOfficeAsync(OfficeRequest officeRequest)
        {
            await _officeService.CreateOfficeAsync(officeRequest.Address, officeRequest.PhotoId, officeRequest.RegistryPhoneNumber, officeRequest.IsActive);
        }

        [HttpGet]
        public async Task<IEnumerable<OfficeModel>> GetAllOfficesAsync()
        {
            return await _officeService.GetAllOfficesAsync();
        }

        [HttpGet("{id}")]
        public async Task<OfficeModel> GetOfficeByIdAsync(Guid id)
        {
            return await _officeService.GetOfficeByIdAsync(id);
        }

        [HttpPut]
        public async Task UpdateOfficeAsync(OfficeRequest officeRequest)
        {
            await _officeService.UpdateOfficeAsync(officeRequest.Address, officeRequest.PhotoId, officeRequest.RegistryPhoneNumber, officeRequest.IsActive);
        }

        [HttpDelete]
        public async Task DeleteOfficeAsync(Guid id)
        {
            await _officeService.DeleteOfficeAsync(id);
        }
    }
}
