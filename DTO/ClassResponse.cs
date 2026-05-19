using ClassService.Models;

namespace ClassService.DTO;

public class ClassResponse
{
    public string? Id { get; set; }
    public string ClassName { get; set; } = "";
    public string ClassDescription { get; set; } = "";
    public string ClassType { get; set; } = "";

    // Fuldt objekt hentet fra Admin Service
    public InstructorDTO? Instructor { get; set; }
    public string CenterId { get; set; } = "";
    public string ClassroomId { get; set; } = "";

    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? ClassCapacity { get; set; }
    public ClassStatus Status { get; set; }

    // Fulde objekter hentet fra User Service
    public List<UserDTO> Attendees { get; set; } = [];
    public List<UserDTO> Waitlist { get; set; } = [];
}