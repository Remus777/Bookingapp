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
                    return View(model);
                }

                var clientId = _userManager.GetUserId(User);
                var modelMapping = _mapper.Map<CreateBookingDTO>(model);

                int flag = _bookingServices.CreateBookingPOST(modelMapping, clientId);

                var modelMapBack = _mapper.Map<CreateBookingVM>(modelMapping);
                switch (flag)
                {
                    case 1:
                        ModelState.AddModelError("", "The Start Date can not be in the past");
                        return View(modelMapBack);
                    case 2:
                        ModelState.AddModelError("", "The Start Date cannot be further in the future than the End Date");
                        return View(modelMapBack);
                    case 3:
                        ModelState.AddModelError("", "There is no room available of this type");
                        return View(modelMapBack);
                    case 4:
                        ModelState.AddModelError("", "Something went wrong with submiting your booking");
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
            if(flag == 1)
            {
                return NotFound();
            }
            if(flag == 2)
            {
                return BadRequest();
            }
            return RedirectToAction("Index");
        }

        
        public ActionResult CancelForUser(int id)
        {
            var flag = _bookingServices.CancelRequest(id);
            if (flag == 1)
            {
                return NotFound();
            }
            if (flag == 2)
            {
                return BadRequest();
            }
            return RedirectToAction("MyBookings");
        }
        public ActionResult CancelForAdmin(int id)
        {
            var flag = _bookingServices.CancelRequest(id);
            if (flag == 1)
            {
                return NotFound();
            }
            if (flag == 2)
            {
                return BadRequest();
            }
            return RedirectToAction("Index");
        }
    }
}
