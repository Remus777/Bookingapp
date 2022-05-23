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
        public CreateBookingDTO CreateBookingGET()
        {
            var model = new CreateBookingDTO
            {
                Rooms = _roomRepo.GetRoomsToSelectList()
            };
            return model;
        }
        public int CreateBookingPOST(CreateBookingDTO model, string clientId)
        {
            model.Rooms = _roomRepo.GetRoomsToSelectList();
            var startDate = Convert.ToDateTime(model.Date_From);
            var endDate = Convert.ToDateTime(model.Date_To);

            if (DateTime.Compare(startDate, DateTime.Now) < 0)
            {          
                return 1;
            }
            if (DateTime.Compare(startDate, endDate) > 0)
            {
                return 2;
            }

            var rooms = _roomRepo.FindAll();
        

            var bookingexists = _bookingRepo.GetUsedBookings(startDate, endDate).ToList();
            
            IgnoreCancelledBooking(bookingexists);
            IgnoreBookedRooms(bookingexists, model, rooms);

            var bookingModel = new BookingDTO
            {
                ClientId = clientId,
                Date_From = startDate,
                Date_To = endDate,
                RoomType = model.RoomType
            };
            var booking = _mapper.Map<Booking>(bookingModel);
            var flag = AddRoomToBooking(bookingModel, booking, rooms);
            if(flag == 1)
            {
                return 3;
            }
            var isSucces = _bookingRepo.Create(booking);
            if(!isSucces)
            {
                return 4;
            }    
            return 0;
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
        private void IgnoreBookedRooms(List<Booking> bookingexists, CreateBookingDTO model, ICollection<Room> rooms)
        {
            
            if (bookingexists.Any())
            {
                var bookingids = bookingexists.Select(q => q.Id).ToList();

                foreach (int bookingid in bookingids)
                {
                    var bookedRooms = _roomRepo.GetRoomsByBookings(bookingid);
                    foreach (Room broom in bookedRooms)
                    {
                        if (broom.RoomType == model.RoomType)
                        {
                            rooms.Remove(rooms.Where(q => q.Id == broom.Id).FirstOrDefault());
                        }
                    }
                }

            }
        }

        private int AddRoomToBooking(BookingDTO bookingModel, Booking booking, ICollection<Room> rooms)
        {

            booking.Rooms = new List<Room>();
            var room = rooms
                .Where(q => q.RoomType == bookingModel.RoomType)
                .FirstOrDefault();
            if (room == null)
            {
                return 1;
            }
            booking.Rooms.Add(room);
            return 0;
        }
        private void AddRoomToBookingDisplay(ClientBookingRequestDTO model, List<int> BookingsIds)
        {
            foreach (int bookingid in BookingsIds)
            {
                var bookedRooms = _roomRepo.GetRoomsByBookings(bookingid);
                var clientRoomsMapping = _mapper.Map<List<RoomDTO>>(bookedRooms);
                var bookingmodel = model.Bookings.Where(q => q.Id == bookingid).FirstOrDefault();
                bookingmodel.Rooms = clientRoomsMapping;

            }
        }
        private void AddClientToBookingDisplay(ClientBookingRequestDTO model, List<string> BookingClientsIds)
        {
            foreach (string clientid in BookingClientsIds)
            {
                var clients = _userManager.FindByIdAsync(clientid).Result;
                var clientsMapping = _mapper.Map<ClientVM>(clients);
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
