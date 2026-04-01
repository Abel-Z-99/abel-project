using Application.Auth;
using Application.Common;
using Application.Users;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users;

public class UserAdminService : IUserAdminService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IAuthService _authService;

    public UserAdminService(ApplicationDbContext dbContext, IAuthService authService)
    {
        _dbContext = dbContext;
        _authService = authService;
    }

    public async Task<PagedResult<AdminUserDto>> GetPagedAsync(string? keyword, int page, int pageSize)
    {
        var query = _dbContext.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.Username.Contains(keyword) || x.Email.Contains(keyword));
        }

        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);
        var total = await query.CountAsync();

        var users = await query
            .OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AdminUserDto
            {
                Id = x.Id,
                Username = x.Username,
                Email = x.Email,
                Phone = x.Phone,
                Role = x.Role,
                Status = x.Status,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<AdminUserDto>
        {
            Page = page,
            PageSize = pageSize,
            Total = total,
            Items = users
        };
    }

    public async Task<AdminUserDto> CreateAsync(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        var user = await _dbContext.Users.FirstAsync(x => x.Id == result.UserId);
        return new AdminUserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            Status = user.Status,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<bool> UpdateStatusAsync(int userId, bool status)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.Status = status;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
