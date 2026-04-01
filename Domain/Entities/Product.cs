using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    /// <summary>
    /// 商品实体。
    /// </summary>
    public class Product
    {
        /// <summary>主键标识。</summary>
        public int Id { get; set; }

        /// <summary>商品名称。</summary>
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>商品描述。</summary>
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>售价。</summary>
        public decimal Price { get; set; }

        /// <summary>库存数量。</summary>
        public int Stock { get; set; }

        /// <summary>商品主图地址。</summary>
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>是否上架销售。</summary>
        public bool IsOnSale { get; set; } = true;

        /// <summary>创建时间（UTC）。</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
