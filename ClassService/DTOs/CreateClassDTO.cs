namespace ClassService.DTOs;

public class CreateClassDTO
{
    public string ClassTemplateId { get; set; } = "";
    public string InstructorId { get; set; } = "";
    public string CenterId { get; set; } = "";
    public ClassroomDto Classroom { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}