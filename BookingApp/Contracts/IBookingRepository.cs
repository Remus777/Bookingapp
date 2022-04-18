
using BookingApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Contracts
{
    interface IBookingRepository : IRepositoryBase<Booking>
    {
    }
}
