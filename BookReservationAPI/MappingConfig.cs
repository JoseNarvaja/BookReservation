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
            CreateMap<Reservation, ReservationDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.UserUsername, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.CopyBarcode, opt => opt.MapFrom(src => src.Copy.Barcode));
            CreateMap<Reservation, ReservationCreateDto>().ReverseMap();
            CreateMap<Copy, CopyDto>().ReverseMap();
            CreateMap<Copy,CopyCreateDto>().ReverseMap();
            CreateMap<Copy,CopyUpdateDto>().ReverseMap();
        }
    }
}
