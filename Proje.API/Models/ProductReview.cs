namespace Proje.API.Models
{
    public class ProductReview : BaseEntity
    {
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; } // 1-5 arası derecelendirme
        public bool IsApproved { get; set; } = false; // Admin onayı gerekebilir

        // İlişkiler
        public virtual Product Product { get; set; }
    }
}