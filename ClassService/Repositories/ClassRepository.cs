using ClassService.Models;
using MongoDB.Driver;

namespace ClassService.Repositories;

public class ClassRepository : IClassRepository
{
    private readonly IMongoCollection<Class> _classes;

    public ClassRepository(IMongoDatabase db)
    {
        _classes = db.GetCollection<Class>("ClassCollection");
    }

    public Task<List<Class>> GetAllAsync() =>
        _classes.Find(_ => true).ToListAsync();

    public Task<Class?> GetByIdAsync(string id) =>
        _classes.Find(x => x.Id == id).FirstOrDefaultAsync();

    public Task<List<Class>> GetByCenterAsync(string centerId) =>
        _classes.Find(x => x.CenterId == centerId).ToListAsync();

    public Task InsertAsync(Class c) =>
        _classes.InsertOneAsync(c);

    public Task ReplaceAsync(string id, Class c) =>
        _classes.ReplaceOneAsync(x => x.Id == id, c);

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _classes.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }
}