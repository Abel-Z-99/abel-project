using Application.Common;
using Application.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductDto>>>> GetAll(
        [FromQuery] bool? isOnSale,
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false)
    {
        var result = await _productService.GetPagedAsync(isOnSale, keyword, page, pageSize, sortBy, sortDesc);
        return Ok(ApiResponse<PagedResult<ProductDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound(ApiResponse<ProductDto>.Fail("Product not found."));
        }

        return Ok(ApiResponse<ProductDto>.Ok(product));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create(ProductUpsertRequest request)
    {
        var dto = await _productService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<ProductDto>.Ok(dto, "created"));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<string>>> Update(int id, ProductUpsertRequest request)
    {
        var ok = await _productService.UpdateAsync(id, request);
        if (!ok)
        {
            return NotFound(ApiResponse<string>.Fail("Product not found."));
        }

        return Ok(ApiResponse<string>.Ok("ok", "updated"));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var ok = await _productService.DeleteAsync(id);
        if (!ok)
        {
            return NotFound(ApiResponse<string>.Fail("Product not found."));
        }

        return Ok(ApiResponse<string>.Ok("ok", "deleted"));
    }
}
