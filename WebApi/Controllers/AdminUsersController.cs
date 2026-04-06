using Application.Auth;
using Application.Common;
using Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common;

namespace WebApi.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly IUserAdminService _userAdminService;

    public AdminUsersController(IUserAdminService userAdminService)
    {
        _userAdminService = userAdminService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AdminUserDto>>>> GetUsers(
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false)
    {
        var result = await _userAdminService.GetPagedAsync(keyword, page, pageSize, sortBy, sortDesc);
        return Ok(ApiResponse<PagedResult<AdminUserDto>>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AdminUserDto>>> CreateUser(RegisterRequest request)
    {
        try
        {
            var dto = await _userAdminService.CreateAsync(request);
            return Ok(ApiResponse<AdminUserDto>.Ok(dto, "user created"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<AdminUserDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateStatus(int id, UpdateUserStatusRequest request)
    {
        var ok = await _userAdminService.UpdateStatusAsync(id, request.Status);
        if (!ok)
        {
            return NotFound(ApiResponse<string>.Fail("User not found."));
        }

        return Ok(ApiResponse<string>.Ok("ok", "user status updated"));
    }
}
