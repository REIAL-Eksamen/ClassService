using ClassService.DTO;

namespace ClassService.Clients;

public class AdminClient
{
    private readonly HttpClient _http;

    public AdminClient(HttpClient http) => _http = http;

    public async Task<InstructorDTO?> GetInstructorAsync(string id)
    {
        var response = await _http.GetAsync($"api/instructors/{id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<InstructorDTO>();
    }
}