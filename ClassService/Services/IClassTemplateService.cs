// ClassService/Services/IClassTemplateService.cs
using ClassService.Models;
using ClassService.DTOs;

namespace ClassService.Services;

// Definerer kontrakten for handlinger på holdtemplates,
// såsom at hente, oprette, opdatere og slette templates.
public interface IClassTemplateService
{
    Task<List<ClassTemplate>> GetAllAsync();
    Task<ClassTemplate?> GetByIdAsync(string id);
    Task CreateAsync(ClassTemplate template);
    Task UpdateAsync(string id, CreateClassTemplateDTO dto);
    Task DeleteAsync(string id);
}