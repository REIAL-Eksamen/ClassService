using System.Text.Json.Serialization;

namespace ClassService.DTOs;

public class AdminCenterDTO
{
    [JsonPropertyName("id")]
    public string AdminId { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Role { get; set; } = "";
    public string CenterId { get; set; } = "";
}