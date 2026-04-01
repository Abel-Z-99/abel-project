using Application.Auth;
using Application.Common;

namespace Application.Users;

public interface IUserAdminService
{
    Task<PagedResult<AdminUserDto>> GetPagedAsync(string? keyword, int page, int pageSize);

    /// <summary>
    /// 管理员创建用户（内部调用注册逻辑）。失败抛出 <see cref="InvalidOperationException"/>。
    /// </summary>
    Task<AdminUserDto> CreateAsync(RegisterRequest request);

    Task<bool> UpdateStatusAsync(int userId, bool status);
}

public class AdminUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateUserStatusRequest
{
    public bool Status { get; set; }
}
