namespace ClassService.Models;

public class Center
{
    public int CenterId { get; set; }
    public string? CenterName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public int PostalCode { get; set; }
    public int MaxCapacity { get; set; }
}