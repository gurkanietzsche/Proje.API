namespace Proje.API.Models
{
    public class CartItem : BaseEntity
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // İlişkiler
        public virtual Cart Cart { get; set; }
        public virtual Product Product { get; set; }
    }
}