using Application.Common;
using Application.Orders;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Orders;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _dbContext;

    public OrderService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateOrderResponse> CreateAsync(int userId, CreateOrderRequest request)
    {
        if (request.Items.Count == 0)
        {
            throw new InvalidOperationException("Order items cannot be empty.");
        }

        var productIds = request.Items.Select(x => x.ProductId).Distinct().ToList();
        var products = await _dbContext.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        if (products.Count != productIds.Count)
        {
            throw new InvalidOperationException("One or more products do not exist.");
        }

        foreach (var item in request.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            if (product.Stock < item.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product: {product.Name}");
            }
        }

        var order = new Order
        {
            UserId = userId,
            OrderNo = $"WS{DateTime.UtcNow:yyyyMMddHHmmssfff}",
            Status = "Pending",
            ShippingAddress = request.ShippingAddress,
            ContactName = request.ContactName,
            ContactPhone = request.ContactPhone,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in request.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            var unitPrice = product.Price;
            var totalPrice = unitPrice * item.Quantity;
            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = unitPrice,
                TotalPrice = totalPrice
            });
            product.Stock -= item.Quantity;
        }

        order.TotalAmount = order.Items.Sum(x => x.TotalPrice);

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        return new CreateOrderResponse
        {
            Id = order.Id,
            OrderNo = order.OrderNo,
            TotalAmount = order.TotalAmount,
            Status = order.Status
        };
    }

    public async Task<IReadOnlyList<OrderDto>> GetMineAsync(int userId)
    {
        var data = await _dbContext.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.Id)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNo = o.OrderNo,
                Status = GetStatusText(o.Status),
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt,
                CreatedAtFormatted = o.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList()
            })
            .ToListAsync();

        return data;
    }

    public async Task<PagedResult<AdminOrderDto>> GetAllPagedAsync(string? keyword, int page, int pageSize, string? sortBy, bool sortDesc)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);
        var query = _dbContext.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.OrderNo.Contains(keyword) || x.User.Username.Contains(keyword) || x.Status.Contains(keyword) );
        }

        query = ApplyAdminSort(query, sortBy, sortDesc);

        var total = await query.CountAsync();

        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new AdminOrderDto
            {
                Id = o.Id,
                OrderNo = o.OrderNo,
                Status = GetStatusText(o.Status),
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt,
                CreatedAtFormatted = o.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                Customer = new OrderCustomerDto { UserId = o.UserId, Username = o.User.Username, Email = o.User.Email },
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList()
            })
            .ToListAsync();

        return new PagedResult<AdminOrderDto>
        {
            Page = page,
            PageSize = pageSize,
            Total = total,
            Items = data
        };
    }

    public async Task<bool> UpdateStatusAsync(int orderId, string newStatus)
    {
        var order = await _dbContext.Orders.FindAsync(orderId);
        if (order == null)
        {
            return false;
        }

        if (!OrderStatusFlow.IsValidStatus(newStatus))
        {
            throw new InvalidOperationException("Invalid target status.");
        }

        if (!OrderStatusFlow.CanTransit(order.Status, newStatus))
        {
            throw new InvalidOperationException($"Cannot change order status from {order.Status} to {newStatus}.");
        }

        order.Status = newStatus;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private static string GetStatusText(string status)
    {
        return status switch
        {
            "Pending" => "待支付",
            "Paid" => "已支付",
            "Shipped" => "已发货",
            "Completed" => "已完成",
            "Cancelled" => "已取消",
            _ => status
        };
    }

    private static IQueryable<Order> ApplyAdminSort(IQueryable<Order> query, string? sortBy, bool sortDesc)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return query.OrderByDescending(o => o.Id);
        }

        var key = sortBy.Trim().ToLowerInvariant();
        return (key, sortDesc) switch
        {
            ("id", true) => query.OrderByDescending(o => o.Id),
            ("id", false) => query.OrderBy(o => o.Id),
            ("orderno", true) => query.OrderByDescending(o => o.OrderNo),
            ("orderno", false) => query.OrderBy(o => o.OrderNo),
            ("status", true) => query.OrderByDescending(o => o.Status),
            ("status", false) => query.OrderBy(o => o.Status),
            ("totalamount", true) => query.OrderByDescending(o => o.TotalAmount),
            ("totalamount", false) => query.OrderBy(o => o.TotalAmount),
            ("createdat", true) => query.OrderByDescending(o => o.CreatedAt),
            ("createdat", false) => query.OrderBy(o => o.CreatedAt),
            ("userid", true) => query.OrderByDescending(o => o.UserId),
            ("userid", false) => query.OrderBy(o => o.UserId),
            ("username", true) => query.OrderByDescending(o => o.User.Username),
            ("username", false) => query.OrderBy(o => o.User.Username),
            _ => query.OrderByDescending(o => o.Id),
        };
    }
}
