namespace api_bui_xuan_thang.Models;

public class Cart
{
    public string UserId { get; set; }
    public int ProductId { get; set; } 
    public int Quantity { get; set; } 
    
    public Product? Product { get; set; } 
    public User? User { get; set; }
}