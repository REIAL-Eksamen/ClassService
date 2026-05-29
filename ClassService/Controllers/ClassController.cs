using System.Security.Claims;
using ClassService.DTOs;
using FitLife.Events;
using ClassService.Services;
using ClassService.Clients;
using ClassService.Models;
using ClassService.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ClassService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassController : ControllerBase
{
    // Cache-key bruges til at gemme holdoversigten midlertidigt.
    private const string OverviewCacheKey = "class_overview";

    private readonly IClassesService _service; // Service-laget håndterer forretningslogikken for hold.
    private readonly IClassTemplateService _templateService; // Service-laget håndterer forretningslogikken for hold.
    private readonly IAdminClient _adminClient; // Bruges til at hente instruktøroplysninger fra AdminService.
    private readonly ICenterRepository _centerRepository; // Bruges til at hente center og lokaleoplysninger fra databasen.
    private readonly IClassTemplateRepository _templateRepository; // Bruges til at hente holdtemplate-oplysninger fra databasen.
    private readonly IMemoryCache _cache; // Bruges til at cache holdoversigten, så den ikke skal bygges op hver gang.
    private readonly IPublishEndpoint _publishEndpoint; // Bruges til at sende events til andre services via MassTransit.

    // Constructor injection: controlleren får sine dependencies gennem dependency injection.
    public ClassController(
        IClassesService service,
        IAdminClient adminClient,
        ICenterRepository centerRepository,
        IClassTemplateRepository templateRepository,
        IMemoryCache cache,
        IPublishEndpoint publishEndpoint)
    {
        _service = service;
        _adminClient = adminClient;
        _centerRepository = centerRepository;
        _templateRepository = templateRepository;
        _cache = cache;
        _publishEndpoint = publishEndpoint;
    }

    // GET /api/Class
    // Henter alle konkrete hold.
    [HttpGet]
    public async Task<ActionResult<List<Class>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Class>> GetById(string id)
    {
        var c = await _service.GetByIdAsync(id);
        return c is null ? NotFound($"Class {id} findes ikke.") : Ok(c);
    }

    [HttpGet("bycenter/{centerId}")]
    public async Task<ActionResult<List<Class>>> GetByCenter(string centerId) =>
        Ok(await _service.GetByCenterAsync(centerId));

    // POST /api/Class
    // Opretter et nyt hold ud fra en template, center, instruktør, lokale og tidspunkt.
    [HttpPost]
    public async Task<ActionResult<Class>> Create([FromBody] CreateClassDTO? dto)
    {
        if (dto is null)
            return BadRequest("Request body cannot be null.");

        try
        {
            var newClass = await _service.CreateFromTemplateAsync(dto);
            _cache.Remove(OverviewCacheKey); // Når hold ændres, fjernes cache, så overview bliver opdateret næste gang.
            return CreatedAtAction(nameof(GetById), new { id = newClass.Id }, newClass);
        }
        catch (KeyNotFoundException e) { return NotFound(e.Message); }
        catch (BadHttpRequestException e) { return BadRequest(e.Message); }
    }

    // PUT /api/Class/{id}
    // Opdaterer et eksisterende hold.
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateClassDTO? dto)
    {
        if (dto is null)
            return BadRequest("Request body cannot be null.");

        try
        {
            await _service.UpdateAsync(id, dto);
            _cache.Remove(OverviewCacheKey);
            return NoContent();
        }
        catch (KeyNotFoundException e) { return NotFound(e.Message); }
        catch (BadHttpRequestException e) { return BadRequest(e.Message); }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _service.DeleteAsync(id);
            _cache.Remove(OverviewCacheKey);
            return NoContent();
        }
        catch (KeyNotFoundException e) { return NotFound(e.Message); }
    }
    
    // PATCH /api/Class/{id}/cancel
    // Aflyser et helt hold og sender et event til andre services.
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(string id)
    {
        var cancelled = await _service.CancelAsync(id);

        if (cancelled is null)
            return NotFound($"Class {id} findes ikke.");

        _cache.Remove(OverviewCacheKey);
        // Sender event, så fx BookingService kan reagere på at holdet er aflyst.
        await _publishEndpoint.Publish(new ClassCancelledEvent { ClassId = id });

        return NoContent();
    }

     // GET /api/Class/{id}/overview
    // Henter en samlet visning af ét hold med center, template, instruktør og lokale.    
    [HttpGet("{id}/overview")]
    public async Task<ActionResult<ClassOverviewDto>> GetOverviewById(string id)
    {
        var classItem = await _service.GetByIdAsync(id);
        if (classItem is null)
            return NotFound($"Class {id} findes ikke.");

        var center = await _centerRepository.GetByIdAsync(classItem.CenterId);
        var template = await _templateRepository.GetByIdAsync(classItem.TemplateId);
        var admin = await _adminClient.GetAdminAsync(classItem.InstructorId);
        var classroom = center?.Classrooms.FirstOrDefault(r => r.ClassroomId == classItem.ClassroomId);

        // Samler data fra flere steder til én DTO, som frontend kan bruge.
        return Ok(new ClassOverviewDto
        {
            Id = classItem.Id ?? "",
            CenterName = center?.Name ?? "",
            ClassName = template?.ClassName ?? "",
            ClassDescription = template?.ClassDescription ?? "",
            ClassType = template?.ClassType ?? "",
            InstructorFirstName = admin?.FirstName ?? "",
            InstructorLastName = admin?.LastName ?? "",
            InstructorName = $"{admin?.FirstName} {admin?.LastName}".Trim(),
            StartTime = classItem.StartTime,
            EndTime = classItem.EndTime,
            Status = classItem.Status.ToString(),
            ClassroomName = classroom?.Name ?? "",
            Capacity = classroom?.Capacity ?? 0
        });
    }

    // GET /api/Class/overview
    // Henter en samlet liste over hold til frontend.
    [HttpGet("overview")]
    public async Task<ActionResult<List<ClassOverviewDto>>> GetOverview()
    {
        // Hvis overview allerede ligger i cache, returneres det direkte.
        if (_cache.TryGetValue(OverviewCacheKey, out List<ClassOverviewDto>? cached))
            return Ok(cached);

        var classes = await _service.GetAllAsync();
        var result = new List<ClassOverviewDto>();

        // Bygger en samlet oversigt ved at hente data fra Class, Center, Template og AdminService.
        foreach (var classItem in classes)
        {
            var center = await _centerRepository.GetByIdAsync(classItem.CenterId);
            var template = await _templateRepository.GetByIdAsync(classItem.TemplateId);
            var admin = await _adminClient.GetAdminAsync(classItem.InstructorId);
            var classroom = center?.Classrooms.FirstOrDefault(r => r.ClassroomId == classItem.ClassroomId);

            result.Add(new ClassOverviewDto
            {
                Id = classItem.Id ?? "",
                CenterName = center?.Name ?? "",
                ClassName = template?.ClassName ?? "",
                ClassDescription = template?.ClassDescription ?? "",
                ClassType = template?.ClassType ?? "",
                InstructorFirstName = admin?.FirstName ?? "",
                InstructorLastName = admin?.LastName ?? "",
                InstructorName = $"{admin?.FirstName} {admin?.LastName}".Trim(),
                StartTime = classItem.StartTime,
                EndTime = classItem.EndTime,
                Status = classItem.Status.ToString(),
                ClassroomName = classroom?.Name ?? "",
                Capacity = classroom?.Capacity ?? 0
            });
        }

        // Gemmer overview i cache i 5 minutter for at mindske gentagne kald.
        _cache.Set(OverviewCacheKey, result, TimeSpan.FromMinutes(5));
        return Ok(result);
    }
    
    // POST /api/Class/{classId}/members
    // Tilføjer den loggede bruger som medlem på et hold.
    [Authorize]
    [HttpPost("{classId}/members")]
    public async Task<IActionResult> AddMember(string classId)
    {
        // Finder brugerens id fra JWT-tokenet.
        var userId =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        if (userId is null)
            return Unauthorized();

        await _service.AddMemberAsync(classId, userId);

        return NoContent();
    }
}