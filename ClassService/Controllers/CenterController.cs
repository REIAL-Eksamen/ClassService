using Microsoft.AspNetCore.Mvc;
using ClassService.Models;
using ClassService.Clients;
using ClassService.Repositories;

namespace ClassService.Controllers;

[ApiController]
[Route("api/centers")]
public class CenterController : ControllerBase
{
    private readonly ICenterRepository _centerRepository; // Repository bruges til at hente centre og lokaler fra databasen.
    private readonly IAdminClient _adminClient; // AdminClient bruges til at hente instruktører fra AdminService.

    // Dependencies bliver givet til controlleren gennem dependency injection.
    public CenterController(ICenterRepository centerRepository, IAdminClient adminClient)
    {
        _centerRepository = centerRepository;
        _adminClient = adminClient;
    }

    // GET /api/centers/{centerId}/classrooms
    // Henter alle lokaler på et bestemt center.
    [HttpGet("{centerId}/classrooms")]
    public async Task<IActionResult> GetClassrooms(string centerId)
    {
        // Finder centeret i databasen.
        var center = await _centerRepository.GetByIdAsync(centerId);
        if (center == null) return NotFound("Center ikke fundet"); // Hvis centeret ikke findes, returneres 404 Not Found.

        return Ok(center.Classrooms);
    }
    
    // GET /api/centers/all
    // Henter alle centre.
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        // Finder alle centre i databasen.
        var centers = await _centerRepository.GetAllAsync();
        return Ok(centers);
    }

    // Henter alle instruktører på et center via AdminService
    [HttpGet("{centerId}/instructors")]
    public async Task<IActionResult> GetInstructors(string centerId)
    {
        var center = await _centerRepository.GetByIdAsync(centerId);
        if (center == null) return NotFound("Center ikke fundet");

        // Henter instruktørerne fra AdminService gennem AdminClient.
        var instructors = await _adminClient.GetInstructorsByCenterAsync(centerId);
        return Ok(instructors);
    }
}