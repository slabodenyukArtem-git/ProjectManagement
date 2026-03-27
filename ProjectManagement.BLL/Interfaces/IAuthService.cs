using System.Threading.Tasks;
using ProjectManagement.BLL.DTOs;

namespace ProjectManagement.BLL.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<bool> AssignRoleAsync(int userId, string role);
}
