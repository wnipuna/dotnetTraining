using EmployeeService.DTOs;
using EmployeeService.Models;

namespace EmployeeService.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> UserExistsAsync(string username);
        string GenerateJwtToken(User user);
    }
}
