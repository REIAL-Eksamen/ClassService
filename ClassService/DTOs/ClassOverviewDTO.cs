namespace ClassService.DTOs;

// Bruges til at sende en samlet oversigt over hold til frontend.
public class ClassOverviewDto
{
    // Id på det konkrete hold, så frontend fx kan booke det rigtige hold.
    public string Id { get; set; } = "";
    // Navnet på centeret, hvor holdet foregår.
    public string CenterName { get; set; } = "";
    
    // Oplysninger om holdtypen fra ClassTemplate.
    public string ClassName { get; set; } = "";
    public string ClassDescription { get; set; } = "";
    public string ClassType { get; set; } = "";
    
    // Instruktørens oplysninger fra AdminService.
    public string InstructorFirstName { get; set; } = "";
    public string InstructorLastName { get; set; } = "";
    public string InstructorName { get; set; } = "";
    
    // Tidspunkt for hvornår holdet starter og slutter.
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    
    // Status for holdet, fx planlagt eller aflyst.
    public string Status { get; set; } = "";
    public string ClassroomName { get; set; } = "";
    public int Capacity { get; set; }
}