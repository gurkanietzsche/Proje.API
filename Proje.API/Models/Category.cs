namespace Proje.API.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // İlişkiler
        public virtual ICollection<Product> Products { get; set; }
    }
}