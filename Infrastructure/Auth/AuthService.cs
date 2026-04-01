using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Application.Auth;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(ApplicationDbContext dbContext, IJwtTokenGenerator jwtTokenGenerator)
        {
            _dbContext = dbContext;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email))
            {
                throw new InvalidOperationException("用户已存在。");
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Phone = request.Phone ?? string.Empty,
                AvatarUrl = request.AvatarUrl ?? string.Empty,
                PasswordHash = HashPassword(request.Password),
                Role = "User",
                Status = true,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Ensure default User role exists and link
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (role != null)
            {
                _dbContext.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                });

                await _dbContext.SaveChangesAsync();
            }

            return await CreateAuthResponseAsync(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == request.UserNameOrEmail ||
                    u.Email == request.UserNameOrEmail);

            if (user == null || user.PasswordHash != HashPassword(request.Password))
            {
                throw new InvalidOperationException("该用户不存在。");
            }

            return await CreateAuthResponseAsync(user);
        }

        private async Task<AuthResponse> CreateAuthResponseAsync(User user)
        {
            var roles = await _dbContext.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role.Name)
                .ToListAsync();

            var permissions = await _dbContext.RolePermissions
                .Include(rp => rp.Permission)
                .Include(rp => rp.Role)
                .Where(rp => roles.Contains(rp.Role.Name))
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToListAsync();

            var token = _jwtTokenGenerator.GenerateToken(
                user.Id,
                user.Username,
                user.Email,
                roles,
                permissions);

            return new AuthResponse
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = roles
            };
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
