﻿using AutoMapper;
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
        }
    }
}
