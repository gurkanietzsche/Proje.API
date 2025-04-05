using System.Collections.Generic;

namespace Proje.API.DTOs
{
    public class CategoryDTO
    {
        public CategoryDTO()
        {
            ChildCategories = new List<CategoryDTO>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        // Parent-Child ilişkisi
        public int? ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }

        // Alt kategorileri listesi
        public List<CategoryDTO> ChildCategories { get; set; }
    }
}