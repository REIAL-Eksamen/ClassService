using ClassService.DTOs;

namespace ClassService.Clients;

// Definerer hvilke kald ClassService kan lave til AdminService.
public interface IAdminClient
{
    Task<AdminCenterDTO?> GetAdminAsync(string adminId);
    Task<List<AdminCenterDTO>> GetInstructorsByCenterAsync(string centerId);  
}