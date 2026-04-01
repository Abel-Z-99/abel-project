namespace Domain.Entities
{
    /// <summary>
    /// 订单明细（订单中的单行商品项）。
    /// </summary>
    public class OrderItem
    {
        /// <summary>主键标识。</summary>
        public int Id { get; set; }

        /// <summary>所属订单标识。</summary>
        public int OrderId { get; set; }

        /// <summary>所属订单导航属性。</summary>
        public Order Order { get; set; } = null!;

        /// <summary>商品标识。</summary>
        public int ProductId { get; set; }

        /// <summary>商品导航属性。</summary>
        public Product Product { get; set; } = null!;

        /// <summary>购买数量。</summary>
        public int Quantity { get; set; }

        /// <summary>下单时的单价（快照）。</summary>
        public decimal UnitPrice { get; set; }

        /// <summary>该行小计金额（单价 × 数量）。</summary>
        public decimal TotalPrice { get; set; }
    }
}
