using BookingApp.Models;
using BookingApp.Models.DataTrasnferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Contracts
{
    public interface IBookingServices
    {
        ClientBookingRequestDTO BookingIndex();
        ClientBookingRequestDTO MyBookings(string clientID);
        CreateBookingDTO CreateBookingGET();
        int CancelRequest(int id);
        int Delete(int id);
        int CreateBookingPOST(CreateBookingDTO model, string clientId);

    }
}
