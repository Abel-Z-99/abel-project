using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    /// <summary>
    /// 订单实体。
    /// </summary>
    public class Order
    {
        /// <summary>主键标识。</summary>
        public int Id { get; set; }

        /// <summary>下单用户标识。</summary>
        public int UserId { get; set; }

        /// <summary>下单用户导航属性。</summary>
        public User User { get; set; } = null!;

        /// <summary>订单号。</summary>
        [MaxLength(50)]
        public string OrderNo { get; set; } = string.Empty;

        /// <summary>订单状态（如 Pending、Paid、Shipped、Completed、Cancelled）。</summary>
        [MaxLength(30)]
        public string Status { get; set; } = "Pending";

        /// <summary>订单总金额。</summary>
        public decimal TotalAmount { get; set; }

        /// <summary>收货地址。</summary>
        [MaxLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        /// <summary>收货人姓名。</summary>
        [MaxLength(50)]
        public string ContactName { get; set; } = string.Empty;

        /// <summary>收货人联系电话。</summary>
        [MaxLength(20)]
        public string ContactPhone { get; set; } = string.Empty;

        /// <summary>下单时间（UTC）。</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>订单明细集合。</summary>
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
