using Application.Dashboard;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _dbContext;

    public DashboardService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var totalProducts = await _dbContext.Products.CountAsync();
        var productsOnSale = await _dbContext.Products.CountAsync(x => x.IsOnSale);
        var lowStockProducts = await _dbContext.Products.CountAsync(x => x.Stock <= 10);

        var totalUsers = await _dbContext.Users.CountAsync();
        var activeUsers = await _dbContext.Users.CountAsync(x => x.Status);

        var totalOrders = await _dbContext.Orders.CountAsync();
        var todayOrders = await _dbContext.Orders.CountAsync(x => x.CreatedAt >= today && x.CreatedAt < tomorrow);
        var todaySales = await _dbContext.Orders
            .Where(x => x.CreatedAt >= today && x.CreatedAt < tomorrow)
            .SumAsync(x => (decimal?)x.TotalAmount) ?? 0m;

        var pendingOrders = await _dbContext.Orders.CountAsync(x => x.Status == "Pending");

        return new DashboardSummaryDto
        {
            TotalProducts = totalProducts,
            ProductsOnSale = productsOnSale,
            LowStockProducts = lowStockProducts,
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            TotalOrders = totalOrders,
            TodayOrders = todayOrders,
            TodaySales = todaySales,
            PendingOrders = pendingOrders
        };
    }
}
