using Application.Dashboard;
using Infrastructure.Data;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Dashboard;

public class DashboardService : IDashboardService
{
    private const string CacheKeyPrefix = "dashboard:summary";
    private readonly ApplicationDbContext _dbContext;
    private readonly IDistributedCache _cache;
    private readonly int _cacheTtlSeconds;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        ApplicationDbContext dbContext,
        IDistributedCache cache,
        IConfiguration configuration,
        ILogger<DashboardService> logger)
    {
        _dbContext = dbContext;
        _cache = cache;
        _cacheTtlSeconds = configuration.GetValue<int?>("Redis:DashboardSummaryTtlSeconds") ?? 30;
        _logger = logger;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        var cacheKey = $"{CacheKeyPrefix}:{today:yyyyMMdd}";

        try
        {
            var cacheJson = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrWhiteSpace(cacheJson))
            {
                var cached = JsonSerializer.Deserialize<DashboardSummaryDto>(cacheJson);
                if (cached != null)
                {
                    return cached;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Read dashboard summary cache failed, fallback to database query.");
        }

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

        var result = new DashboardSummaryDto
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

        try
        {
            var payload = JsonSerializer.Serialize(result);
            await _cache.SetStringAsync(
                cacheKey,
                payload,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(Math.Max(5, _cacheTtlSeconds))
                });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Write dashboard summary cache failed.");
        }

        return result;
    }
}
