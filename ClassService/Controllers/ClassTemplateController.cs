using ClassService.DTOs;
using ClassService.Models;
using ClassService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClassService.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        return template is null ? NotFound($"Template {id} findes ikke.") : Ok(template);
    }

    [HttpPost]
    public async Task<ActionResult<ClassTemplate>> Create([FromBody] CreateClassTemplateDTO? dto)
    {
        if (dto is null)
            return BadRequest("Request body cannot be null.");

        var template = new ClassTemplate
        {
            ClassName = dto.ClassName,
            ClassDescription = dto.ClassDescription,
            ClassType = dto.ClassType
        };

        await _service.CreateAsync(template);
        return CreatedAtAction(nameof(GetById), new { id = template.Id }, template);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateClassTemplateDTO? dto)
    {
        if (dto is null)
            return BadRequest("Request body cannot be null.");

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