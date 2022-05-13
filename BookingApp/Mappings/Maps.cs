using AutoMapper;
using BookingApp.Data;
using BookingApp.Models;
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
            CreateMap<Room, RoomVM>().ReverseMap();
            CreateMap<Booking, BookingVM>().ReverseMap();
            CreateMap<Client, ClientPasswordVM>().ReverseMap();
            CreateMap<Client, ClientVM>().ReverseMap();
            CreateMap<Client, ClientVMnoID>().ReverseMap();
        }
    }
}
