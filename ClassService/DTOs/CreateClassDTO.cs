namespace ClassService.DTOs;

public class CreateClassDTO
{
    public string TemplateId { get; set; } = "";
    public string CenterId { get; set; } = "";
    public string InstructorId { get; set; } = "";
    public string ClassroomId { get; set; } = "";
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}