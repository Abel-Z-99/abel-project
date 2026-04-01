namespace Application.Dashboard;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync();
}

/// <summary>
/// 管理后台仪表盘汇总指标。
/// </summary>
public class DashboardSummaryDto
{
    /// <summary>商品总数。</summary>
    public int TotalProducts { get; set; }

    /// <summary>在售商品数量。</summary>
    public int ProductsOnSale { get; set; }

    /// <summary>低库存商品数量（库存 ≤ 10）。</summary>
    public int LowStockProducts { get; set; }

    /// <summary>用户总数。</summary>
    public int TotalUsers { get; set; }

    /// <summary>启用中的用户数。</summary>
    public int ActiveUsers { get; set; }

    /// <summary>订单总数。</summary>
    public int TotalOrders { get; set; }

    /// <summary>今日新增订单数（UTC 自然日）。</summary>
    public int TodayOrders { get; set; }

    /// <summary>今日销售额合计（UTC 自然日）。</summary>
    public decimal TodaySales { get; set; }

    /// <summary>待处理订单数（状态为 Pending）。</summary>
    public int PendingOrders { get; set; }
}
