using ClassService.DTOs;
using ClassService.Models;
using MongoDB.Driver;

namespace ClassService.Services;

public class ClassService
{
    private readonly IMongoCollection<Class> _classes;
    private readonly IMongoCollection<ClassTemplate> _templates;

    public ClassService(IConfiguration config)
    {
        var client = new MongoClient(config["CosmosDB:AccountKey"]);
        var db = client.GetDatabase(config["MongoDB:Database"]);
        _classes = db.GetCollection<Class>("ClassCollection");
        _templates = db.GetCollection<ClassTemplate>("ClassTemplateCollection");
    }

    public Task<List<Class>> GetAllAsync() =>
        _classes.Find(_ => true).ToListAsync();

    public Task<Class?> GetByIdAsync(string id) =>
        _classes.Find(x => x.Id == id).FirstOrDefaultAsync();

    public Task<List<Class>> GetByCenterAsync(string centerId) =>
        _classes.Find(x => x.CenterId == centerId).ToListAsync();

    public async Task<Class> CreateFromTemplateAsync(CreateClassDTO dto)
    {
        var template = await _templates.Find(x => x.Id == dto.ClassTemplateId).FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException($"Template {dto.ClassTemplateId} findes ikke.");

        var newClass = new Class
        {
            ClassName = template.ClassName,
            ClassDescription = template.ClassDescription,
            ClassType = template.ClassType,
            InstructorId = dto.InstructorId,
            CenterId = dto.CenterId,
            ClassroomId = dto.ClassroomId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            ClassCapacity = dto.ClassCapacity,
            Status = ClassStatus.Scheduled
        };

        await _classes.InsertOneAsync(newClass);
        return newClass;
    }

    public async Task UpdateAsync(string id, CreateClassDTO dto)
    {
        var existing = await GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Class {id} findes ikke.");

        var template = await _templates.Find(x => x.Id == dto.ClassTemplateId).FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException($"Template {dto.ClassTemplateId} findes ikke.");

        existing.ClassName = template.ClassName;
        existing.ClassDescription = template.ClassDescription;
        existing.ClassType = template.ClassType;
        existing.InstructorId = dto.InstructorId;
        existing.CenterId = dto.CenterId;
        existing.ClassroomId = dto.ClassroomId;
        existing.StartTime = dto.StartTime;
        existing.EndTime = dto.EndTime;
        existing.ClassCapacity = dto.ClassCapacity;

        await _classes.ReplaceOneAsync(x => x.Id == id, existing);
    }

    public async Task DeleteAsync(string id)
    {
        var result = await _classes.DeleteOneAsync(x => x.Id == id);
        if (result.DeletedCount == 0)
            throw new KeyNotFoundException($"Class {id} findes ikke.");
    }
}