using ClassService.DTOs;

namespace ClassService.Clients;

public class InstructorClient
{
    private readonly HttpClient _httpClient;

    public InstructorClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<InstructorDTO?> GetInstructorAsync(int instructorId)
    {
        var response = await _httpClient.GetAsync($"api/admin/{instructorId}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<InstructorDTO>();
    }
}