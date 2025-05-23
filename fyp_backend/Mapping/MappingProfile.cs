using AutoMapper;
using FYP_Backend.DTOs.Category;
using FYP_Backend.DTOs.MenuItem;
using FYP_Backend.DTOs.Order;
using FYP_Backend.DTOs.User;
using FYP_Backend.Models;

namespace FYP_Backend.Mapping
{
    // AutoMapper Profile for defining how models map to DTOs and vice versa
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ------------------ MenuItem Mappings ------------------
            CreateMap<MenuItem, MenuItemDTO>()
                .ForMember(dest => dest.CategoryName, opt =>
                    opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));
            CreateMap<CreateMenuItemDTO, MenuItem>();
            CreateMap<UpdateMenuItemDTO, MenuItem>();

            // ------------------ Category Mappings ------------------
            CreateMap<Category, CategoryDTO>();
            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<UpdateCategoryDTO, Category>();

            // ------------------ OrderItem Mappings ------------------
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.MenuItemName, opt =>
                    opt.MapFrom(src => src.MenuItem.Name));
            CreateMap<CreateOrderItemDTO, OrderItem>();

            // ------------------ Order Mappings ------------------
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.OrderItems, opt =>
                    opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.FirstName, opt =>
                    opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt =>
                    opt.MapFrom(src => src.User.LastName));
            CreateMap<CreateOrderDTO, Order>();

            // ------------------ User Mappings ------------------
            CreateMap<User, UserDTO>();
            CreateMap<RegisterDTO, User>();
            CreateMap<UpdateUserDTO, User>();
        }
    }
}

