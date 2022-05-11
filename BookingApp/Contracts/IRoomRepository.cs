using BookingApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Contracts
{
    public interface IRoomRepository : IRepositoryBase<Room>
    {
        ICollection<Room> GetRoomsByBookings(int bookingid);
        bool roomExists(int roomnb);
    }
}
