using ClassService.Models;

namespace ClassService.Repositories;


public interface IClassroomRepository
{ 
    Task<Classroom?> GetByIdAndCenterAsync(string classroomId, string centerId);
}