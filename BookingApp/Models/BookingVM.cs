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
        public int Id { get; set; }
        [Required]
        public DateTime Date_From { get; set; }
        [Required]
        public DateTime Date_To { get; set; }
        public ClientVM Client { get; set; }
        public string ClientId { get; set; }
        public RoomVM Room { get; set; }
        public int RoomId { get; set; }
    }
    public class CreateBookingVM
    {
        [Display(Name = "Start Date")]
        [Required]
        public string Date_From { get; set; }
        [Display(Name = "End Date")]
        [Required]
        public string Date_To { get; set; }
        public IEnumerable<SelectListItem> Rooms { get; set; }
        [Display(Name = "Room Type")]
        public int RoomId { get; set; }
    }
    public class ClientBookingRequestVM
    {
        public List<RoomVM> Rooms { get; set; }
        public List<BookingVM> Bookings { get; set; }
    }
}
