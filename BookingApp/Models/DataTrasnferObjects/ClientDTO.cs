﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Models.DataTrasnferObjects
{
    public class ClientDTO
    {
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [RegularExpression("^\\s*(?:\\+?(\\d{1,3}))?[-. (]*(\\d{3})[-. )]*(\\d{3})[-. ]*(\\d{4})(?: *x(\\d+))?\\s*$", ErrorMessage = "Phone number is not valid")]
        public string PhoneNumber { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Address { get; set; }
        [Required]
        public string RoleName { get; set; }

    }
    public class ClientWithRoleDTO
    {
        public List<ClientDTO> Clients { get; set; }

    }
    public class ClientDetailsDTO
    {
        public ClientDTO Client { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
    public class ClientEditDTO
    {
        public ClientDTO Client { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
    public class ClientDTOnoID
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [RegularExpression("^\\s*(?:\\+?(\\d{1,3}))?[-. (]*(\\d{3})[-. )]*(\\d{3})[-. ]*(\\d{4})(?: *x(\\d+))?\\s*$", ErrorMessage = "Phone number is not valid")]
        public string PhoneNumber { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Address { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
    public class ClientPasswordDTO
    {
        public string Id { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
