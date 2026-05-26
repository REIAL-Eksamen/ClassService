using ClassService.DTOs;
using ClassService.Models;
using ClassService.Repositories;

namespace ClassService.Services;

public class ClassTemplateService
{
    private readonly IClassTemplateRepository _templates;

    public ClassTemplateService(IClassTemplateRepository templates)
    {
        _templates = templates;
    }

    public Task<List<ClassTemplate>> GetAllAsync() =>
        _templates.GetAllAsync();

    public Task<ClassTemplate?> GetByIdAsync(string id) =>
        _templates.GetByIdAsync(id);

    public Task CreateAsync(ClassTemplate template) =>
        _templates.InsertAsync(template);

    public async Task UpdateAsync(string id, CreateClassTemplateDTO dto)
    {
        var existing = await _templates.GetByIdAsync(id)
                       ?? throw new KeyNotFoundException($"Template {id} findes ikke.");

        existing.ClassName = dto.ClassName;
        existing.ClassDescription = dto.ClassDescription;
        existing.ClassType = dto.ClassType;

        await _templates.ReplaceAsync(id, existing);
    }

    public async Task DeleteAsync(string id)
    {
        var deleted = await _templates.DeleteAsync(id);
        if (!deleted)
            throw new KeyNotFoundException($"Template {id} findes ikke.");
    }
}