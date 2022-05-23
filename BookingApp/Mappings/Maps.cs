using AutoMapper;
using BookingApp.Data;
using BookingApp.Models;
using BookingApp.Models.DataTrasnferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Mappings
{
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<Room, RoomDTO>().ReverseMap();
            CreateMap<RoomDTO, RoomVM>().ReverseMap();
            CreateMap<Booking, BookingDTO>().ReverseMap();
            CreateMap<BookingDTO, BookingVM>().ReverseMap();
            CreateMap<CreateBookingDTO, CreateBookingVM>().ReverseMap();
            CreateMap<ClientBookingRequestDTO, ClientBookingRequestVM>().ReverseMap();
            CreateMap<Client, ClientPasswordVM>().ReverseMap();
            CreateMap<Client, ClientVM>().ReverseMap();
            CreateMap<Client, ClientVMnoID>().ReverseMap();
        }
    }
}
