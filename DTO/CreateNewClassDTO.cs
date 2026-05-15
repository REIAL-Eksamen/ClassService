using System.ComponentModel.DataAnnotations;
using ClassService.Models;
namespace ClassService.DTO;

public class CreateNewClassDTO
{
    public int ClassId { get; set; }

    [Required, MaxLength(50)]
    public string ClassName { get; set; } = string.Empty;

    [Required, MaxLength(250)]
    public string ClassDescription { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string ClassType { get; set; } = string.Empty;
    
    public static CreateNewClassDTO FromClass(Class c) => new()
    {
        ClassId          = c.ClassId,
        ClassName        = c.ClassName ?? string.Empty,
        ClassDescription = c.ClassDescription ?? string.Empty,
        ClassType        = c.ClassType ?? string.Empty,
    };
}