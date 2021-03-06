using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Models.DataTrasnferObjects
{
    public class BookingDTO
    {
        public int Id { get; set; }
        [Required]
        public DateTime Date_From { get; set; }
        [Required]
        public DateTime Date_To { get; set; }
        public ClientDTO Client { get; set; }
        public string ClientId { get; set; }
        public List<RoomDTO> Rooms { get; set; }
        public int NrOfRooms { get; set; }
        public bool Cancelled { get; set; }
    }
    public class CreateBookingDTO
    {
        [Display(Name = "Start Date")]
        [Required]
        public string Date_From { get; set; }
        [Display(Name = "End Date")]
        [Required]
        public string Date_To { get; set; }
        public IList<SelectListItem> Rooms { get; set; }
        [Display(Name = "Room Type")]
        public List<BookingRoomTypesDTO> RoomTypes { get; set; }
        public int NrOfRoomTypes { get; set; }
    }
    public class ClientBookingRequestDTO
    {
        public List<BookingDTO> Bookings { get; set; }
    }
    public class BookingRoomTypesDTO
    {
        public string RoomType { get; set; }
        public int? NrOfRooms { get; set; }
    }

}
