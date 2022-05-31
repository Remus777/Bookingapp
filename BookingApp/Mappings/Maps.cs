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
            CreateMap<BookingRoomTypesDTO, BookingRoomTypesVM>().ReverseMap();
            CreateMap<ClientBookingRequestDTO, ClientBookingRequestVM>().ReverseMap();

            CreateMap<Client, ClientDTO>().ReverseMap();
            CreateMap<ClientDTO, ClientVM>().ReverseMap();
            CreateMap<ClientDetailsDTO, ClientDetailsVM>().ReverseMap();
            CreateMap<Client, ClientPasswordDTO>().ReverseMap();
            CreateMap<ClientPasswordDTO, ClientPasswordVM>().ReverseMap();
            CreateMap<ClientWithRoleDTO, ClientWithRoleVM>().ReverseMap();
            CreateMap<ClientDTOnoID, ClientVMnoID>().ReverseMap();
            CreateMap<ClientEditDTO, ClientEditVM>().ReverseMap();
        }
    }
}
