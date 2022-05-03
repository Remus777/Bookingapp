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
            return View();
        }

        public ActionResult MyBookings()
        {
            var client = _userManager.GetUserAsync(User).Result;
            var clientId = client.Id;
            var clientBookings = _bookingRepo.GetBookingsByClient(clientId);
            var rooms = _roomRepo.FindAll();
            var room = new List<Room>();
           
            var clientRequestsModel = _mapper.Map<List<BookingVM>>(clientBookings);
            var clientRoomsModel = _mapper.Map<List<RoomVM>>(rooms);

            var model = new ClientBookingRequestVM
            {
                Rooms = clientRoomsModel,
                Bookings = clientRequestsModel
            };
            return View(model);
        }

        // GET: BookingController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BookingController/Create
        public ActionResult Create()
        {
            var rooms = _roomRepo.FindAll();

            var roomItems = rooms.Select(q => new SelectListItem
            {
                Text = q.RoomType,
                Value = q.Id.ToString()
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
                var startDate = Convert.ToDateTime(model.Date_From);
                var endDate = Convert.ToDateTime(model.Date_To);
                var rooms = _roomRepo.FindAll();

                var roomItems = rooms.Select(q => new SelectListItem
                {
                    Text = q.RoomType,
                    Value = q.Id.ToString()
                }).GroupBy(p => p.Text).Select(g => g.First()).ToList();
                model.Rooms = roomItems;

                if(!ModelState.IsValid)
                {
                    return View(model);
                }
                if (DateTime.Compare(startDate, DateTime.Now) < 0)
                {
                    ModelState.AddModelError("", "Start Date cannot be in the past");
                    return View(model);
                }
                if (DateTime.Compare(startDate, endDate) > 0)
                {
                    ModelState.AddModelError("", "Start Date cannot be further in the future than the End Date");
                    return View(model);
                }
                var client = _userManager.GetUserAsync(User).Result;

                var bookingModel = new BookingVM
                {
                    ClientId = client.Id,
                    Date_From = startDate,
                    Date_To = endDate,
                    RoomId = model.RoomId
                };
                var booking = _mapper.Map<Booking>(bookingModel);

                booking.Rooms = new List<Room>();
                var room = rooms
                    .Where(q => q.Id == bookingModel.RoomId)
                    .FirstOrDefault();
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

        // GET: BookingController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BookingController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BookingController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BookingController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
