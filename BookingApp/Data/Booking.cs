using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Data
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date_From { get; set; }
        public DateTime Date_To { get; set; }
        [ForeignKey("ClientId")]
        public  Client Client { get; set; }
        public string ClientId { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
    }
}
