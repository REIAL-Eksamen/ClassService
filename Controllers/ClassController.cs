using ClassService.Models;
using ClassService.DTO;
using ClassService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClassService.Controllers;

[ApiController]
[Route("[controller]")]
public class ClassController : ControllerBase
{
    private readonly Services.ClassService _classService;
    private readonly ClassTemplateService _templateService;

    public ClassController(Services.ClassService classService, ClassTemplateService templateService)
    {
        _classService = classService;
        _templateService = templateService;
    }

    // ── Hold ────────────────────────────────────────────────

    [HttpGet("scheduledclasses")]
    public async Task<ActionResult<List<ClassResponse>>> GetAllScheduledClasses()
    {
        var classes = await _classService.GetAllAsync();
        return Ok(classes);
    }

    [HttpGet("scheduledclasses/{id}")]
    public async Task<ActionResult<ClassResponse>> GetScheduledClassById(string id)
    {
        try
        {
            var result = await _classService.GetWithDetailsAsync(id);
            return Ok(result);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet("scheduledclasses/bycenter/{centerId}")]
    public async Task<ActionResult<List<Class>>> GetClassesByCenter(string centerId)
    {
        var classes = await _classService.GetByCenterAsync(centerId);
        return Ok(classes);
    }

    [HttpPost("scheduledclasses")]
    public async Task<ActionResult<Class>> CreateScheduledClass([FromBody] CreateClassRequest? request)
    {
        if (request is null)
            return BadRequest("Request body cannot be null.");

        try
        {
            var created = await _classService.CreateAsync(request);
            return CreatedAtAction(nameof(GetScheduledClassById), new { id = created.Id }, created);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("scheduledclasses/{id}")]
    public async Task<IActionResult> UpdateScheduledClass(string id, [FromBody] CreateClassRequest? request)
    {
        if (request is null)
            return BadRequest("Request body cannot be null.");

        try
        {
            await _classService.UpdateAsync(id, request);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("scheduledclasses/{id}")]
    public async Task<IActionResult> DeleteScheduledClass(string id)
    {
        try
        {
            await _classService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    // ── Skabeloner ──────────────────────────────────────────

    [HttpGet("templates")]
    public async Task<ActionResult<List<ClassTemplate>>> GetAllTemplates()
    {
        var templates = await _templateService.GetAllAsync();
        return Ok(templates);
    }

    [HttpGet("templates/{id}")]
    public async Task<ActionResult<ClassTemplate>> GetTemplateById(string id)
    {
        var template = await _templateService.GetByIdAsync(id);
        return template is null ? NotFound($"Skabelon {id} findes ikke.") : Ok(template);
    }

    [HttpPost("templates")]
    public async Task<ActionResult<ClassTemplate>> CreateTemplate([FromBody] ClassTemplate? template)
    {
        if (template is null)
            return BadRequest("Request body cannot be null.");

        await _templateService.CreateAsync(template);
        return CreatedAtAction(nameof(GetTemplateById), new { id = template.Id }, template);
    }

    [HttpPut("templates/{id}")]
    public async Task<IActionResult> UpdateTemplate(string id, [FromBody] ClassTemplate? updated)
    {
        if (updated is null)
            return BadRequest("Request body cannot be null.");

        try
        {
            await _templateService.UpdateAsync(id, updated);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("templates/{id}")]
    public async Task<IActionResult> DeleteTemplate(string id)
    {
        var existing = await _templateService.GetByIdAsync(id);
        if (existing is null)
            return NotFound($"Skabelon {id} findes ikke.");

        await _templateService.DeleteAsync(id);
        return NoContent();
    }
}