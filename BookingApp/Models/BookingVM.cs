using BookingApp.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Models
{
    public class BookingVM
    {
        public int BookingId { get; set; }
        [Required]
        public DateTime Date_From { get; set; }
        [Required]
        public DateTime Date_To { get; set; }
        public ClientVM Client { get; set; }
        public string ClientId { get; set; }
        public IEnumerable<SelectListItem> Clients { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
