using ClassService.DTOs;
using ClassService.Services;
using ClassService.Clients;
using ClassService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClassService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassController : ControllerBase
{
    private readonly Services.ClassService _service;
    private readonly InstructorClient _instructorClient;

    public ClassController(Services.ClassService service, InstructorClient instructorClient)
    {
        _service = service;
        _instructorClient = instructorClient;
    }

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

    [HttpPost]
    public async Task<ActionResult<Class>> Create([FromBody] CreateClassDTO? dto)
    {
        if (dto is null)
            return BadRequest("Request body cannot be null.");

        /* var instructor = await _instructorClient.GetInstructorAsync(int.Parse(dto.InstructorId));
        if (instructor is null)
            return NotFound($"Instruktør {dto.InstructorId} findes ikke."); */

        try
        {
            var newClass = await _service.CreateFromTemplateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newClass.Id }, newClass);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateClassDTO? dto)
    {
        if (dto is null)
            return BadRequest("Request body cannot be null.");

        var instructor = await _instructorClient.GetInstructorAsync(int.Parse(dto.InstructorId));
        if (instructor is null)
            return NotFound($"Instruktør {dto.InstructorId} findes ikke.");

        try
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}