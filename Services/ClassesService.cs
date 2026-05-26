using ClassService.Repositories;
using ClassService.Models;
using ClassService.DTOs;

namespace ClassService.Services;
public class ClassesService
{
    private readonly IClassRepository _classes;
    private readonly IClassTemplateRepository _templates;
    private readonly IClassroomRepository _classrooms;

    public ClassesService(
        IClassRepository classes,
        IClassTemplateRepository templates,
        IClassroomRepository classrooms)
    {
        _classes = classes;
        _templates = templates;
        _classrooms = classrooms;
    }

    public Task<List<Class>> GetAllAsync() => _classes.GetAllAsync();
    public Task<Class?> GetByIdAsync(string id) => _classes.GetByIdAsync(id);
    public Task<List<Class>> GetByCenterAsync(string centerId) => _classes.GetByCenterAsync(centerId);

    public async Task<Class> CreateFromTemplateAsync(CreateClassDTO dto)
    {
        if (dto.Classroom is null)
            throw new ArgumentException("Classroom må ikke være null.");

        var template = await _templates.GetByIdAsync(dto.ClassTemplateId)
            ?? throw new KeyNotFoundException($"Template {dto.ClassTemplateId} findes ikke.");

        var classroom = await _classrooms.GetByIdAndCenterAsync(dto.Classroom.ClassroomId, dto.CenterId);
        if (classroom is null)
            throw new KeyNotFoundException($"Classroom {dto.Classroom.ClassroomId} tilhører ikke center {dto.CenterId}.");

        var newClass = new Class
        {
            ClassName = template.ClassName,
            ClassDescription = template.ClassDescription,
            ClassType = template.ClassType,
            InstructorId = dto.InstructorId,
            CenterId = dto.CenterId,
            Classroom = dto.Classroom,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Status = ClassStatus.Scheduled
        };

        await _classes.InsertAsync(newClass);
        return newClass;
    }

    public async Task UpdateAsync(string id, CreateClassDTO dto)
    {
        if (dto.Classroom is null)
            throw new ArgumentException("Classroom må ikke være null.");

        var existing = await _classes.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Class {id} findes ikke.");

        var template = await _templates.GetByIdAsync(dto.ClassTemplateId)
            ?? throw new KeyNotFoundException($"Template {dto.ClassTemplateId} findes ikke.");

        var classroom = await _classrooms.GetByIdAndCenterAsync(dto.Classroom.ClassroomId, dto.CenterId);
        if (classroom is null)
            throw new KeyNotFoundException($"Classroom {dto.Classroom.ClassroomId} tilhører ikke center {dto.CenterId}.");

        existing.ClassName = template.ClassName;
        existing.ClassDescription = template.ClassDescription;
        existing.ClassType = template.ClassType;
        existing.InstructorId = dto.InstructorId;
        existing.CenterId = dto.CenterId;
        existing.Classroom = dto.Classroom;
        existing.StartTime = dto.StartTime;
        existing.EndTime = dto.EndTime;

        await _classes.ReplaceAsync(id, existing);
    }

    public async Task DeleteAsync(string id)
    {
        var deleted = await _classes.DeleteAsync(id);
        if (!deleted)
            throw new KeyNotFoundException($"Class {id} findes ikke.");
    }
}