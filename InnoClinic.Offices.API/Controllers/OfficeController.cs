using InnoClinic.Offices.Core.Abstractions;
using InnoClinic.Offices.Core.Models.OfficeModels;
using InnoClinic.Offices.Infrastructure.Enums.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoClinic.Offices.API.Controllers;

/// <summary>
/// Controller for managing offices.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Receptionist")]
public class OfficeController(IOfficeService officeService) : ControllerBase
{
    private readonly IOfficeService _officeService = officeService;

    /// <summary>
    /// Create a new office.
    /// </summary>
    /// <param name="officeRequest">Request to create an office.</param>
    /// <returns>ActionResult with the newly created office.</returns>
    [HttpPost]
    public async Task<ActionResult> CreateOffice(OfficeRequest officeRequest)
    {
        var office = await _officeService.CreateOfficeAsync(officeRequest);

        return CreatedAtAction(nameof(GetOfficeById), new { id = office.Id }, office);
    }

    /// <summary>
    /// Get all offices.
    /// </summary>
    /// <returns>ActionResult with a list of all offices.</returns>
    [AllowAnonymous]
    [HttpGet]
    [ResponseCache(CacheProfileName = nameof(CacheProfileNameEnum.CacheDefault90))]
    public async Task<ActionResult> GetAllOffices()
    {
        return Ok(await _officeService.GetAllOfficesAsync());
    }

    /// <summary>
    /// Get an office by its ID.
    /// </summary>
    /// <param name="id">The ID of the office to retrieve.</param>
    /// <returns>ActionResult with the office information.</returns>
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ResponseCache(CacheProfileName = nameof(CacheProfileNameEnum.CacheDefault90))]
    public async Task<ActionResult> GetOfficeById(Guid id)
    {
        return Ok(await _officeService.GetOfficeByIdAsync(id));
    }

    /// <summary>
    /// Get all active offices.
    /// </summary>
    /// <returns>ActionResult with a list of all active offices.</returns>
    [AllowAnonymous]
    [HttpGet("active")]
    [ResponseCache(CacheProfileName = nameof(CacheProfileNameEnum.CacheDefault90))]
    public async Task<ActionResult> GetAllActiveOffices()
    {
        return Ok(await _officeService.GetAllActiveOfficesAsync());
    }

    /// <summary>
    /// Update an existing office.
    /// </summary>
    /// <param name="id">The ID of the office to update.</param>
    /// <param name="officeRequest">Request to update the office.</param>
    /// <returns>ActionResult with the updated office information.</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateOffice(Guid id, OfficeRequest officeRequest)
    {
        var office = await _officeService.UpdateOfficeAsync(id, officeRequest);

        return CreatedAtAction(nameof(GetOfficeById), new { id = office.Id }, office);
    }

    /// <summary>
    /// Delete an office by its ID.
    /// </summary>
    /// <param name="id">The ID of the office to delete.</param>
    /// <returns>NoContent result.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteOffice(Guid id)
    {
        await _officeService.DeleteOfficeAsync(id);

        return NoContent();
    }
}