using AutoMapper;
using FYP_Backend.DTOs.Category;
using FYP_Backend.DTOs.MenuItem;
using FYP_Backend.DTOs.Order;
using FYP_Backend.DTOs.User;
using FYP_Backend.Models;

namespace FYP_Backend.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // MenuItem mappings
            CreateMap<MenuItem, MenuItemDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));

            CreateMap<CreateMenuItemDTO, MenuItem>();
            CreateMap<UpdateMenuItemDTO, MenuItem>();

            // Category mappings
            CreateMap<Category, CategoryDTO>();
            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<UpdateCategoryDTO, Category>();

            // OrderItem mappings
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.MenuItemName, opt =>
                    opt.MapFrom(src => src.MenuItem != null ? src.MenuItem.Name : string.Empty));
            CreateMap<CreateOrderItemDTO, OrderItem>();

            // Order mappings
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.OrderItems, opt =>
                    opt.MapFrom(src => src.OrderItems));
            CreateMap<CreateOrderDTO, Order>();

            // User Mappings
            CreateMap<User, UserDTO>();
            CreateMap<RegisterDTO, User>();


        }
    }
}
