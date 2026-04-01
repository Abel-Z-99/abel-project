namespace Application.Orders;

/// <summary>
/// 订单状态校验与流转规则（领域/应用层规则，不依赖 Web）。
/// </summary>
public static class OrderStatusFlow
{
    private static readonly HashSet<string> Statuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pending",
        "Paid",
        "Shipped",
        "Completed",
        "Cancelled"
    };

    public static bool IsValidStatus(string status)
    {
        return Statuses.Contains(status);
    }

    public static bool CanTransit(string from, string to)
    {
        from = from.Trim();
        to = to.Trim();

        if (from.Equals(to, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return (from, to) switch
        {
            ("Pending", "Paid") => true,
            ("Pending", "Cancelled") => true,
            ("Paid", "Shipped") => true,
            ("Paid", "Cancelled") => true,
            ("Shipped", "Completed") => true,
            _ => false
        };
    }
}
