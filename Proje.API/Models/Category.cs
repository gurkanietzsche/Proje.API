using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proje.API.Models
{
    public class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
            ChildCategories = new HashSet<Category>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        // Self-referencing relationship
        public int? ParentCategoryId { get; set; }

        [ForeignKey("ParentCategoryId")]
        public Category? ParentCategory { get; set; }

        // Navigation properties
        public ICollection<Category> ChildCategories { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}