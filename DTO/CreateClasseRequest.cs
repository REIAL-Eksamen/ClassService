using System.ComponentModel.DataAnnotations;

namespace ClassService.DTO;

public class CreateClassRequest
{
    public string? TemplateId { get; set; }
    
    [Required, MaxLength(50)]
    public string ClassName { get; set; } = "";

    [Required, MaxLength(250)]
    public string ClassDescription { get; set; } = "";

    [Required, MaxLength(50)]
    public string ClassType { get; set; } = "";

    [Required]
    public string InstructorId { get; set; } = "";

    
    [Required]
    public string CenterId { get; set; } = "";

    [Required]
    public string ClassroomId { get; set; } = "";
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    [Range(0, 100)]
    public int? ClassCapacity { get; set; }
}