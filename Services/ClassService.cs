using ClassService.DTOs;
using ClassService.Models;
using MongoDB.Driver;

namespace ClassService.Services;

public class ClassService
{
    private readonly IMongoCollection<Class> _classes;
    private readonly IMongoCollection<ClassTemplate> _templates;
    private readonly IMongoCollection<Classroom> _classrooms;

    public ClassService(IConfiguration config)
    {
        var client = new MongoClient(config["CosmosDB:AccountKey"]);
        var db = client.GetDatabase(config["MongoDB:Database"]);
        _classes = db.GetCollection<Class>("ClassCollection");
        _templates = db.GetCollection<ClassTemplate>("ClassTemplateCollection");
        _classrooms = db.GetCollection<Classroom>("ClassroomCollection");
    }

    public Task<List<Class>> GetAllAsync() =>
        _classes.Find(_ => true).ToListAsync();

    public Task<Class?> GetByIdAsync(string id) =>
        _classes.Find(x => x.Id == id).FirstOrDefaultAsync();

    public Task<List<Class>> GetByCenterAsync(string centerId) =>
        _classes.Find(x => x.CenterId == centerId).ToListAsync();

    public async Task<Class> CreateFromTemplateAsync(CreateClassDTO dto)
    {
        if (dto.Classroom is null)
            throw new ArgumentException("Classroom må ikke være null.");
        
        var template = await _templates.Find(x => x.Id == dto.ClassTemplateId).FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException($"Template {dto.ClassTemplateId} findes ikke.");
        
        var classroom = await _classrooms.Find(x => x.Id == dto.Classroom.ClassroomId && x.CenterId == dto.CenterId).FirstOrDefaultAsync();
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

        await _classes.InsertOneAsync(newClass);
        return newClass;
    }

    public async Task UpdateAsync(string id, CreateClassDTO dto)
    {
        if (dto.Classroom is null)
            throw new ArgumentException("Classroom må ikke være null.");
        
        var existing = await GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Class {id} findes ikke.");

        var template = await _templates.Find(x => x.Id == dto.ClassTemplateId).FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException($"Template {dto.ClassTemplateId} findes ikke.");
        
        var classroom = await _classrooms.Find(x => x.Id == dto.Classroom.ClassroomId && x.CenterId == dto.CenterId).FirstOrDefaultAsync();
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

        await _classes.ReplaceOneAsync(x => x.Id == id, existing);
    }

    public async Task DeleteAsync(string id)
    {
        var result = await _classes.DeleteOneAsync(x => x.Id == id);
        if (result.DeletedCount == 0)
            throw new KeyNotFoundException($"Class {id} findes ikke.");
    }
}