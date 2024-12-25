using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _context;

    public CartRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. Lấy giỏ hàng của người dùng theo UserId
    // 1. Lấy giỏ hàng của người dùng theo UserId
    public async Task<List<Cart>> GetCartByIdUserAsync(string idUser)
    {
        var carts = await _context.Carts
            .Include(c => c.Product)  // Bao gồm thông tin sản phẩm
            .Where(c => c.UserId == idUser)  // Lấy tất cả sản phẩm trong giỏ hàng của người dùng
            .ToListAsync();  // Trả về danh sách giỏ hàng của người dùng

        return carts;  // Trả về danh sách các sản phẩm trong giỏ hàng
    }


    // 2. Thêm sản phẩm vào giỏ hàng
    public async Task<Cart> AddProductToCartAsync(string idUser, int idProduct, int quantity)
    {
        var cart = await _context.Carts
            .Include(c => c.Product)  // Bao gồm thông tin sản phẩm
            .FirstOrDefaultAsync(c => c.UserId == idUser && c.ProductId == idProduct);

        // Nếu sản phẩm chưa có trong giỏ hàng của người dùng, tạo mới
        if (cart == null)
        {
            cart = new Cart
            {
                UserId = idUser.ToString(),
                ProductId = idProduct,
                Quantity = quantity
            };
            _context.Carts.Add(cart);
        }
        else
        {
            // Nếu sản phẩm đã có trong giỏ, chỉ cần tăng số lượng
            cart.Quantity += quantity;
        }

        await _context.SaveChangesAsync();  // Lưu thay đổi
        return cart;  // Trả về giỏ hàng đã cập nhật
    }

    // 3. Xóa sản phẩm khỏi giỏ hàng
    public async Task<Cart> RemoveProductFromCartAsync(string idUser, int idProduct)
    {
        var cart = await _context.Carts
            .FirstOrDefaultAsync(c => c.UserId == idUser && c.ProductId == idProduct);

        if (cart != null)
        {
            _context.Carts.Remove(cart);  // Xóa sản phẩm khỏi giỏ hàng
            await _context.SaveChangesAsync();
        }

        return cart;  // Trả về giỏ hàng đã xóa sản phẩm
    }

    // 4. Cập nhật số lượng sản phẩm trong giỏ hàng
    public async Task<Cart> UpdateProductQuantityAsync(string idUser, int idProduct, int newQuantity)
    {
        var cart = await _context.Carts
            .FirstOrDefaultAsync(c => c.UserId == idUser && c.ProductId == idProduct);

        if (cart != null)
        {
            cart.Quantity = newQuantity;  // Cập nhật số lượng
            await _context.SaveChangesAsync();
        }

        return cart;  // Trả về giỏ hàng sau khi cập nhật
    }

    // 5. Xóa toàn bộ giỏ hàng của người dùng
    public async Task ClearCartAsync(string idUser)
    {
        var cartItems = await _context.Carts
            .Where(c => c.UserId == idUser)
            .ToListAsync();  // Lấy tất cả các sản phẩm trong giỏ hàng

        _context.Carts.RemoveRange(cartItems);  // Xóa tất cả sản phẩm khỏi giỏ hàng
        await _context.SaveChangesAsync();
    }

    // 6. Tính tổng giá trị giỏ hàng
    public async Task<decimal> GetCartTotalAsync(string idUser)
    {
        var cartItems = await _context.Carts
            .Include(c => c.Product)  // Bao gồm thông tin sản phẩm
            .Where(c => c.UserId == idUser)
            .ToListAsync();  // Lấy tất cả sản phẩm trong giỏ hàng

        return cartItems.Sum(ci => ci.Quantity * ci.Product.Price);  // Tính tổng giá trị giỏ hàng
    }
}
