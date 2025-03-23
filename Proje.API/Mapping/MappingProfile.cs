using AutoMapper;
using Proje.API.DTOs;
using Proje.API.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Proje.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Kategori Eşleşmeleri
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();

            // Ürün Eşleşmeleri
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<ProductDTO, Product>();

            // Sipariş Eşleşmeleri
            CreateMap<Order, OrderDTO>();
            CreateMap<OrderDTO, Order>();

            // Sipariş Öğesi Eşleşmeleri
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
            CreateMap<OrderItemDTO, OrderItem>();

            // Sepet Eşleşmeleri
            CreateMap<Cart, CartDTO>();
            CreateMap<CartDTO, Cart>();

            // Sepet Öğesi Eşleşmeleri
            CreateMap<CartItem, CartItemDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product.Price));
            CreateMap<CartItemDTO, CartItem>();
        }
    }
}