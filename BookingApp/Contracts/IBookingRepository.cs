
using BookingApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Contracts
{
    public interface IBookingRepository : IRepositoryBase<Booking>
    {
        ICollection<Booking> GetBookingsByClient(string clientid);
        ICollection<Booking> GetBookingsByRoom(int roomid);
    }
}
