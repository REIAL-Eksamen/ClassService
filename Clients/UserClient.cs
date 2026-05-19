using ClassService.DTO;

namespace ClassService.Clients;

public class UserClient
{
    private readonly HttpClient _http;

    public UserClient(HttpClient http) => _http = http;

    public async Task<List<UserDTO>> GetUsersAsync(List<string> ids)
    {
        if (ids.Count == 0)
            return [];

        var query = string.Join("&", ids.Select(id => $"ids={id}"));
        var response = await _http.GetAsync($"api/users?{query}");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<UserDTO>>() ?? [];
    }
}