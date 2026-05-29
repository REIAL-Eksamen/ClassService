using ClassService.Models;

namespace ClassService.Repositories;

// Definerer hvilke databasehandlinger resten af systemet kan bruge til holdtemplates.
public interface IClassTemplateRepository
{
    Task<List<ClassTemplate>> GetAllAsync();
    Task<ClassTemplate?> GetByIdAsync(string id);
    Task InsertAsync(ClassTemplate template);
    Task ReplaceAsync(string id, ClassTemplate template);
    Task<bool> DeleteAsync(string id);
}