using AutoMapper;
using BookingApp.Contracts;
using BookingApp.Data;
using BookingApp.Models;
using BookingApp.Models.DataTrasnferObjects;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IBookingServices _bookingServices;
        private readonly IMapper _mapper;
        private readonly UserManager<Client> _userManager;
        public BookingController(
            IBookingServices bookingServices,
            IMapper mapper,
            UserManager<Client> userManager
        )
        {
            _bookingServices = bookingServices;
            _mapper = mapper;
            _userManager = userManager;
        }
        // GET: BookingController
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var Indexlogic = _bookingServices.BookingIndex();
            var IndexlogicMapping = _mapper.Map<ClientBookingRequestVM>(Indexlogic);
            return View(IndexlogicMapping);
        }
        public ActionResult IndexDetails(int id)
        {
            var IndexDetailsLogic = _bookingServices.Details(id);
            var IndexDetailsLogicMapping = _mapper.Map<BookingVM>(IndexDetailsLogic);
            return View(IndexDetailsLogicMapping);
        }
        public ActionResult MyBookingsDetails(int id)
        {
            var myBookingDetailsLogic = _bookingServices.Details(id);
            var myBookingDetailsLogicMapping = _mapper.Map<BookingVM>(myBookingDetailsLogic);
            return View(myBookingDetailsLogicMapping);
        }
        public ActionResult MyBookings()
        {
            var clientId = _userManager.GetUserId(User);
            var myBookingLogic = _bookingServices.MyBookings(clientId);
            var myBookingLogicMapping = _mapper.Map<ClientBookingRequestVM>(myBookingLogic);
            return View(myBookingLogicMapping);
        }


        // GET: BookingController/Create
        public ActionResult Create()
        {
            var createBookingLogic = _bookingServices.CreateBookingGET();
            var createBookingLogicMapping = _mapper.Map<CreateBookingVM>(createBookingLogic);
            createBookingLogicMapping.RoomTypes = new List<BookingRoomTypesVM>(new BookingRoomTypesVM[createBookingLogicMapping.NrOfRoomTypes]);
            return View(createBookingLogicMapping);
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
                    ModelState.AddModelError("", "Something went wrong");
                    return View(model);
                }

                var clientId = _userManager.GetUserId(User);
                var modelMapping = _mapper.Map<CreateBookingDTO>(model);

                string flag = _bookingServices.CreateBookingPOST(modelMapping, clientId);

                var modelMapBack = _mapper.Map<CreateBookingVM>(modelMapping);               
                if (flag != null)
                {
                    ModelState.AddModelError("", flag);
                    return View(modelMapBack);
                }

                return RedirectToAction("MyBookings");
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(model);
            }
        }


        // GET: BookingController/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            var flag = _bookingServices.Delete(id);
            switch (flag)
            {
                case 1:
                    return NotFound();
                case 2:
                    return BadRequest();
            }
            return RedirectToAction("Index");
        }

        
        public ActionResult CancelForUser(int id)
        {
            var flag = _bookingServices.CancelRequest(id);
            switch (flag)
            {
                case 1:
                    return NotFound();
                case 2:
                    return BadRequest();
            }
            return RedirectToAction("MyBookings");
        }
        public ActionResult CancelForAdmin(int id)
        {
            var flag = _bookingServices.CancelRequest(id);
            switch (flag)
            {
                case 1:
                    return NotFound();
                case 2:
                    return BadRequest();
            }
            return RedirectToAction("Index");
        }
    }
}
