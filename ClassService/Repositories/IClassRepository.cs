using ClassService.Models;

namespace ClassService.Repositories;

// Definerer hvilke databasehandlinger resten af systemet kan bruge til konkrete hold.
public interface IClassRepository
{
    Task<List<Class>> GetAllAsync();
    Task<Class?> GetByIdAsync(string id);
    Task<List<Class>> GetByCenterAsync(string centerId);
    Task InsertAsync(Class c);
    Task ReplaceAsync(string id, Class c);
    Task<bool> DeleteAsync(string id);
    Task UpdateAsync(string id, Class c);
}