namespace Proje.API.DTOs
{
    public class ProductReviewDTO : BaseDTO
    {
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public bool IsApproved { get; set; }
    }

    public class CreateProductReviewDTO
    {
        public int ProductId { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
    }
}