using ClassService.Models;
using ClassService.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ClassService.Controllers;

[ApiController]
[Route("[controller]")]
public class ClassController : ControllerBase
{
    private static List<Class> _classes = new List<Class>()
    {
        new()
        {
            ClassId = 1,
            ClassName = "Yin Yoga",
            ClassDescription = "Den afslappende side af yoga, hvor du får udstukket hele kroppen",
            ClassType = "Yoga",
            StartTime = new DateTime(2026, 05, 24, 14, 30, 0),
            EndTime = new DateTime(2026, 05, 24, 15, 25, 0),
            ClassCapacity = 34,
            Status = ClassStatus.Scheduled ,
            Attendees = new List<string>()
            {
              "Lukas Sloth",
              "Rasmus Pedersen"
            },
            Waitlist =
            {
            }
        }
    };
    
    [HttpGet("classes")]
    public List<Class> GetAllClasses()
    {
        return _classes;
    }

    [HttpGet("classes/{ClassId}")]
    public Class? GetClassById(int ClassId)
    {
        return _classes.FirstOrDefault(c => c.ClassId == ClassId);
    }
    
    [HttpPost("newscheduledclass")]
    public ActionResult<Class> CreateNewScheduledClass([FromBody] Class? newClass)
    {
        if (newClass is null)
            return BadRequest("Request body cannot be null.");
        
        return CreatedAtAction(nameof(CreateNewScheduledClass), newClass);
    }

    [HttpPost("createnewclass")]
    public ActionResult<CreateNewClassDTO> MakeNewClass([FromBody] CreateNewClassDTO? newClassDto)
    {
        if (newClassDto is null)
            return BadRequest("Request body cannot be null.");

        Class newClass = new()
        {
            ClassName        = newClassDto.ClassName,
            ClassDescription = newClassDto.ClassDescription,
            ClassType        = newClassDto.ClassType,
        };

        // TODO: persist newClass to database, get generated ClassId back

        return CreatedAtAction(nameof(MakeNewClass), CreateNewClassDTO.FromClass(newClass));
    }
}