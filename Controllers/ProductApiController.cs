using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;
[Route("api/[controller]")]
[ApiController]

public class ProductApiController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    public ProductApiController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        try
        {
            var products = await _productRepository.GetProductsAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            // Handle exception
            return StatusCode(500, "Internal server error");
        }
    }
    [
    HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        try
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }
        catch (Exception ex)
        {
            // Handle exception
            return StatusCode(500, "Internal server error");
        }
    }
    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] Product product)
    {
        try
        {
            await _productRepository.AddProductAsync(product);
            return CreatedAtAction(nameof(GetProductById), new
            {
                id = product.Id
            }, product);
        }
        catch (Exception ex)
        {
            // Handle exception
            return StatusCode(500, "Internal server error");
        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
    {
        try
        {
            if (id != product.Id)
                return BadRequest();
            await _productRepository.UpdateProductAsync(product);
            return NoContent();
        }
        catch (Exception ex)
        {
            // Handle exception
            return StatusCode(500, "Internal server error");
        }
    }
    [
    HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            await _productRepository.DeleteProductAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            // Handle exception
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string? name)
    {
        try
        {
            IEnumerable<Product> products;  // Sử dụng IEnumerable để lưu kết quả

            // Nếu không có từ khóa tìm kiếm, trả về tất cả sản phẩm
            if (string.IsNullOrEmpty(name))
            {
                products = await _productRepository.GetProductsAsync();  // Lấy tất cả sản phẩm
            }
            else
            {
                // Nếu có từ khóa tìm kiếm, lọc theo tên
                products = await _productRepository.SearchProductsByNameAsync(name);
            }

            // Chuyển đổi từ IEnumerable sang List nếu cần thiết
            var productList = products.ToList();  // Chuyển từ IEnumerable sang List

            // Kiểm tra nếu không có sản phẩm nào
            if (!productList.Any())
                return NotFound("No products found matching the search criteria.");
        
            return Ok(productList);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
