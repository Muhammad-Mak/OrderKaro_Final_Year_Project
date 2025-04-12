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

            // Map from MenuItem model to MenuItemDTO
            CreateMap<MenuItem, MenuItemDTO>()
                .ForMember(dest => dest.CategoryName, opt =>
                    opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));
            // Maps Category.Name to the CategoryName field in the DTO

            // Map from CreateMenuItemDTO and UpdateMenuItemDTO to MenuItem model
            CreateMap<CreateMenuItemDTO, MenuItem>();
            CreateMap<UpdateMenuItemDTO, MenuItem>();

            // ------------------ Category Mappings ------------------

            // Map between Category model and DTOs
            CreateMap<Category, CategoryDTO>();
            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<UpdateCategoryDTO, Category>();

            // ------------------ OrderItem Mappings ------------------

            // Map from OrderItem model to OrderItemDTO
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.MenuItemName, opt =>
                    opt.MapFrom(src => src.MenuItem != null ? src.MenuItem.Name : string.Empty));
            // Maps related MenuItem.Name to MenuItemName in the DTO

            // Map from CreateOrderItemDTO to OrderItem model
            CreateMap<CreateOrderItemDTO, OrderItem>();

            // ------------------ Order Mappings ------------------

            // Map from Order model to OrderDTO and include OrderItems mapping
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.OrderItems, opt =>
                    opt.MapFrom(src => src.OrderItems));

            // Map from CreateOrderDTO to Order model
            CreateMap<CreateOrderDTO, Order>();

            // ------------------ User Mappings ------------------

            // Map between User model and various DTOs
            CreateMap<User, UserDTO>();
            CreateMap<RegisterDTO, User>();
            CreateMap<UpdateUserDTO, User>();
        }
    }
}
