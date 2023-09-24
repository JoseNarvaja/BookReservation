using AutoMapper;
using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using System.Runtime.InteropServices;

namespace BookReservationAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Book, BookDto>().ReverseMap();
            CreateMap<Book, BookCreateDto>().ReverseMap();
            CreateMap<Book, BookUpdateDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CategoryCreateDto>().ReverseMap();
            CreateMap<Category, CategoryUpdateDto>().ReverseMap();
            CreateMap<LocalUser, UserDto>().ReverseMap();
            CreateMap<Reservation, ReservationDto>().ReverseMap();
            CreateMap<Reservation, ReservationCreateDto>().ReverseMap();
        }
    }
}
