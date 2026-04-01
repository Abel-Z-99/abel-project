using Application.Common;
using Application.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateOrderResponse>>> Create(CreateOrderRequest request)
    {
        var userIdText = User.FindFirst("sub")?.Value;
        if (!int.TryParse(userIdText, out var userId))
        {
            return Unauthorized(ApiResponse<CreateOrderResponse>.Fail("Invalid user token."));
        }

        try
        {
            var result = await _orderService.CreateAsync(userId, request);
            return Ok(ApiResponse<CreateOrderResponse>.Ok(result));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CreateOrderResponse>.Fail(ex.Message));
        }
    }

    [HttpGet("mine")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<OrderDto>>>> MyOrders()
    {
        var userIdText = User.FindFirst("sub")?.Value;
        if (!int.TryParse(userIdText, out var userId))
        {
            return Unauthorized(ApiResponse<IReadOnlyList<OrderDto>>.Fail("Invalid user token."));
        }

        var data = await _orderService.GetMineAsync(userId);
        return Ok(ApiResponse<IReadOnlyList<OrderDto>>.Ok(data));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PagedResult<AdminOrderDto>>>> AllOrders(
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _orderService.GetAllPagedAsync(keyword,page, pageSize);
        return Ok(ApiResponse<PagedResult<AdminOrderDto>>.Ok(result));
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateStatus(int id, UpdateOrderStatusRequest request)
    {
        try
        {
            var ok = await _orderService.UpdateStatusAsync(id, request.Status);
            if (!ok)
            {
                return NotFound(ApiResponse<string>.Fail("Order not found."));
            }

            return Ok(ApiResponse<string>.Ok("ok", "status updated"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }
}
