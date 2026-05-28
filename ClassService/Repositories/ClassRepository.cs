using ClassService.Models;
using MongoDB.Driver;

namespace ClassService.Repositories;

public class ClassRepository : IClassRepository
{
    private readonly IMongoCollection<Class> _classes;

    public ClassRepository(IMongoDatabase db)
    {
        // Henter MongoDB collectionen, hvor de konkrete hold ligger.
        _classes = db.GetCollection<Class>("ClassCollection");
    }

    // Henter alle hold fra databasen.
    public Task<List<Class>> GetAllAsync() =>
        _classes.Find(_ => true).ToListAsync();

    // Finder ét hold ud fra dets id.
    public Task<Class?> GetByIdAsync(string id) =>
        _classes.Find(x => x.Id == id).FirstOrDefaultAsync();

    // Henter alle hold, der tilhører et bestemt center.
    public Task<List<Class>> GetByCenterAsync(string centerId) =>
        _classes.Find(x => x.CenterId == centerId).ToListAsync();

    // Gemmer et nyt hold i databasen.
    public Task InsertAsync(Class c) =>
        _classes.InsertOneAsync(c);

    // Erstatter et eksisterende hold med en opdateret version
    public Task ReplaceAsync(string id, Class c) =>
        _classes.ReplaceOneAsync(x => x.Id == id, c);

    // Sletter et hold og returnerer true, hvis der faktisk blev slettet noget.
    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _classes.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }
    
    // Opdaterer et hold ved at erstatte dokumentet i databasen.
    public Task UpdateAsync(string id, Class c) =>
        _classes.ReplaceOneAsync(x => x.Id == id, c);
}