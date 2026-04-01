using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(RegisterRequest request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                return Ok(ApiResponse<AuthResponse>.Ok(result));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<AuthResponse>.Fail(ex.Message));
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(ApiResponse<AuthResponse>.Ok(result));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<AuthResponse>.Fail(ex.Message));
            }
        }

        [HttpGet("me")]
        [Authorize]
        public ActionResult<object> Me()
        {
            return Ok(new
            {
                UserId = User.FindFirst("sub")?.Value,
                Username = User.Identity?.Name,
                Email = User.FindFirst("email")?.Value,
                Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value)
            });
        }
    }
}
