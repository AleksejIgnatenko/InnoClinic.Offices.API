using InnoClinic.Offices.Application.Services;
using InnoClinic.Offices.Core.Models.OfficeModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoClinic.Offices.API.Controllers
{
    //[Authorize(Roles = "Receptionist")]
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeController : ControllerBase
    {
        private readonly IOfficeService _officeService;

        public OfficeController(IOfficeService officeService)
        {
            _officeService = officeService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateOfficeAsync(OfficeRequest officeRequest)
        {
            await _officeService.CreateOfficeAsync(officeRequest.City, officeRequest.Street, officeRequest.HouseNumber, officeRequest.OfficeNumber, officeRequest.PhotoId, officeRequest.RegistryPhoneNumber, officeRequest.IsActive);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetAllOfficesAsync()
        {
            return Ok(await _officeService.GetAllOfficesAsync());
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult> GetOfficeByIdAsync(Guid id)
        {
            return Ok(await _officeService.GetOfficeByIdAsync(id));
        }

        [AllowAnonymous]
        [HttpGet("all-active-offices")]
        public async Task<ActionResult> GetAllActiveOfficesAsync()
        {
            return Ok(await _officeService.GetAllActiveOfficesAsync());
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateOfficeAsync(Guid id, OfficeRequest officeRequest)
        {
            await _officeService.UpdateOfficeAsync(id, officeRequest.City, officeRequest.Street, officeRequest.HouseNumber, officeRequest.OfficeNumber, officeRequest.PhotoId, officeRequest.RegistryPhoneNumber, officeRequest.IsActive);

            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteOfficeAsync(Guid id)
        {
            await _officeService.DeleteOfficeAsync(id);

            return Ok();
        }
    }
}
