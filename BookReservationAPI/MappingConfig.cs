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
        }
    }
}
