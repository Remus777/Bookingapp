
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
        public List<int> FindAllIds();
        public List<string> FindAllClientIds();
        ICollection<Booking> GetUsedBookings(DateTime startDate, DateTime endDate);
    }
}
