using ClassService.Models;
using MongoDB.Driver;

namespace ClassService.Repositories;

public class ClassTemplateRepository : IClassTemplateRepository
{
    private readonly IMongoCollection<ClassTemplate> _templates;

    public ClassTemplateRepository(IMongoDatabase db)
    {
        // Henter MongoDB collectionen, hvor holdtemplates ligger.
        _templates = db.GetCollection<ClassTemplate>("ClassTemplateCollection");
    }

    // Henter alle holdtemplates fra databasen.
    public Task<List<ClassTemplate>> GetAllAsync() =>
        _templates.Find(_ => true).ToListAsync();

    // Finder én holdtemplate ud fra dens id.
    public Task<ClassTemplate?> GetByIdAsync(string id) =>
        _templates.Find(x => x.Id == id).FirstOrDefaultAsync();

    // Gemmer en ny holdtemplate i databasen.
    public Task InsertAsync(ClassTemplate template) =>
        _templates.InsertOneAsync(template);

    // Erstatter en eksisterende holdtemplate med nye oplysninger
    public async Task ReplaceAsync(string id, ClassTemplate template)
    {
        await _templates.ReplaceOneAsync(x => x.Id == id, template);
    }

    // Sletter en holdtemplate og returnerer true, hvis den blev fundet og slettet.
    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _templates.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }
}