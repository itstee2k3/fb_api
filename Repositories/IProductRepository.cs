using api_bui_xuan_thang.Models;

namespace api_bui_xuan_thang.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductsAsync();
    Task<Product> GetProductByIdAsync(int id);
    Task AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
    Task<IEnumerable<Product>> SearchProductsByNameAsync(string name); // Thêm dòng này

}
