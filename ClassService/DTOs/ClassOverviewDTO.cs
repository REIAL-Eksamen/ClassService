namespace ClassService.DTOs;

public class ClassOverviewDto
{
    public string Id { get; set; } = "";
    public string CenterName { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string InstructorFirstName { get; set; } = "";
    public string InstructorLastName { get; set; } = "";
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Status { get; set; } = "";
    public string ClassroomName { get; set; } = "";
    public int Capacity { get; set; }
}