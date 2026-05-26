using ClassService.DTOs;

namespace ClassService.Clients;

public interface IInstructorClient
{
    Task<InstructorDTO?> GetInstructorAsync(string instructorId);
}