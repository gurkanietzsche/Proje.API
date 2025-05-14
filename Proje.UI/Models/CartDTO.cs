namespace Proje.UI.Models.DTOs
{
    public class CartDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<CartItemDTO> CartItems { get; set; }
        public decimal TotalAmount => CartItems?.Sum(item => item.Quantity * item.UnitPrice) ?? 0;
    }
}