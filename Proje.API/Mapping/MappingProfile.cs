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
            // Category mappings
            CreateMap<Category, CategoryDTO>().ReverseMap();

            // Product mappings
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));
            CreateMap<ProductDTO, Product>();

            // User mappings
            CreateMap<IdentityUser, UserDTO>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Token, opt => opt.Ignore())  // Token manuel atanacak
                .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Roller manuel atanacak

            // Eğer varsa Order mappings
            // CreateMap<Order, OrderDTO>().ReverseMap();

            // Eğer varsa Cart mappings
            // CreateMap<Cart, CartDTO>().ReverseMap();
        }
    }
}