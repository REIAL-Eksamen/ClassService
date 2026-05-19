using ClassService.Models;
using MongoDB.Driver;

namespace ClassService.Services;

public class ClassTemplateService
{
    private readonly IMongoCollection<ClassTemplate> _templates;

    public ClassTemplateService(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:Database"]);
        _templates = db.GetCollection<ClassTemplate>("class_templates");
    }

    public Task<List<ClassTemplate>> GetAllAsync() =>
        _templates.Find(_ => true).ToListAsync();

    public Task<ClassTemplate?> GetByIdAsync(string id) =>
        _templates.Find(x => x.Id == id).FirstOrDefaultAsync();

    public Task CreateAsync(ClassTemplate template) =>
        _templates.InsertOneAsync(template);

    public async Task UpdateAsync(string id, ClassTemplate updated)
    {
        var existing = await GetByIdAsync(id)
                       ?? throw new KeyNotFoundException($"Skabelon {id} findes ikke.");

        updated.Id = id;
        await _templates.ReplaceOneAsync(x => x.Id == id, updated);
    }

    public async Task DeleteAsync(string id)
    {
        var result = await _templates.DeleteOneAsync(x => x.Id == id);
        if (result.DeletedCount == 0)
            throw new KeyNotFoundException($"Skabelon {id} findes ikke.");
    }
}