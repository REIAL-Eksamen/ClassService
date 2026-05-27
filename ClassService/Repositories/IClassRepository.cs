using ClassService.Models;

namespace ClassService.Repositories;

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