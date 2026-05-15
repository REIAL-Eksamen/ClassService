using ClassService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClassService.Controllers;

public class ClassController
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
            ClassStatus = true,
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
}