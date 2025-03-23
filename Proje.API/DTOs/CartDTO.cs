namespace Proje.API.DTOs
{
    public class CartDTO : BaseDTO
    {
        public string UserId { get; set; }
        public List<CartItemDTO> CartItems { get; set; }
        public decimal TotalAmount => CartItems?.Sum(item => item.Quantity * item.UnitPrice) ?? 0;
    }
}