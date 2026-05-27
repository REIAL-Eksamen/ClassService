using ClassService.Models;
using ClassService.Repositories;
using MongoDB.Driver;

public class CenterRepository : ICenterRepository
{
    private readonly IMongoCollection<Center> _centers;

    public CenterRepository(IMongoDatabase db)
    {
        _centers = db.GetCollection<Center>("CenterCollection");
    }

    public Task<Center?> GetByIdAsync(string id) =>
        _centers.Find(x => x.Id == id).FirstOrDefaultAsync();

    public Task<List<Center>> GetAllAsync() =>
        _centers.Find(_ => true).ToListAsync();
}
