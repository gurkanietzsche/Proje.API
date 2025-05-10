using System.ComponentModel.DataAnnotations;

namespace Proje.API.Models
{
    public class Product : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Stock { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        // İlişkiler
        public virtual Category Category { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<ProductReview> Reviews { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}