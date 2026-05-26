using ClassService.Models;
using MongoDB.Driver;

namespace ClassService.Repositories;

public class ClassTemplateRepository : IClassTemplateRepository
{
    private readonly IMongoCollection<ClassTemplate> _templates;

    public ClassTemplateRepository(IMongoDatabase db)
    {
        _templates = db.GetCollection<ClassTemplate>("ClassTemplateCollection");
    }

    public Task<List<ClassTemplate>> GetAllAsync() =>
        _templates.Find(_ => true).ToListAsync();

    public Task<ClassTemplate?> GetByIdAsync(string id) =>
        _templates.Find(x => x.Id == id).FirstOrDefaultAsync();

    public Task InsertAsync(ClassTemplate template) =>
        _templates.InsertOneAsync(template);

    public async Task ReplaceAsync(string id, ClassTemplate template)
    {
        await _templates.ReplaceOneAsync(x => x.Id == id, template);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _templates.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }
}