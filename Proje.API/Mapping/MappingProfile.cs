using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Proje.API.DTOs;
using Proje.API.Models;

namespace Proje.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // MappingProfile içine ekleyin
            CreateMap<Category, CategoryDTO>()
                .ForMember(dest => dest.ParentCategoryName, opt =>
                    opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null))
                .ForMember(dest => dest.ChildCategories, opt =>
                    opt.MapFrom(src => src.ChildCategories));
            CreateMap<CategoryDTO, Category>();

            // Product mappings
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));
            CreateMap<ProductDTO, Product>();

            // User mappings
            CreateMap<IdentityUser, UserDTO>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.Ignore());
        }
    }
}