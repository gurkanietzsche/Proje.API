namespace Proje.API.DTOs
{
    public class AddToCartDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemDTO
    {
        public int Quantity { get; set; }
    }
}