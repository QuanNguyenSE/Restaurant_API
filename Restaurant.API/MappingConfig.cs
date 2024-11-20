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

            CreateMap<RegisterRequestDTO, UserDTO>();
            CreateMap<ApplicationUser, UserDTO>();


            CreateMap<ShoppingCart, ShoppingCartDTO>().ReverseMap();
            CreateMap<CartItem, CartItemDTO>().ReverseMap();

            CreateMap<OrderHeader, OrderHeaderDTO>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailDTO>().ReverseMap();

            CreateMap<Booking, BookingDTO>().ReverseMap();
            CreateMap<Booking, BookingCreateDTO>().ReverseMap();
            CreateMap<BookingUpdateDTO, Booking>()
                .ForMember(dest => dest.BookingStatus, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
