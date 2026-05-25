using ClassService.DTOs;
using ClassService.Services;
using ClassService.Clients;
using ClassService.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ClassService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassController : ControllerBase
{
    private readonly Services.ClassService _service;
    private readonly InstructorClient _instructorClient;
    private readonly IMongoCollection<Classroom> _classroomCollection;
    private readonly IMongoCollection<Class> _classCollection;

    public ClassController(Services.ClassService service, InstructorClient instructorClient, IMongoClient mongoClient)
    {
        _service = service;
        _instructorClient = instructorClient;
        var database = mongoClient.GetDatabase("ClassDb");
        _classroomCollection = database.GetCollection<Classroom>("Classrooms");
        _classCollection = database.GetCollection<Class>("Classes");
    }

    // Henter alle hold
    [HttpGet]
    public async Task<ActionResult<List<Class>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    // Henter et specifikt hold via ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Class>> GetById(string id)
    {
        var c = await _service.GetByIdAsync(id);
        return c is null ? NotFound($"Class {id} findes ikke.") : Ok(c);
    }

    // Henter alle hold tilknyttet et specifikt center
    [HttpGet("bycenter/{centerId}")]
    public async Task<ActionResult<List<Class>>> GetByCenter(string centerId) =>
        Ok(await _service.GetByCenterAsync(centerId));

    // Opretter et nyt hold ud fra en DTO
    [HttpPost]
    public async Task<ActionResult<Class>> Create([FromBody] CreateClassDTO? dto)
    {
        if (dto is null)
            return BadRequest("Request body cannot be null.");

        var instructor = await _instructorClient.GetInstructorAsync(dto.InstructorId);
        if (instructor is null)
            return NotFound($"Instruktør {dto.InstructorId} findes ikke.");

        if (instructor.CenterId != dto.CenterId)
            return BadRequest($"Instruktør {dto.InstructorId} tilhører ikke center {dto.CenterId}.");

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

    // Opdaterer et eksisterende hold via ID
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateClassDTO? dto)
    {
        if (dto is null)
            return BadRequest("Request body cannot be null.");

        var instructor = await _instructorClient.GetInstructorAsync(dto.InstructorId);
        if (instructor is null)
            return NotFound($"Instruktør {dto.InstructorId} findes ikke.");

        if (instructor.CenterId != dto.CenterId)
            return BadRequest($"Instruktør {dto.InstructorId} tilhører ikke center {dto.CenterId}.");

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

    // Sletter et hold via ID
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

    // Tilknytter et classroom til et hold og gemmer navn og kapacitet fra Classrooms collectionen
    [HttpPost("{classId}/classrooms/{classroomId}")]
    public async Task<IActionResult> AddClassroom(string classId, string classroomId)
    {
        var classroom = await _classroomCollection.Find(c => c.Id == classroomId).FirstOrDefaultAsync();
        if (classroom == null) return NotFound("Classroom ikke fundet");

        var classroomDto = new ClassroomDto
        {
            ClassroomId = classroom.Id,
            ClassroomName = classroom.ClassroomName,
            Capacity = classroom.Capacity
        };

        var update = Builders<Class>.Update
            .Set(c => c.Classroom, classroomDto);

        await _classCollection.UpdateOneAsync(c => c.Id == classId, update);
        return Ok();
    }
}