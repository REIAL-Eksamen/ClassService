using ClassService.DTOs;
using ClassService.Models;
using ClassService.Repositories;

namespace ClassService.Services;

public class ClassTemplateService : IClassTemplateService
{
    private readonly IClassTemplateRepository _templates;

    public ClassTemplateService(IClassTemplateRepository templates)
    {
        _templates = templates;
    }

    // hent alle templates
    public Task<List<ClassTemplate>> GetAllAsync() =>
        _templates.GetAllAsync();

    //hent en template
    public Task<ClassTemplate?> GetByIdAsync(string id) =>
        _templates.GetByIdAsync(id);

    //opret template
    public Task CreateAsync(ClassTemplate template) =>
        _templates.InsertAsync(template);

    //rediger template
    public async Task UpdateAsync(string id, CreateClassTemplateDTO dto)
    {
        // En template beskriver selve holdtypen, fx Yoga eller Spinning, og kan genbruges til flere konkrete hold.
        var existing = await _templates.GetByIdAsync(id)
                       ?? throw new KeyNotFoundException($"Template {id} findes ikke.");

        existing.ClassName = dto.ClassName;
        existing.ClassDescription = dto.ClassDescription;
        existing.ClassType = dto.ClassType;

        await _templates.ReplaceAsync(id, existing);
    }

    //slet template
    public async Task DeleteAsync(string id)
    {
        // DeleteAsync returnerer false, hvis der ikke blev slettet noget, så vi kan give controlleren en tydelig fejl.
        var deleted = await _templates.DeleteAsync(id);
        if (!deleted)
            throw new KeyNotFoundException($"Template {id} findes ikke.");
    }
}