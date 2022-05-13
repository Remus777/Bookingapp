using AutoMapper;
using BookingApp.Areas.Identity.Pages.Account;
using BookingApp.Contracts;
using BookingApp.Data;
using BookingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            
            var mappedClients = _mapper.Map<List<ClientVM>>(clients);
            var model = new ClientWithRoleVM
            {
                Clients = mappedClients
            };
            foreach (Client client in clients)
            {
                var clientRole = _userManager.GetRolesAsync(client).Result.FirstOrDefault();
                var clientModel = model.Clients.Where(q => q.Id == client.Id).FirstOrDefault();
                clientModel.RoleName = clientRole;
            }
           
            return View(model);
        }

        // GET: UserRoles/Details/5
        public ActionResult Details(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            var userMapped = _mapper.Map<ClientVM>(user);
            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            var model = new ClientDetailsVM
            {
                Client = userMapped,
                RoleName = role
            };
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // GET: UserRoles/Create
        public ActionResult Create()
        {
            var roles = _roleManager.Roles.ToList();
            var roleItems = roles.Select(q => new SelectListItem
            {
                Text = q.Name,
                Value = q.Name
            }).ToList();
            var model = new ClientVMnoID
            {
                Roles = roleItems
            };
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
                var user = new Client
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber
                    
                };
                IdentityResult result = _userManager.CreateAsync(user, model.Password).Result;
                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, model.RoleName).Wait();
                    _logger.LogInformation("Admin created a new account with password.");
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

            var user = _userManager.FindByIdAsync(id).Result;
            var userMapped = _mapper.Map<ClientVM>(user);
            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            var model = new ClientDetailsVM
            {
                Client = userMapped,
                RoleName = role
            };
            if (model != null)
                return View(model);
            else
                return RedirectToAction("Index");
        }

        // POST: UserRoles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClientDetailsVM model)
        {
            try
            {
                var user = _userManager.FindByIdAsync(model.Client.Id).Result;
                var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(model.Client.Email))
                    {
                        user.Email = model.Client.Email;
                        user.UserName = model.Client.Email;
                    }
                    else
                        ModelState.AddModelError("", "Email cannot be empty");
                    if (!string.IsNullOrEmpty(model.Client.PhoneNumber))
                        user.PhoneNumber = model.Client.PhoneNumber;
                    else
                        ModelState.AddModelError("", "Phone number cannot be empty");
                    if (!string.IsNullOrEmpty(model.Client.FirstName))
                        user.FirstName = model.Client.FirstName;
                    else
                        ModelState.AddModelError("", "First name cannot be empty");
                    if (!string.IsNullOrEmpty(model.Client.LastName))
                        user.LastName = model.Client.LastName;
                    else
                        ModelState.AddModelError("", "Last name cannot be empty");
                    if (!string.IsNullOrEmpty(model.RoleName))
                        role = model.RoleName;
                    else
                        ModelState.AddModelError("", "Role name cannot be empty");
                    user.Address = model.Client.Address;
                    if (!string.IsNullOrEmpty(model.Client.Email))
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

        public ActionResult EditPassword(string id)
        {
            var user = _mapper.Map<ClientPasswordVM>(_userManager.FindByIdAsync(id).Result);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Edit");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPassword(ClientPasswordVM model)
        {
            try
            {
                var user = _userManager.FindByIdAsync(model.Id).Result;
                if (!string.IsNullOrEmpty(model.Password))
                    user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
                else
                    ModelState.AddModelError("", "Password cannot be empty");
                if (!string.IsNullOrEmpty(model.Password))
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
                ModelState.AddModelError("", "Something went wrong...");
                return View(model);
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong...");
                return View(model);
            }
        }
    }
}
