﻿using AutoMapper;
using BookingApp.Contracts;
using BookingApp.Data;
using BookingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class RoomsController : Controller
    {   
        private readonly IRoomRepository _repo;
        private readonly IMapper _mapper;

        public RoomsController(IRoomRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
       
        // GET: RoomsController
        public ActionResult Index()
        {
            var rooms = _repo.FindAll().ToList();
            var model = _mapper.Map<List<Room>, List<RoomVM>>(rooms);
            return View(model);
        }

        // GET: RoomsController/Details/5
        public ActionResult Details(int id)
        {
            if(!_repo.isExists(id))
            {
                return NotFound();
            }
            var rooms = _repo.FindById(id);
            var model = _mapper.Map<RoomVM>(rooms);
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
                var rooms = _mapper.Map<Room>(model);
                if(_repo.isExists(rooms.RoomNumber))
                {
                    ModelState.AddModelError("", "Room already exists");
                    return View(model);
                }
                var isSucces = _repo.Create(rooms);
                if(!isSucces)
                {
                    ModelState.AddModelError("", "Something went wrong...");
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong...");
                return View();
            }
        }

        // GET: RoomsController/Edit/5
        public ActionResult Edit(int id)
        {
            if (!_repo.isExists(id))
            {
                return NotFound();
            }
            var rooms = _repo.FindById(id);
            var model = _mapper.Map<RoomVM>(rooms);
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
                var rooms = _mapper.Map<Room>(model);
                var isSucces = _repo.Update(rooms);
                if (!isSucces)
                {
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
            var rooms = _repo.FindById(id);
            if (rooms == null)
            {
                return NotFound();
            }
            var isSucces = _repo.Delete(rooms);
            if (!isSucces)
            {
                return BadRequest();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
