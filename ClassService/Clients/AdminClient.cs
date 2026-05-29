using ClassService.DTOs;

namespace ClassService.Clients;

// HTTP-klient der bruges til at hente admin- og instruktøroplysninger fra AdminService.
public class AdminClient : IAdminClient
{
    private readonly HttpClient _httpClient;

    // HttpClient bliver sat op i Program.cs med base address til AdminService.
    public AdminClient(HttpClient httpClient) => _httpClient = httpClient;

    // Henter én admin/instruktør ud fra id.
    public async Task<AdminCenterDTO?> GetAdminAsync(string adminId)
    {
        // Kalder AdminService endpointet: /admin/{adminId}
        var response = await _httpClient.GetAsync($"admin/{adminId}");
        if (!response.IsSuccessStatusCode)
            return null; // Hvis AdminService ikke finder adminen, returnerer vi null.

        // Læser JSON-svaret fra AdminService og mapper det til AdminCenterDTO.
        return await response.Content.ReadFromJsonAsync<AdminCenterDTO>();
    }

    // Henter alle instruktører/admins, der hører til et bestemt center.
    public async Task<List<AdminCenterDTO>> GetInstructorsByCenterAsync(string centerId)
    {
        // Kalder AdminService endpointet: /admin/bycenter/{centerId}
        var response = await _httpClient.GetAsync($"admin/bycenter/{centerId}");
        if (!response.IsSuccessStatusCode)
            return new List<AdminCenterDTO>();

        return await response.Content.ReadFromJsonAsync<List<AdminCenterDTO>>() ?? new List<AdminCenterDTO>();
    }
}