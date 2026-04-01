using Application.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common;

namespace WebApi.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> GetSummary()
    {
        var summary = await _dashboardService.GetSummaryAsync();
        return Ok(ApiResponse<DashboardSummaryDto>.Ok(summary));
    }
}
