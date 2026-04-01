using Application.Common;
using Application.Products;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Products;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _dbContext;

    public ProductService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ProductDto>> GetPagedAsync(bool? isOnSale, string? keyword, int page, int pageSize)
    {
        var query = _dbContext.Products.AsQueryable();
        if (isOnSale.HasValue)
        {
            query = query.Where(x => x.IsOnSale == isOnSale.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.Name.Contains(keyword) || x.Description.Contains(keyword));
        }

        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);
        var total = await query.CountAsync();

        var data = await query
            .OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                Stock = x.Stock,
                ImageUrl = x.ImageUrl,
                IsOnSale = x.IsOnSale,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<ProductDto>
        {
            Page = page,
            PageSize = pageSize,
            Total = total,
            Items = data
        };
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _dbContext.Products.FindAsync(id);
        if (product == null)
        {
            return null;
        }

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            IsOnSale = product.IsOnSale,
            CreatedAt = product.CreatedAt
        };
    }

    public async Task<ProductDto> CreateAsync(ProductUpsertRequest request)
    {
        var entity = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            ImageUrl = request.ImageUrl,
            IsOnSale = request.IsOnSale,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Products.Add(entity);
        await _dbContext.SaveChangesAsync();

        return new ProductDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            Stock = entity.Stock,
            ImageUrl = entity.ImageUrl,
            IsOnSale = entity.IsOnSale,
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task<bool> UpdateAsync(int id, ProductUpsertRequest request)
    {
        var product = await _dbContext.Products.FindAsync(id);
        if (product == null)
        {
            return false;
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.ImageUrl = request.ImageUrl;
        product.IsOnSale = request.IsOnSale;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _dbContext.Products.FindAsync(id);
        if (product == null)
        {
            return false;
        }

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
