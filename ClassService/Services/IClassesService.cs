using ClassService.Models;
using ClassService.DTOs;

namespace ClassService.Services;

public interface IClassesService
{
    Task<List<Class>> GetAllAsync();
    Task<Class?> GetByIdAsync(string id);
    Task<List<Class>> GetByCenterAsync(string centerId);
    Task<Class> CreateFromTemplateAsync(CreateClassDTO dto);
    Task UpdateAsync(string id, CreateClassDTO dto);
    Task DeleteAsync(string id);
    Task AddMemberAsync(string classId, string userId);
}