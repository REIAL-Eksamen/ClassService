using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using ClassService.Models;
using ClassService.DTOs;

namespace ClassService.Controllers;

[ApiController]
[Route("api/centers")]
public class CenterController : ControllerBase
{
    private readonly IMongoCollection<Center> _centerCollection;

    public CenterController(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("ClassDb");
        _centerCollection = database.GetCollection<Center>("Centers");
    }

    // Tilføjer en admin til et center. Modtager AdminId, navn og rolle fra AdminService via AdminCenterDto.
    // Tjekker om admin allerede er tilknyttet centeret før tilføjelse.
    [HttpPost("{centerId}/admins")]
    public async Task<IActionResult> AddAdmin(string centerId, [FromBody] AdminCenterDto dto)
    {
        var center = await _centerCollection.Find(c => c.Id == centerId).FirstOrDefaultAsync();
        if (center == null) return NotFound("Center ikke fundet");

        bool findes = center.Admins.Any(a => a.AdminId == dto.AdminId);
        if (findes) return Conflict("Admin er allerede tilknyttet centeret");

        var update = Builders<Center>.Update
            .Push(c => c.Admins, new CenterAdmin 
            { 
                AdminId = dto.AdminId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = dto.Role
            });

        await _centerCollection.UpdateOneAsync(c => c.Id == centerId, update);
        return Ok();
    }

    // Henter alle admins på et center med rollen "Instruktør".
    // Bruges ved holdoprettelse så kun gyldige instruktører kan vælges.
    [HttpGet("{centerId}/instruktører")]
    public async Task<IActionResult> GetInstructors(string centerId)
    {
        var center = await _centerCollection.Find(c => c.Id == centerId).FirstOrDefaultAsync();
        if (center == null) return NotFound("Center ikke fundet");

        var instructors = center.Admins
            .Where(a => a.Role == "Instruktør")
            .ToList();

        return Ok(instructors);
    }
}