using ClassService.DTOs;

namespace ClassService.Clients;

public interface IAdminClient
{
    Task<AdminCenterDTO?> GetAdminAsync(string adminId);
    Task<List<AdminCenterDTO>> GetInstructorsByCenterAsync(string centerId);  
}