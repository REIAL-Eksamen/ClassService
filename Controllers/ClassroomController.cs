using ClassService.Models;
using ClassService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClassService.Controllers;

[ApiController]
[Route("[controller]")]
public class ClassroomController : ControllerBase
{
    private readonly ClassroomService _service;

    public ClassroomController(ClassroomService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<Classroom>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Classroom>> GetById(string id)
    {
        var classroom = await _service.GetByIdAsync(id);
        return classroom is null ? NotFound($"Lokale {id} findes ikke.") : Ok(classroom);
    }

    [HttpGet("bycenter/{centerId}")]
    public async Task<ActionResult<List<Classroom>>> GetByCenter(string centerId) =>
        Ok(await _service.GetByCenterAsync(centerId));

    [HttpPost]
    public async Task<ActionResult<Classroom>> Create([FromBody] Classroom? classroom)
    {
        if (classroom is null)
            return BadRequest("Request body cannot be null.");

        await _service.CreateAsync(classroom);
        return CreatedAtAction(nameof(GetById), new { id = classroom.ClassroomId }, classroom);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Classroom? updated)
    {
        if (updated is null)
            return BadRequest("Request body cannot be null.");

        try
        {
            await _service.UpdateAsync(id, updated);
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
        catch (InvalidOperationException e)
        {
            return Conflict(e.Message);
        }
    }
}