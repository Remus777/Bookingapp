using AutoMapper;
using BookingApp.Contracts;
using BookingApp.Data;
using BookingApp.Models.DataTrasnferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class RoomServices : IRoomServices
    {
        private readonly IRoomRepository _repo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IMapper _mapper;

        public RoomServices(
            IRoomRepository repo,
            IBookingRepository bookingRepo,
            IMapper mapper
            )
        {
            _repo = repo;
            _bookingRepo = bookingRepo;
            _mapper = mapper;
        }

        public int CreateRoom(RoomDTO room)
        {
            if (_repo.roomExists(room.RoomNumber))
            {
                return 1;
            }
            var roomMapping = _mapper.Map<Room>(room);
            var isSucces = _repo.Create(roomMapping);
            if (!isSucces)
            {
                return 2;
            }
            return 0;
        }

        public int DeleteRoom(int id)
        {
            var rooms = _repo.FindById(id);
            var roomBookings = _bookingRepo.GetBookingsByRoom(id);
            if (roomBookings.Any())
            {
                return 1;
            }
            if (rooms == null)
            {
                return 2;
            }
            var isSucces = _repo.Delete(rooms);
            if (!isSucces)
            {
                return 3;
            }
            return 0;
        }

        public RoomDTO DetailsRoom(int id)
        {
            if (!_repo.isExists(id))
            {
                return null;
            }
            var rooms = _repo.FindById(id);
            var model = _mapper.Map<RoomDTO>(rooms);
            return model;
        }

        public int EditRoom(RoomDTO model)
        {
            var room = _mapper.Map<Room>(model);
            if (_repo.roomExists(model.RoomNumber) && _repo.roomNbChanged(room))
            {
                return 1;
            }
            var isSucces = _repo.Update(room);
            if (!isSucces)
            {
                return 2;
            }
            return 0;
        }

        public List<RoomDTO> RoomsIndex()
        {
            var rooms = _repo.FindAll().ToList();
            var model = _mapper.Map<List<RoomDTO>>(rooms);
            return model;
        }
    }
}
