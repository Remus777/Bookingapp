using AutoMapper;
using BookingApp.Contracts;
using BookingApp.Models;
using BookingApp.Models.DataTrasnferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class RoomsController : Controller
    {   
        private readonly IRoomServices _roomServices;
        private readonly IMapper _mapper;

        public RoomsController(
            IRoomServices roomServices,
            IMapper mapper)
        {
            _roomServices = roomServices;
            _mapper = mapper;
        }
       
        // GET: RoomsController
        public ActionResult Index()
        {
            var rooms = _roomServices.RoomsIndex();
            var model = _mapper.Map<List<RoomVM>>(rooms);
            return View(model);
        }

        // GET: RoomsController/Details/5
        public ActionResult Details(int id)
        {
            var room = _roomServices.DetailsRoom(id);
            if (room == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<RoomVM>(room);
            return View(model);
        }

        // GET: RoomsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoomsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoomVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var room = _mapper.Map<RoomDTO>(model);
                int flag = _roomServices.CreateRoom(room);
                switch (flag)
                {
                    case 1:
                        ModelState.AddModelError("", "Room already exists");
                        return View(model);
                    case 2:
                        ModelState.AddModelError("", "Something went wrong...");
                        return View(model);
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong...");
                return View(model);
            }
        }

        // GET: RoomsController/Edit/5
        public ActionResult Edit(int id)
        {
            var room = _roomServices.DetailsRoom(id);
            if (room == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<RoomVM>(room);
            return View(model);
        }

        // POST: RoomsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RoomVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var room = _mapper.Map<RoomDTO>(model);      
                int flag = _roomServices.EditRoom(room);
                
                switch (flag)
                {
                    case 1:
                        ModelState.AddModelError("", "Room already exists");
                        return View(model);
                    case 2:
                        ModelState.AddModelError("", "Something went wrong...");
                        return View(model);
                }
               
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong...");
                return View(model);
            }
        }

        // GET: RoomsController/Delete/5
        public ActionResult Delete(int id)
        {
            var flag = _roomServices.DeleteRoom(id);
            switch (flag)
            {
                case 1:
                    ModelState.AddModelError("", "Can't delete a booked room");
                    return RedirectToAction(nameof(Index));
                case 2:
                    return NotFound();
                case 3:
                    return BadRequest();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
