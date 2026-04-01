using Application.Common;

namespace Application.Products;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetPagedAsync(bool? isOnSale, string? keyword, int page, int pageSize);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(ProductUpsertRequest request);
    Task<bool> UpdateAsync(int id, ProductUpsertRequest request);
    Task<bool> DeleteAsync(int id);
}

/// <summary>
/// 商品对外展示/传输模型。
/// </summary>
public class ProductDto
{
    /// <summary>商品主键。</summary>
    public int Id { get; set; }

    /// <summary>商品名称。</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>商品描述。</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>单价。</summary>
    public decimal Price { get; set; }

    /// <summary>库存数量。</summary>
    public int Stock { get; set; }

    /// <summary>主图 URL。</summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>是否上架销售。</summary>
    public bool IsOnSale { get; set; }

    /// <summary>创建时间（UTC）。</summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 创建或更新商品时的请求体。
/// </summary>
public class ProductUpsertRequest
{
    /// <summary>商品名称。</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>商品描述。</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>单价。</summary>
    public decimal Price { get; set; }

    /// <summary>库存数量。</summary>
    public int Stock { get; set; }

    /// <summary>主图 URL。</summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>是否上架销售；新建时默认为 true。</summary>
    public bool IsOnSale { get; set; } = true;
}
