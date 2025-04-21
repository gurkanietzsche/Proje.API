using System.Collections.Generic;
using System.ComponentModel;

namespace Proje.API.DTOs
{
    public class CategoryDTO
    {
        [DefaultValue(0)]
        public int Id { get; set; }

        [DefaultValue("Elektronik")]
        public string Name { get; set; }

        [DefaultValue("Elektronik Ürünler")]
        public string Description { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [DefaultValue(null)]
        public int? ParentCategoryId { get; set; }

        [DefaultValue(null)]
        public string ParentCategoryName { get; set; }

        [DefaultValue(new object[0])]  // Boş array için
        public List<CategoryDTO> ChildCategories { get; set; } = new List<CategoryDTO>();
    }
}
