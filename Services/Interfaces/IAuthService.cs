using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectMVC.DTO;
using ProjectMVC.Models;

namespace ProjectMVC.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool success, string message)> LoginAsync(LoginDTO model);

        Task LogoutAsync();
        Task<User?> GetCurrentUserAsync();
        Task<(bool success, string message)> RegisterAsync(RegisterDTO model);
        
        Task<bool> IsInRoleAsync(User user,string roleCode);

        Task<IEnumerable<string>> GetUserRoleAsync(User user);


        

    }
}