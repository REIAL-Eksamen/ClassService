using ClassService.Repositories;
using ClassService.Clients;
using ClassService.Models;
using ClassService.DTOs;

namespace ClassService.Services;

public class ClassesService : IClassesService
{
    private readonly IClassRepository _classes;
    private readonly IClassTemplateRepository _templates;
    private readonly ICenterRepository _centers;
    private readonly IAdminClient _adminClient;

    public ClassesService(
        IClassRepository classes,
        IClassTemplateRepository templates,
        ICenterRepository centers,
        IAdminClient adminClient)
    {
        _classes = classes;
        _templates = templates;
        _centers = centers;
        _adminClient = adminClient;
    }

    public Task<List<Class>> GetAllAsync() => _classes.GetAllAsync();
    public Task<Class?> GetByIdAsync(string id) => _classes.GetByIdAsync(id);
    public Task<List<Class>> GetByCenterAsync(string centerId) => _classes.GetByCenterAsync(centerId);

    public async Task<Class> CreateFromTemplateAsync(CreateClassDTO dto)
    {
        var template = await _templates.GetByIdAsync(dto.TemplateId)
            ?? throw new KeyNotFoundException($"Template {dto.TemplateId} findes ikke.");

        var center = await _centers.GetByIdAsync(dto.CenterId)
            ?? throw new KeyNotFoundException($"Center {dto.CenterId} findes ikke.");

        var classroom = center.Classrooms.FirstOrDefault(r => r.ClassroomId == dto.ClassroomId)
            ?? throw new KeyNotFoundException($"Classroom {dto.ClassroomId} tilhører ikke center {dto.CenterId}.");

        var admin = await _adminClient.GetAdminAsync(dto.InstructorId)
            ?? throw new KeyNotFoundException($"Admin {dto.InstructorId} findes ikke.");

        if (admin.CenterId != dto.CenterId)
            throw new BadHttpRequestException($"Admin {dto.InstructorId} tilhører ikke center {dto.CenterId}.");

        if (admin.Role != "Instruktør" && admin.Role != "Pt")
            throw new BadHttpRequestException($"Admin {dto.InstructorId} har ikke en instruktørrolle.");

        var newClass = new Class
        {
            TemplateId = dto.TemplateId,
            InstructorId = dto.InstructorId,
            CenterId = dto.CenterId,
            ClassroomId = dto.ClassroomId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Status = ClassStatus.Planlagt
        };

        await _classes.InsertAsync(newClass);
        return newClass;
    }

    public async Task UpdateAsync(string id, CreateClassDTO dto)
    {
        var existing = await _classes.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Class {id} findes ikke.");

        var template = await _templates.GetByIdAsync(dto.TemplateId)
            ?? throw new KeyNotFoundException($"Template {dto.TemplateId} findes ikke.");

        var center = await _centers.GetByIdAsync(dto.CenterId)
            ?? throw new KeyNotFoundException($"Center {dto.CenterId} findes ikke.");

        var classroom = center.Classrooms.FirstOrDefault(r => r.ClassroomId == dto.ClassroomId)
            ?? throw new KeyNotFoundException($"Classroom {dto.ClassroomId} tilhører ikke center {dto.CenterId}.");

        var admin = await _adminClient.GetAdminAsync(dto.InstructorId)
            ?? throw new KeyNotFoundException($"Admin {dto.InstructorId} findes ikke.");

        if (admin.CenterId != dto.CenterId)
            throw new BadHttpRequestException($"Admin {dto.InstructorId} tilhører ikke center {dto.CenterId}.");

        if (admin.Role != "Instruktør" && admin.Role != "Pt")
            throw new BadHttpRequestException($"Admin {dto.InstructorId} har ikke en instruktørrolle.");

        existing.TemplateId = dto.TemplateId;
        existing.InstructorId = dto.InstructorId;
        existing.CenterId = dto.CenterId;
        existing.ClassroomId = dto.ClassroomId;
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
    
    public async Task AddMemberAsync(string classId, string userId)
    {
        var c = await _classes.GetByIdAsync(classId)
                ?? throw new KeyNotFoundException("Class not found");

        if (c.UserIds.Contains(userId))
            throw new BadHttpRequestException("Member already enrolled");

        c.UserIds.Add(userId);

        await _classes.UpdateAsync(c.Id!, c);
    }
}