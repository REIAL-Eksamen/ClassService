using ClassService.DTOs;

namespace ClassService.Clients;

public class AdminClient : IAdminClient
{
    private readonly HttpClient _httpClient;

    public AdminClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<AdminCenterDTO?> GetAdminAsync(string adminId)
    {
        var response = await _httpClient.GetAsync($"admin/{adminId}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AdminCenterDTO>();
    }

    public async Task<List<AdminCenterDTO>> GetInstructorsByCenterAsync(string centerId)
    {
        var response = await _httpClient.GetAsync($"admin/bycenter/{centerId}");
        if (!response.IsSuccessStatusCode)
            return new List<AdminCenterDTO>();

        return await response.Content.ReadFromJsonAsync<List<AdminCenterDTO>>() ?? new List<AdminCenterDTO>();
    }
}