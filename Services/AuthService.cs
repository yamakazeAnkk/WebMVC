using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectMVC.DTO;
using ProjectMVC.Models;
using ProjectMVC.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace ProjectMVC.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EmployeeContext _context;

        public AuthService(IHttpContextAccessor httpContextAccessor, EmployeeContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) { return null; }
            if (int.TryParse(userIdClaim.Value, out int userId))
            {
                return await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }
            return null;
        }

        public async Task<IEnumerable<string>> GetUserRoleAsync(User user)
        {
            if (user == null) return Enumerable.Empty<string>();
            
            return await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role.RoleCode)
                .ToListAsync();
        }

        public async Task<bool> IsInRoleAsync(User user, string roleCode)
        {
            if (user == null) return false;
            
            return await _context.UserRoles
                .AnyAsync(ur => ur.UserId == user.Id && ur.Role.RoleCode == roleCode);
        }

        public async Task<(bool success, string message)> LoginAsync(LoginDTO model)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null)
            {
                return (false, "Invalid username or password");
            }

            if (user.IsActive == false)
            {
                return (false, "Account is inactive");
            }

            var hashedPassword = HashPassword(model.Password);
            if (user.PasswordHash != hashedPassword)
            {
                return (false, "Invalid username or password");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleCode));
            }

            var claimsIdentity = new ClaimsIdentity(claims, "CustomAuth");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await _httpContextAccessor.HttpContext.SignInAsync(
                "CustomAuth",
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return (true, "Login successful");
        }

        public async Task LogoutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync("CustomAuth");
        }

        public async Task<(bool success, string message)> RegisterAsync(RegisterDTO model)
        {
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            {
                throw new Exception("Username already exists");
            }
            if (await _context.Users.AnyAsync(e => e.Email == model.Email))
            {
                throw new Exception("Email already exists");
            }

            // Validate department exists
            if (model.DepartmentId.HasValue)
            {
                var departmentExists = await _context.Departments.AnyAsync(d => d.Id == model.DepartmentId.Value);
                if (!departmentExists)
                {
                    throw new Exception("Selected department does not exist");
                }
            }

            var user = new User
            {
                Username = model.Username,
                PasswordHash = HashPassword(model.Password),
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                DepartmentId = model.DepartmentId
            };
             // Add default role (you can modify this based on your requirements)
            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleCode == "USER");
            if (defaultRole != null)
            {
                user.UserRoles.Add(new UserRole
                {
                    RoleId = defaultRole.Id,
                    AssignedAt = DateTime.UtcNow
                });
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return (true, "Register success");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}