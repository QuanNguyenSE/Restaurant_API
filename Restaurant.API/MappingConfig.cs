using AutoMapper;
using Restaurant.API.Models;
using Restaurant.API.Models.DTO;

namespace Restaurant.API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<MenuItem, MenuItemDTO>().ReverseMap();
            CreateMap<MenuItem, MenuItemCreateDTO>().ReverseMap();
            CreateMap<MenuItem, MenuItemUpdateDTO>().ReverseMap();

            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Category, CategoryCreateDTO>().ReverseMap();
            CreateMap<Category, CategoryUpdateDTO>().ReverseMap();

            CreateMap<RegisterRequestDTO, UserDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();

            CreateMap<ShoppingCart, ShoppingCartDTO>().ReverseMap();
            CreateMap<CartItem, CartItemDTO>().ReverseMap();

            CreateMap<OrderHeader, OrderHeaderDTO>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailDTO>().ReverseMap();

        }
    }
}
