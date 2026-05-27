using ClassService.Models;

public class ClassViewModel
{
    public string? Id { get; set; }

    // Fra ClassTemplate
    public string ClassName { get; set; } = "";
    public string ClassDescription { get; set; } = "";
    public string ClassType { get; set; } = "";

    // Fra Admin
    public string InstructorFirstName { get; set; } = "";
    public string InstructorLastName { get; set; } = "";

    // Fra Center
    public string CenterName { get; set; } = "";

    // Fra Center.Classrooms[]
    public string ClassroomName { get; set; } = "";
    public int ClassroomCapacity { get; set; }

    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public ClassStatus Status { get; set; }
}