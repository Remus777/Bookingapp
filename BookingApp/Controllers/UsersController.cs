using AutoMapper;
using BookingApp.Areas.Identity.Pages.Account;
using BookingApp.Contracts;
using BookingApp.Data;
using BookingApp.Models;
using BookingApp.Models.DataTrasnferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookingApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly IUsersServices _usersServices;
        private readonly UserManager<Client> _userManager;
        private readonly IMapper _mapper;
        public UsersController(
            IMapper mapper, 
            IUsersServices usersServices,
            UserManager<Client> userManager)
        {
            _mapper = mapper;
            _usersServices = usersServices;
            _userManager = userManager;
        }
        // GET: UserRoles
        public ActionResult Index()
        {
            var currentUser = _userManager.GetUserAsync(User).Result;

            var indexLogic = _usersServices.UsersIndex(currentUser);
            var indexLogicMapping = _mapper.Map<ClientWithRoleVM>(indexLogic);
            return View(indexLogicMapping);
        }

         public ActionResult Details(string id)
        {
            var userDetails = _usersServices.DetailsUser(id);
            if (userDetails == null)
            {
                return NotFound();
            }
            var userMapped = _mapper.Map<ClientDetailsVM>(userDetails);

            return View(userMapped);
        }

        // GET: UserRoles/Create
        public ActionResult Create()
        {
            var createUserLogic = _usersServices.CreateUserGET();
            var model = _mapper.Map<ClientVMnoID>(createUserLogic);
            return View(model);
        }

        // POST: UserRoles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClientVMnoID model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var modelMapping = _mapper.Map<ClientDTOnoID>(model);
                var flag = _usersServices.CreateUserPOST(modelMapping);
                var modelMapback = _mapper.Map<ClientVMnoID>(modelMapping);
                if (flag == 1)
                {
                    ModelState.AddModelError("", "Something went wrong...");
                    return View(modelMapback);
                }            
                return RedirectToAction(nameof(Index));

            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong...");
                return View(model);
            }
        }

        // GET: UserRoles/Edit/5
        public ActionResult Edit(string id)
        {

           var editLogic = _usersServices.EditUserGET(id);
           var model = _mapper.Map<ClientEditVM>(editLogic);
           if (model == null)
           {
                return NotFound();
           }
                
            return View(model);
        }

        // POST: UserRoles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClientEditVM model)
        {
            try
            {
                var modelMapping = _mapper.Map<ClientEditDTO>(model);
                var flag = _usersServices.EditUserPOST(modelMapping);
                switch(flag)
                {
                    case 1:
                        ModelState.AddModelError("", "Email cannot be empty");
                        return View(model);
                    case 2:
                        ModelState.AddModelError("", "Phone number cannot be empty");
                        return View(model);
                    case 3:
                        ModelState.AddModelError("", "First name cannot be empty");
                        return View(model);
                    case 4:
                        ModelState.AddModelError("", "Last name cannot be empty");
                        return View(model);
                    case 5:
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

        // GET: UserRoles/Delete/5
        public ActionResult Delete(string id)
        {
            var flag = _usersServices.DeleteUser(id);
            switch (flag)
            {
                case 1:
                    return NotFound();
                case 2:
                    return BadRequest();
            }
            return RedirectToAction("Index");
        }

        public ActionResult EditPassword(string id)
        {
            var user = _usersServices.EditPasswordGET(id);
            var model = _mapper.Map<ClientPasswordVM>(user);
            if (model == null)
            {
                return RedirectToAction("Edit");
            }
            return View(model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPassword(ClientPasswordVM model)
        {
            try
            {
                var modelMapping = _mapper.Map<ClientPasswordDTO>(model);
                var flag = _usersServices.EditPasswordPOST(modelMapping);
                switch (flag)
                {
                    case 1:
                        ModelState.AddModelError("", "Password cannot be empty");
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
    }
}
