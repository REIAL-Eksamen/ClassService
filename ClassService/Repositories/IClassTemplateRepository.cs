using ClassService.Models;

namespace ClassService.Repositories;

public interface IClassTemplateRepository
{
    Task<List<ClassTemplate>> GetAllAsync();
    Task<ClassTemplate?> GetByIdAsync(string id);
    Task InsertAsync(ClassTemplate template);
    Task ReplaceAsync(string id, ClassTemplate template);
    Task<bool> DeleteAsync(string id);
}