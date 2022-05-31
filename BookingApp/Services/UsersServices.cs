using AutoMapper;
using BookingApp.Areas.Identity.Pages.Account;
using BookingApp.Contracts;
using BookingApp.Data;
using BookingApp.Models.DataTrasnferObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class UsersServices : IUsersServices
    {
        private readonly UserManager<Client> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Client> _passwordHasher;
        public UsersServices(
            IMapper mapper,
            UserManager<Client> userManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            IPasswordHasher<Client> passwordHasher)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public ClientDTOnoID CreateUserGET()
        {
            var model = new ClientDTOnoID
            {
                Roles = GetRolesToDropdown()
            };
            return model;
        }

        public int CreateUserPOST(ClientDTOnoID model)
        {
            model.Roles = GetRolesToDropdown();
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
            if (!result.Succeeded)
            {
                return 1;
            }
            _userManager.AddToRoleAsync(user, model.RoleName).Wait();
            _logger.LogInformation("Admin created a new account with password.");
            return 0;
        }

        public ClientDetailsDTO DetailsUser(string id)
        {
            if(id == null)
            {
                return null;
            }
            var user = _userManager.FindByIdAsync(id).Result;
            var userMapped = _mapper.Map<ClientDTO>(user);
            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            var model = new ClientDetailsDTO
            {
                Client = userMapped,
                RoleName = role
            };
            return model;
        }

        public ClientEditDTO EditUserGET(string id)
        {
            if (id == null)
            {
                return null;
            }
            var user = _userManager.FindByIdAsync(id).Result;
            var userMapped = _mapper.Map<ClientDTO>(user);
            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();

            var model = new ClientEditDTO
            {
                Client = userMapped,
                Roles = GetRolesToDropdown(),
                RoleName = role
            };

            return model;
        }
        public int EditUserPOST(ClientEditDTO model)
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
                    return 1;
                if (!string.IsNullOrEmpty(model.Client.PhoneNumber))
                    user.PhoneNumber = model.Client.PhoneNumber;
                else
                    return 2;
                if (!string.IsNullOrEmpty(model.Client.FirstName))
                    user.FirstName = model.Client.FirstName;
                else
                    return 3;
                if (!string.IsNullOrEmpty(model.Client.LastName))
                    user.LastName = model.Client.LastName;
                else
                    return 4;

                user.Address = model.Client.Address;

                IdentityResult userresult = _userManager.UpdateAsync(user).Result;
                if (role != model.RoleName)
                {
                    _userManager.RemoveFromRoleAsync(user, role).Wait();
                    _userManager.AddToRoleAsync(user, model.RoleName).Wait();
                }

                if (!userresult.Succeeded)
                {     
                    return 5;
                }
                return 0;
            }
            return 5;
        }

        private List<SelectListItem> GetRolesToDropdown()
        {
            var roles = _roleManager.Roles.ToList();
            var roleItems = roles.Select(q => new SelectListItem
            {
                Text = q.Name,
                Value = q.Name
            }).ToList();
            return roleItems;
        }
        public ClientWithRoleDTO UsersIndex(Client currentUser)
        {
            var clients = _userManager.Users.ToList();

            if (clients.Where(q => q.Id == currentUser.Id).FirstOrDefault() == currentUser)
            {
                clients.Remove(currentUser);
            }

            var mappedClients = _mapper.Map<List<ClientDTO>>(clients);
            var model = new ClientWithRoleDTO
            {
                Clients = mappedClients
            };
            foreach (Client client in clients)
            {
                var clientRole = _userManager.GetRolesAsync(client).Result.FirstOrDefault();
                var clientModel = model.Clients.Where(q => q.Id == client.Id).FirstOrDefault();
                clientModel.RoleName = clientRole;
            }
            return model;
        }

        public ClientPasswordDTO EditPasswordGET(string id)
        {
            if (id == null)
            {
                return null;
            }
            var user = _userManager.FindByIdAsync(id).Result;
            var model = _mapper.Map<ClientPasswordDTO>(user);
            return model;
        }

        public int EditPasswordPOST(ClientPasswordDTO model)
        {
            var user = _userManager.FindByIdAsync(model.Id).Result;
            if (user != null)
            {
                if (!string.IsNullOrEmpty(model.Password))
                    user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
                else
                    return 1;
                if (!string.IsNullOrEmpty(model.Password))
                {
                    IdentityResult result = _userManager.UpdateAsync(user).Result;
                    if (!result.Succeeded)
                    {
                        return 2;
                    }
                    return 0;
                }
            }
            return 2;
        }

        public int DeleteUser(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user != null)
            {
                IdentityResult result = _userManager.DeleteAsync(user).Result;
                if (!result.Succeeded)
                {
                    return 1;
                }
                return 0;
            }
            return 2;
        }
    }
}
