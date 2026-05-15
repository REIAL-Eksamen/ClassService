using System.ComponentModel.DataAnnotations;

namespace ClassService.Models;

public enum ClassStatus {Scheduled, Active, Cancelled, Done}
public class Class
{
    public int ClassId { get; set; }
    [Required, MaxLength (50)]
    public string ClassName { get; set; }
    [Required, MaxLength(250)]
    public string ClassDescription { get; set; }
    [Required, MaxLength(50)]
    public string ClassType { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    [Range (0, 100)]
    public int? ClassCapacity { get; set; }
    public ClassStatus? Status { get; set; } =  ClassStatus.Scheduled;
    public List<string>? Attendees { get; set; } = [];
    public List<string>? Waitlist { get; set; } = [];
}