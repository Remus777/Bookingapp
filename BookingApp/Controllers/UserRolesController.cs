using AutoMapper;
using BookingApp.Areas.Identity.Pages.Account;
using BookingApp.Contracts;
using BookingApp.Data;
using BookingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserRolesController : Controller
    {
        private readonly UserManager<Client> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Client> _passwordHasher;
        public UserRolesController(
            IMapper mapper, 
            UserManager<Client> userManager, 
            ILogger<RegisterModel> logger, 
            RoleManager<IdentityRole> roleManager,
            IPasswordHasher<Client> passwordHasher)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager= roleManager;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }
        // GET: UserRoles
        public ActionResult Index()
        {
            var clients = _userManager.Users;
            var model = _mapper.Map<List<ClientVM>>(clients);
            return View(model);
        }

        // GET: UserRoles/Details/5
        public ActionResult Details(string id)
        {
            var model = _mapper.Map<ClientVM>(_userManager.FindByIdAsync(id).Result);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // GET: UserRoles/Create
        public ActionResult Create()
        {
            return View();
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
                var user = new Client
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber
                };
                //var users = _mapper.Map<Client>(user);
                IdentityResult result = _userManager.CreateAsync(user, model.Password).Result;
                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "Client").Wait();
                    _logger.LogInformation("User created a new account with password.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong...");
                    return View(model);
                }
             
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
            var user = _mapper.Map<ClientVM>(_userManager.FindByIdAsync(id).Result);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }

        // POST: UserRoles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClientVM model)
        {
            try
            {
                var user = _userManager.FindByIdAsync(model.Id).Result;
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(model.Email))
                    {
                        user.Email = model.Email;
                        user.UserName = model.Email;
                    }
                    else
                        ModelState.AddModelError("", "Email cannot be empty");
                    if (!string.IsNullOrEmpty(model.PhoneNumber))
                        user.PhoneNumber = model.PhoneNumber;
                    else
                        ModelState.AddModelError("", "Phone number cannot be empty");
                    if (!string.IsNullOrEmpty(model.FirstName))
                        user.FirstName = model.FirstName;
                    else
                        ModelState.AddModelError("", "First name cannot be empty");
                    if (!string.IsNullOrEmpty(model.LastName))
                        user.LastName = model.LastName;
                    else
                        ModelState.AddModelError("", "Last name cannot be empty");
                    if (!string.IsNullOrEmpty(model.Password))
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
                    else
                        ModelState.AddModelError("", "Password cannot be empty");
                    user.Address = model.Address;
                    if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Password))
                    {
                        IdentityResult result = _userManager.UpdateAsync(user).Result;
                        if (result.Succeeded)
                            return RedirectToAction(nameof(Index));
                        else
                        {
                            ModelState.AddModelError("", "Something went wrong...");
                            return View(model);
                        }
                    }
                }
                ModelState.AddModelError("", "Something went wrong...");
                return View(model);
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
            var user = _userManager.FindByIdAsync(id).Result;
            if (user != null)
            {
                IdentityResult result = _userManager.DeleteAsync(user).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index)); ;
                }
            }
            ModelState.AddModelError("", "User Not Found");
            return View("Index");
        }

    }
}
