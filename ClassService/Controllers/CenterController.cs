using Microsoft.AspNetCore.Mvc;
using ClassService.Models;
using ClassService.Clients;
using ClassService.Repositories;

namespace ClassService.Controllers;

[ApiController]
[Route("api/centers")]
public class CenterController : ControllerBase
{
    private readonly ICenterRepository _centerRepository;
    private readonly IAdminClient _adminClient;

    public CenterController(ICenterRepository centerRepository, IAdminClient adminClient)
    {
        _centerRepository = centerRepository;
        _adminClient = adminClient;
    }

    // Henter alle classrooms på et center
    [HttpGet("{centerId}/classrooms")]
    public async Task<IActionResult> GetClassrooms(string centerId)
    {
        var center = await _centerRepository.GetByIdAsync(centerId);
        if (center == null) return NotFound("Center ikke fundet");

        return Ok(center.Classrooms);
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var centers = await _centerRepository.GetAllAsync();
        return Ok(centers);
    }

    // Henter alle instruktører på et center via AdminService
    [HttpGet("{centerId}/instructors")]
    public async Task<IActionResult> GetInstructors(string centerId)
    {
        var center = await _centerRepository.GetByIdAsync(centerId);
        if (center == null) return NotFound("Center ikke fundet");

        var instructors = await _adminClient.GetInstructorsByCenterAsync(centerId);
        return Ok(instructors);
    }
}