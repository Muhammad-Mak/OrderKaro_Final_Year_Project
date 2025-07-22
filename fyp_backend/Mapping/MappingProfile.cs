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

            // Map MenuItem → MenuItemDTO, including nested Category name
            CreateMap<MenuItem, MenuItemDTO>()
                .ForMember(dest => dest.CategoryName, opt =>
                    opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));

            // Map CreateMenuItemDTO → MenuItem (used for item creation)
            CreateMap<CreateMenuItemDTO, MenuItem>();

            // Map UpdateMenuItemDTO → MenuItem (used for item updates)
            CreateMap<UpdateMenuItemDTO, MenuItem>();

            // ------------------ Category Mappings ------------------

            // Map Category → CategoryDTO
            CreateMap<Category, CategoryDTO>();

            // Map CreateCategoryDTO → Category (for adding new categories)
            CreateMap<CreateCategoryDTO, Category>();

            // Map UpdateCategoryDTO → Category (for editing existing categories)
            CreateMap<UpdateCategoryDTO, Category>();

            // ------------------ OrderItem Mappings ------------------

            // Map OrderItem → OrderItemDTO and include related MenuItem's name
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.MenuItemName, opt =>
                    opt.MapFrom(src => src.MenuItem.Name));

            // Map CreateOrderItemDTO → OrderItem
            CreateMap<CreateOrderItemDTO, OrderItem>();

            // ------------------ Order Mappings ------------------

            // Map Order → OrderDTO and include nested order items and user names
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.OrderItems, opt =>
                    opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.FirstName, opt =>
                    opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt =>
                    opt.MapFrom(src => src.User.LastName));

            // Map CreateOrderDTO → Order
            CreateMap<CreateOrderDTO, Order>();

            // ------------------ User Mappings ------------------

            // Map User → UserDTO
            CreateMap<User, UserDTO>();

            // Map RegisterDTO → User (used during registration)
            CreateMap<RegisterDTO, User>();

            // Map UpdateUserDTO → User (used when admin updates user profile)
            CreateMap<UpdateUserDTO, User>();
        }
    }
}
