namespace ClassService.Models;

public class Class
{
    public int ClassId { get; set; }
    public string? ClassName { get; set; }
    public string? ClassDescription { get; set; }
    public string? ClassType { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int ClassCapacity { get; set; }
    public bool ClassStatus { get; set; }
    public List<string>? Attendees { get; set; }
    public List<string>? Waitlist { get; set; }
}