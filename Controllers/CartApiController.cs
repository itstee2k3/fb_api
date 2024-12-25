using api.Models;
using api.Repositories;
using api.DTOs; // Thêm không gian tên chứa DTO
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartApiController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartApiController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        // 1. Lấy giỏ hàng của người dùng
        [HttpGet("user/{idUser}")]
        public async Task<ActionResult<List<Cart>>> GetCartByUserIdAsync(string idUser)
        {
            try
            {
                var carts = await _cartRepository.GetCartByIdUserAsync(idUser);
                if (carts == null || carts.Count == 0)
                {
                    return NotFound(new { message = "Giỏ hàng không tồn tại." });
                }
                return Ok(carts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy giỏ hàng.", details = ex.Message });
            }
        }

        [HttpPost("user/{idUser}/product/{idProduct}")]
        public async Task<ActionResult<Cart>> AddProductToCartAsync(string idUser, int idProduct, [FromBody] AddToCartRequest request)
        {
            try
            {
                if (request.Quantity <= 0)
                {
                    return BadRequest(new { message = "Số lượng sản phẩm phải lớn hơn 0." });
                }

                var cart = await _cartRepository.AddProductToCartAsync(idUser, idProduct, request.Quantity);
                return Ok(cart);  // Trả về giỏ hàng đã cập nhật
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi thêm sản phẩm vào giỏ hàng.", details = ex.InnerException?.Message ?? ex.Message });
            }
        }


        // 3. Xóa sản phẩm khỏi giỏ hàng
        [HttpDelete("user/{idUser}/product/{idProduct}")]
        public async Task<ActionResult> RemoveProductFromCartAsync(string idUser, int idProduct)
        {
            try
            {
                var cart = await _cartRepository.RemoveProductFromCartAsync(idUser, idProduct);
                if (cart == null)
                {
                    return NotFound(new { message = "Sản phẩm không tồn tại trong giỏ hàng." });
                }
                return NoContent();  // Trả về mã 204 khi thành công
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa sản phẩm khỏi giỏ hàng.", details = ex.Message });
            }
        }

        // 4. Cập nhật số lượng sản phẩm trong giỏ hàng
        [HttpPut("user/{idUser}/product/{idProduct}")]
        public async Task<ActionResult<Cart>> UpdateProductQuantityAsync(string idUser, int idProduct, [FromBody] AddToCartRequest request)
        {
            try
            {
                if (request.Quantity <= 0)
                {
                    return BadRequest(new { message = "Số lượng sản phẩm phải lớn hơn 0." });
                }

                var cart = await _cartRepository.UpdateProductQuantityAsync(idUser, idProduct, request.Quantity);
                if (cart == null)
                {
                    return NotFound(new { message = "Sản phẩm không tồn tại trong giỏ hàng." });
                }
                return Ok(cart);  // Trả về giỏ hàng đã cập nhật
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật số lượng sản phẩm.", details = ex.Message });
            }
        }

        // 5. Xóa toàn bộ giỏ hàng của người dùng
        [HttpDelete("user/{idUser}/clear")]
        public async Task<ActionResult> ClearCartAsync(string idUser)
        {
            try
            {
                await _cartRepository.ClearCartAsync(idUser);
                return NoContent();  // Trả về mã 204 khi thành công
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa toàn bộ giỏ hàng.", details = ex.Message });
            }
        }

        // 6. Tính tổng giá trị giỏ hàng của người dùng
        [HttpGet("user/{idUser}/total")]
        public async Task<ActionResult<decimal>> GetCartTotalAsync(string idUser)
        {
            try
            {
                var total = await _cartRepository.GetCartTotalAsync(idUser);
                return Ok(total);  // Trả về tổng giá trị giỏ hàng
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tính tổng giá trị giỏ hàng.", details = ex.Message });
            }
        }
    }
}
