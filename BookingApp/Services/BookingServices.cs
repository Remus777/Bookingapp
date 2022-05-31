using AutoMapper;
using BookingApp.Contracts;
using BookingApp.Data;
using BookingApp.Models;
using BookingApp.Models.DataTrasnferObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class BookingServices : IBookingServices
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Client> _userManager;
        public BookingServices(
            IRoomRepository roomRepo,
            IBookingRepository bookingRepo,
            IMapper mapper,
            UserManager<Client> userManager
        )
        {
            _bookingRepo = bookingRepo;
            _roomRepo = roomRepo;
            _mapper = mapper;
            _userManager = userManager;
        }

        public ClientBookingRequestDTO BookingIndex()
        {
            var bookings = _bookingRepo.FindAll();
            var BookingsIds = _bookingRepo.FindAllIds();
            var BookingClientsIds = _bookingRepo.FindAllClientIds();

            var clientRequestsModel = _mapper.Map<List<BookingDTO>>(bookings);

            var model = new ClientBookingRequestDTO
            {
                Bookings = clientRequestsModel
            };
            AddRoomToBookingDisplay(model, BookingsIds);
            AddClientToBookingDisplay(model, BookingClientsIds);


            return model;
        }
        public ClientBookingRequestDTO MyBookings(string clientID)
        {
            var clientBookings = _bookingRepo.GetBookingsByClient(clientID);
            var clientBookingsIds = clientBookings.Select(q => q.Id).ToList();

            var clientRequestsModel = _mapper.Map<List<BookingDTO>>(clientBookings);

            var model = new ClientBookingRequestDTO
            {
                Bookings = clientRequestsModel
            };
            AddRoomToBookingDisplay(model, clientBookingsIds);

            return model;
        }
        public BookingDTO Details(int id)
        {
            if (!_bookingRepo.isExists(id))
            {
                return null;
            }
            var booking = _bookingRepo.FindById(id);
            var model = _mapper.Map<BookingDTO>(booking);
            var bookedRooms = RoomsByBooking(id);
            model.Rooms = bookedRooms;
            model.NrOfRooms = bookedRooms.Count();
            return model;
        }
        public CreateBookingDTO CreateBookingGET()
        {
            var rooms = _roomRepo.GetRoomsToSelectList();
            
            var model = new CreateBookingDTO
            {
                Rooms = rooms,
                NrOfRoomTypes = rooms.Count(),
            };
            return model;
        }
        public string CreateBookingPOST(CreateBookingDTO model, string clientId)
        {
            var startDate = Convert.ToDateTime(model.Date_From);
            var endDate = Convert.ToDateTime(model.Date_To);

            if (DateTime.Compare(startDate, DateTime.Now) < 0)
            {
                return "The Start Date can not be in the past";
            }
            if (DateTime.Compare(startDate, endDate) > 0)
            {
                return "The Start Date cannot be further in the future than the End Date";
            }
            var rooms = _roomRepo.FindAll();
            var bookingModel = new BookingDTO
            {
                ClientId = clientId,
                Date_From = startDate,
                Date_To = endDate,
            };
            var booking = _mapper.Map<Booking>(bookingModel);
            booking.Rooms = new List<Room>();

            var bookingexists = _bookingRepo.GetUsedBookings(startDate, endDate).ToList();
            var nrOfUsedRoomTypes = CountUsedRoomTypes(model.RoomTypes);
            IgnoreCancelledBooking(bookingexists);
            int count = 0, countR = 0;
            for (int i = 0; i < nrOfUsedRoomTypes; i++)
            {
                var nrofRooms = model.RoomTypes[i].NrOfRooms;
                if(nrofRooms != null)
                {
                    count += 1;
                }
                if(model.RoomTypes[i].RoomType != null)
                {
                    countR += 1;
                }
                IgnoreBookedRooms(bookingexists, model.RoomTypes[i].RoomType, rooms);

                var flag = AddRoomToBooking(model.RoomTypes[i].RoomType, booking, rooms, nrofRooms);
                if (flag == 1)
                {
                    return $"There is no room available of type: {model.RoomTypes[i].RoomType}";
                }
            }
            if (countR == 0)
            {
                return "Please select a Room Type";
            }
            if (count == 0)
            {
                return "Please select a number of rooms";
            }    
            var isSucces = _bookingRepo.Create(booking);
            if (!isSucces)
            {
                return "Something went wrong with submiting your booking";
            }

            return null;
        }
        private int CountUsedRoomTypes (List<BookingRoomTypesDTO> RoomTypes)
        {
            int count = 0;
            for(int i = 0; i < RoomTypes.Count(); i++)
            {
                if(RoomTypes[i].RoomType != null)
                {
                    count += 1;
                }
            }
            return count;
        }
        private void IgnoreCancelledBooking(List<Booking> bookingexists)
        {
            var checkbooking = bookingexists
                .Where(q => q.Cancelled == true)
                .ToList();

            foreach (Booking cancelled in checkbooking)
            {
                bookingexists.Remove(bookingexists.FirstOrDefault());
            }

           
        }
        private List<RoomDTO> RoomsByBooking(int id)
        {
            var bookedRooms = _roomRepo.GetRoomsByBookings(id);
            var clientRoomsMapping = _mapper.Map<List<RoomDTO>>(bookedRooms);
            return clientRoomsMapping;
        }
        private void IgnoreBookedRooms(List<Booking> bookingexists, string roomType, ICollection<Room> rooms)
        {
            
            if (bookingexists.Any())
            {
                var bookingids = bookingexists.Select(q => q.Id).ToList();

                foreach (int bookingid in bookingids)
                {
                    var bookedRooms = _roomRepo.GetRoomsByBookings(bookingid);
                    foreach (Room bookedRoom in bookedRooms)
                    {
                        if (bookedRoom.RoomType == roomType)
                        {
                            rooms.Remove(rooms.Where(q => q.Id == bookedRoom.Id).FirstOrDefault());
                        }
                    }
                }

            }
        }

        private int AddRoomToBooking(string RoomType, Booking booking, ICollection<Room> rooms, int? nrofRooms)
        {
 
            for (int i = 0; i < nrofRooms; i++)
            {
                var room = rooms
                    .Where(q => q.RoomType == RoomType)
                    .FirstOrDefault();
                if (room == null)
                {
                    return 1;
                }
                booking.Rooms.Add(room);
                rooms.Remove(room);
            }
            return 0;
        }
        private void AddRoomToBookingDisplay(ClientBookingRequestDTO model, List<int> BookingsIds)
        {
            foreach (int bookingid in BookingsIds)
            {
                var bookedRooms = RoomsByBooking(bookingid);
                var bookingmodel = model.Bookings.Where(q => q.Id == bookingid).FirstOrDefault();
                bookingmodel.NrOfRooms = bookedRooms.Count();
                bookingmodel.Rooms = bookedRooms;

            }
        }
        private void AddClientToBookingDisplay(ClientBookingRequestDTO model, List<string> BookingClientsIds)
        {
            foreach (string clientid in BookingClientsIds)
            {
                var clients = _userManager.FindByIdAsync(clientid).Result;
                var clientsMapping = _mapper.Map<ClientDTO>(clients);
                var clientBookings = _bookingRepo.GetBookingsByClient(clientid);
                var clientBookingsIds = clientBookings.Select(q => q.Id).ToList();
                foreach (int bookingid in clientBookingsIds)
                {
                    var bkingmodel = model.Bookings.Where(q => q.ClientId == clientid && q.Id == bookingid).FirstOrDefault();
                    bkingmodel.Client = clientsMapping;
                }
            }
        }
        public int CancelRequest(int id)
        {
            var booking = _bookingRepo.FindById(id);
            if (booking == null)
            {
                return 1;
            }
            booking.Cancelled = true;
            var isSucces = _bookingRepo.Update(booking);
            if (!isSucces)
            {
                return 2;
            }
            return 0;
        }

        public int Delete(int id)
        {
            var bookings = _bookingRepo.FindById(id);
            if (bookings == null)
            {
               return 1;
            }
            var isSucces = _bookingRepo.Delete(bookings);
            if(!isSucces)
            {
                return 2;
            }
            return 0;
        }
    }
}
