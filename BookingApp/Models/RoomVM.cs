using BookingApp.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Models
{
    public class RoomVM
    {
        public int RoomId { get; set; }
        [Required]
        public string RoomType { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
