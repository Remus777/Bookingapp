using AutoMapper;
using BookingApp.Contracts;
using BookingApp.Data;
using BookingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Client> _userManager;
        public BookingController(
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
        // GET: BookingController
        public ActionResult Index()
        {
            var bookings = _bookingRepo.FindAll().ToList();
            var BookingsIds = bookings.Select(q => q.Id).ToList();
            var BookingClientsIds = bookings.Select(q => q.ClientId).ToList();

            var clientRequestsModel = _mapper.Map<List<BookingVM>>(bookings);

            var model = new ClientBookingRequestVM
            {
                Bookings = clientRequestsModel
            };
            foreach (int bookingid in BookingsIds)
            {
                var bookedRooms = _roomRepo.GetRoomsByBookings(bookingid);
                var clientRoomsMapping = _mapper.Map<List<RoomVM>>(bookedRooms);
                var bookingmodel = model.Bookings.Where(q => q.Id == bookingid).FirstOrDefault();
                bookingmodel.Rooms = clientRoomsMapping;
                
            }
            foreach (string clientid in BookingClientsIds)
            {
                var clients = _userManager.FindByIdAsync(clientid).Result;
                var clientsMapping = _mapper.Map<ClientVM>(clients);
                var clientId = clients.Id;
                var clientBookings = _bookingRepo.GetBookingsByClient(clientId);
                var clientBookingsIds = clientBookings.Select(q => q.Id).ToList();
                foreach (int bookingid in clientBookingsIds)
                {
                    var bkingmodel = model.Bookings.Where(q => q.ClientId == clientid && q.Id == bookingid).FirstOrDefault();
                    bkingmodel.Client = clientsMapping;
                }
            }
            return View(model);
        }

        public ActionResult MyBookings()
        {
            var client = _userManager.GetUserAsync(User).Result;
            var clientId = client.Id;
            var clientBookings = _bookingRepo.GetBookingsByClient(clientId);
            var clientBookingsIds = clientBookings.Select(q => q.Id ).ToList();

            var clientRequestsModel = _mapper.Map<List<BookingVM>>(clientBookings);

            var model = new ClientBookingRequestVM
            {
                Bookings = clientRequestsModel
            };
            foreach (int bookingid in clientBookingsIds)
            {
                var bookedRooms = _roomRepo.GetRoomsByBookings(bookingid);
                var clientRoomsModel = _mapper.Map<List<RoomVM>>(bookedRooms);
                var bookingmodel = model.Bookings.Where(q => q.Id == bookingid).FirstOrDefault();
                bookingmodel.Rooms = clientRoomsModel;

            }

           
            return View(model);
        }

     

        // GET: BookingController/Create
        public ActionResult Create()
        {
            var rooms = _roomRepo.FindAll();

            var roomItems = rooms.Select(q => new SelectListItem
            {
                Text = q.RoomType,
                Value = q.RoomType
            }).GroupBy(p => p.Text).Select(g => g.First()).ToList();
            var model = new CreateBookingVM 
            {
                Rooms = roomItems
            };
            return View(model);
        }

        // POST: BookingController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateBookingVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var startDate = Convert.ToDateTime(model.Date_From);
                var endDate = Convert.ToDateTime(model.Date_To);
                var rooms = _roomRepo.FindAll();
                var bookings = _bookingRepo.FindAll();

                var bookingexists= bookings
                    .Where(q => !(q.Date_From > endDate || q.Date_To < startDate))
                    .ToList();

                var checkbooking = bookingexists
                    .Where(q => q.Cancelled == true)       
                    .ToList();

                foreach(Booking cancelled in checkbooking) 
                { 
                    bookingexists.Remove(bookingexists.FirstOrDefault());
                }

                if (DateTime.Compare(startDate, DateTime.Now) < 0)
                {
                    ModelState.AddModelError("", "The Start Date can not be in the past");
                    return View(model);
                }
                if (DateTime.Compare(startDate, endDate) > 0)
                {
                    ModelState.AddModelError("", "The Start Date cannot be further in the future than the End Date");
                    return View(model);
                }

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


                var client = _userManager.GetUserAsync(User).Result;

                var bookingModel = new BookingVM
                {
                    ClientId = client.Id,
                    Date_From = startDate,
                    Date_To = endDate,
                    RoomType = model.RoomType
                };
                var booking = _mapper.Map<Booking>(bookingModel);

                booking.Rooms = new List<Room>();
                var room = rooms
                    .Where(q => q.RoomType == bookingModel.RoomType)
                    .FirstOrDefault();
                if(room == null)
                {
                    ModelState.AddModelError("", "There is no room available of this type");
                    return View(model);
                }

                booking.Rooms.Add(room);    

                var isSucces = _bookingRepo.Create(booking);

                if (!isSucces)
                {
                    ModelState.AddModelError("", "Something went wrong with submiting your booking");
                    return View(model);
                }
                return RedirectToAction(nameof(MyBookings));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(model);
            }
        }

       

        // GET: BookingController/Delete/5
        public ActionResult Delete(int id)
        {
            var bookings = _bookingRepo.FindById(id);
            if (bookings == null)
            {
                return NotFound();
            }
            var isSucces = _bookingRepo.Delete(bookings);
            if (!isSucces)
            {
                return BadRequest();
            }
            return RedirectToAction(nameof(Index));
        }

        private void CancelRequest(int id)
        {
            var booking = _bookingRepo.FindById(id);
            booking.Cancelled = true;
            _bookingRepo.Update(booking);
        }
        public ActionResult CancelForUser(int id)
        {
            CancelRequest(id);
            return RedirectToAction("MyBookings");
        }
        public ActionResult CancelForAdmin(int id)
        {
            CancelRequest(id);
            return RedirectToAction("Index");
        }
    }
}
