using ClassService.DTOs;
using ClassService.Models;
using ClassService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClassService.Controllers;

// Route bliver /api/ClassTemplate, fordi controllerens navn er ClassTemplateController.
[ApiController]
[Route("api/[controller]")]
public class ClassTemplateController : ControllerBase
{
    // Service-laget bruges til selve logikken omkring holdtemplates.
    private readonly IClassTemplateService _service;
    
    // Constructor injection: controlleren får sin service gennem dependency injection.
    public ClassTemplateController(IClassTemplateService service) => _service = service;

    // GET /api/ClassTemplate
    // Henter alle holdtemplates fra systemet.
    [HttpGet]
    public async Task<ActionResult<List<ClassTemplate>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    // GET /api/ClassTemplate/{id}
    // Henter én bestemt holdtemplate ud fra id.
    [HttpGet("{id}")]
    public async Task<ActionResult<ClassTemplate>> GetById(string id)
    {
        // Kalder service-laget for at finde templaten.
        var template = await _service.GetByIdAsync(id);
        
        // Hvis templaten ikke findes, returneres 404 Not Found.
        // Ellers returneres 200 OK med templaten.
        return template is null ? NotFound($"Template {id} findes ikke.") : Ok(template);
    }
    
    // POST /api/ClassTemplate
    // Opretter en ny holdtemplate
    [HttpPost]
    public async Task<ActionResult<ClassTemplate>> Create([FromBody] CreateClassTemplateDTO? dto)
    {
        // Hvis request body mangler, kan vi ikke oprette en template.
        if (dto is null)
            return BadRequest("Request body cannot be null.");

        // Mapper data fra DTO'en over i den model, der skal gemmes i databasen.
        var template = new ClassTemplate
        {
            ClassName = dto.ClassName,
            ClassDescription = dto.ClassDescription,
            ClassType = dto.ClassType
        };

        // Sender den nye template videre til service-laget.
        await _service.CreateAsync(template);
        return CreatedAtAction(nameof(GetById), new { id = template.Id }, template);
    }

    // PUT /api/ClassTemplate/{id}
    // Opdaterer en eksisterende holdtemplate.
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
    
    // DELETE /api/ClassTemplate/{id}
    // Sletter en holdtemplate ud fra id
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        //“Hvis service-laget siger, at noget ikke findes, så fang fejlen her og returnér 404 Not Found.”
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}