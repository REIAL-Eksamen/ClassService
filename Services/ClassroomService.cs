using ClassService.Models;
using MongoDB.Driver;

namespace ClassService.Services;

public class ClassroomService
{
    private readonly IMongoCollection<Classroom> _classrooms;

    public ClassroomService(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:Database"]);
        _classrooms = db.GetCollection<Classroom>("classrooms");
    }

    public Task<List<Classroom>> GetAllAsync() =>
        _classrooms.Find(_ => true).ToListAsync();

    public Task<Classroom?> GetByIdAsync(string id) =>
        _classrooms.Find(x => x.ClassroomId == id).FirstOrDefaultAsync();

    public Task<List<Classroom>> GetByCenterAsync(string centerId) =>
        _classrooms.Find(x => x.CenterId == centerId).ToListAsync();

    public Task CreateAsync(Classroom classroom) =>
        _classrooms.InsertOneAsync(classroom);

    public async Task UpdateAsync(string id, Classroom updated)
    {
        var existing = await GetByIdAsync(id)
                       ?? throw new KeyNotFoundException($"Lokale {id} findes ikke.");

        updated.ClassroomId = id;
        await _classrooms.ReplaceOneAsync(x => x.ClassroomId == id, updated);
    }

    public async Task DeleteAsync(string id)
    {
        var result = await _classrooms.DeleteOneAsync(x => x.ClassroomId == id);
        if (result.DeletedCount == 0)
            throw new KeyNotFoundException($"Lokale {id} findes ikke.");
    }
}