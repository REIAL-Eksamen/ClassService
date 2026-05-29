using ClassService.Models;
using ClassService.Repositories;
using MongoDB.Driver;

public class CenterRepository : ICenterRepository
{
    private readonly IMongoCollection<Center> _centers;

    public CenterRepository(IMongoDatabase db)
    {
        // Henter MongoDB collectionen, hvor alle centre og deres lokaler ligger.
        _centers = db.GetCollection<Center>("CenterCollection");
    }

    //// Finder ét center ud fra dets id.
    public Task<Center?> GetByIdAsync(string id) =>
        _centers.Find(x => x.Id == id).FirstOrDefaultAsync();

    // // Henter alle centre fra databasen.
    public Task<List<Center>> GetAllAsync() =>
        _centers.Find(_ => true).ToListAsync();
}
