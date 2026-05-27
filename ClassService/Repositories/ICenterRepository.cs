using ClassService.Models;

namespace ClassService.Repositories;

public interface ICenterRepository
{
    Task<Center?> GetByIdAsync(string id);
    Task<List<Center>> GetAllAsync();
}