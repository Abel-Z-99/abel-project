using Application.Common;

namespace Application.Orders;

public interface IOrderService
{
    /// <summary>
    /// 创建订单；业务失败时抛出 <see cref="InvalidOperationException"/>。
    /// </summary>
    Task<CreateOrderResponse> CreateAsync(int userId, CreateOrderRequest request);

    /// <summary>
    /// 查询当前用户的订单列表。
    /// </summary>
    Task<IReadOnlyList<OrderDto>> GetMineAsync(int userId);

    /// <summary>
    /// 管理员分页查询全部订单。
    /// </summary>
    Task<PagedResult<AdminOrderDto>> GetAllPagedAsync(string? keyword,int page, int pageSize);

    /// <summary>
    /// 更新订单状态；订单不存在返回 false；状态非法抛出 <see cref="InvalidOperationException"/>。
    /// </summary>
    Task<bool> UpdateStatusAsync(int orderId, string newStatus);
}

/// <summary>
/// 用户下单请求体（收货信息与明细行）。
/// </summary>
public class CreateOrderRequest
{
    /// <summary>收货地址。</summary>
    public string ShippingAddress { get; set; } = string.Empty;

    /// <summary>联系人姓名。</summary>
    public string ContactName { get; set; } = string.Empty;

    /// <summary>联系电话。</summary>
    public string ContactPhone { get; set; } = string.Empty;

    /// <summary>订单商品行集合。</summary>
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

/// <summary>
/// 下单时单行商品：商品标识与购买数量。
/// </summary>
public class CreateOrderItemRequest
{
    /// <summary>商品主键。</summary>
    public int ProductId { get; set; }

    /// <summary>购买数量。</summary>
    public int Quantity { get; set; }
}

/// <summary>
/// 创建订单成功后的简要返回信息。
/// </summary>
public class CreateOrderResponse
{
    /// <summary>订单主键。</summary>
    public int Id { get; set; }

    /// <summary>订单号。</summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>订单总金额。</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>当前订单状态（如 Pending）。</summary>
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// 订单明细行（对外展示）。
/// </summary>
public class OrderItemDto
{
    /// <summary>商品主键。</summary>
    public int ProductId { get; set; }

    /// <summary>商品名称（下单时快照）。</summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>数量。</summary>
    public int Quantity { get; set; }

    /// <summary>成交单价。</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>该行小计（单价 × 数量）。</summary>
    public decimal TotalPrice { get; set; }
}

/// <summary>
/// 订单概要（用户侧列表/详情）。
/// </summary>
public class OrderDto
{
    /// <summary>订单主键。</summary>
    public int Id { get; set; }

    /// <summary>订单号。</summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>订单状态。</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>订单总金额。</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>下单时间（UTC）。</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>下单时间（格式化）。</summary>
    public string CreatedAtFormatted { get; set; } = string.Empty;

    /// <summary>订单明细行。</summary>
    public IEnumerable<OrderItemDto> Items { get; set; } = Array.Empty<OrderItemDto>();
}

/// <summary>
/// 订单关联的客户简要信息（管理端用）。
/// </summary>
public class OrderCustomerDto
{
    /// <summary>用户主键。</summary>
    public int UserId { get; set; }

    /// <summary>用户名。</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>邮箱。</summary>
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// 管理端订单视图：在 <see cref="OrderDto"/> 基础上附带客户信息。
/// </summary>
public class AdminOrderDto : OrderDto
{
    /// <summary>下单用户信息。</summary>
    public OrderCustomerDto Customer { get; set; } = new();
}

/// <summary>
/// 管理员更新订单状态的请求体。
/// </summary>
public class UpdateOrderStatusRequest
{
    /// <summary>目标状态（如 Paid、Shipped）；默认 Pending。</summary>
    public string Status { get; set; } = "Pending";
}
