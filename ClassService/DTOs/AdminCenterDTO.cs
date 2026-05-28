using System.Text.Json.Serialization;

namespace ClassService.DTOs;

// Bruges til at modtage admin-/instruktøroplysninger fra AdminService.
public class AdminCenterDTO
{
    // Mapper JSON-feltet "id" til AdminId, så navnet passer bedre i ClassService.
    [JsonPropertyName("id")]
    public string AdminId { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Role { get; set; } = "";
    public string CenterId { get; set; } = "";
}