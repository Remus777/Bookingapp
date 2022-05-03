using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Data
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int RoomNumber { get; set; }
        [Required]
        public string RoomType { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
