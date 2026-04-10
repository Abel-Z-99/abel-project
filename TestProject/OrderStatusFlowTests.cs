using Application.Orders;
using Domain.Entities;

namespace TestProject;

/// <summary>
/// 订单状态流转规则（<see cref="OrderStatusFlow"/>）与订单实体默认值的单元测试。
/// </summary>
public class OrderStatusFlowTests
{
	/// <summary>
	/// 待支付 → 已支付 为合法业务流转。
	/// </summary>
	[Fact]
	public void CanTransit_allows_Pending_to_Paid()
	{
		Assert.True(OrderStatusFlow.CanTransit("Pending", "Paid"));
	}

	/// <summary>
	/// 已发货后不允许回退到待支付，避免状态乱序。
	/// </summary>
	[Fact]
	public void CanTransit_denies_Shipped_to_Pending()
	{
		Assert.False(OrderStatusFlow.CanTransit("Shipped", "Pending"));
	}

	/// <summary>
	/// 新建订单未显式赋值时，默认应为待支付（Pending）。
	/// </summary>
	[Fact]
	public void New_order_default_status_is_Pending()
	{
		var order = new Order();
		Assert.Equal("Pending", order.Status);
	}
}
