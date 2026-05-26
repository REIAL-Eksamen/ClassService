using MongoDB.Driver;
using ClassService.Models;

namespace ClassService.Repositories;

public class ClassroomRepository : IClassroomRepository
{
    private readonly IMongoCollection<Classroom> _classrooms;

    public ClassroomRepository(IMongoDatabase db)
    {
        _classrooms = db.GetCollection<Classroom>("ClassroomCollection");
    }

    public Task<Classroom?> GetByIdAndCenterAsync(string classroomId, string centerId) =>
        _classrooms.Find(x => x.Id == classroomId && x.CenterId == centerId).FirstOrDefaultAsync();
}