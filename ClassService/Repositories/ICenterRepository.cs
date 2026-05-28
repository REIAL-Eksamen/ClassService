using ClassService.Models;

namespace ClassService.Repositories;

// Definerer hvilke databasehandlinger resten af systemet kan bruge til centre.
public interface ICenterRepository
{
    Task<Center?> GetByIdAsync(string id);
    Task<List<Center>> GetAllAsync();
}