namespace Proje.API.Models
{
    public class Cart : BaseEntity
    {
        public string UserId { get; set; }
        public DateTime ModifiedDate { get; set; } = DateTime.Now; // Bu satırı ekleyin

        // İlişkiler
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}