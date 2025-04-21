namespace Proje.API.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; }

        // İlişkiler
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<ProductReview> Reviews { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}