using ClassService.Models;
using ClassService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClassService.Controllers;

[ApiController]
[Route("[controller]")]
public class ClassTemplateController : ControllerBase
{
    private readonly ClassTemplateService _service;

    public ClassTemplateController(ClassTemplateService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<ClassTemplate>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<ClassTemplate>> GetById(string id)
    {
        var template = await _service.GetByIdAsync(id);
        return template is null ? NotFound($"Skabelon {id} findes ikke.") : Ok(template);
    }

    [HttpPost]
    public async Task<ActionResult<ClassTemplate>> Create([FromBody] ClassTemplate? template)
    {
        if (template is null)
            return BadRequest("Request body cannot be null.");

        await _service.CreateAsync(template);
        return CreatedAtAction(nameof(GetById), new { id = template.Id }, template);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ClassTemplate? updated)
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
    }
}