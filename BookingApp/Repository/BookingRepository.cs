    using BookingApp.Contracts;
using BookingApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _db;
        public BookingRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool Create(Booking entity)
        {
            _db.Bookings.Add(entity);
            return Save();
        }

        public bool Delete(Booking entity)
        {
            _db.Bookings.Remove(entity);
            return Save();
        }

        public ICollection<Booking> FindAll()
        {
            var Bookings = _db.Bookings.ToList();
            return Bookings;
        }
        public List<int> FindAllIds()
        {
            var Bookings = _db.Bookings.Select(q => q.Id).ToList();
            return Bookings;
        }
        public List<String> FindAllClientIds()
        {
            var Bookings = _db.Bookings.Select(q => q.ClientId).ToList();
            return Bookings;
        }

        public Booking FindById(int id)
        {
            var Booking = _db.Bookings.Find(id);
            return Booking;
        }

        public ICollection<Booking> GetBookingsByClient(string clientid)
        {
            var bookings = FindAll()
                 .Where(q => q.ClientId == clientid)
                 .ToList();
            return bookings;
        }

        public ICollection<Booking> GetBookingsByRoom(int roomid)
        {
            var bookings = _db.Rooms
                    .Where(q => q.Id == roomid)
                    .SelectMany(c => c.Bookings)
                    .ToList();
            return bookings;
        }

        public bool isExists(int id)
        {
            var exists = _db.Bookings.Any(q => q.Id == id);
            return exists;
        }


        public bool Save()
        {
            var changes = _db.SaveChanges();
            return changes > 0;
        }

        public bool Update(Booking entity)
        {
            _db.Bookings.Update(entity);
            return Save();

        }

        public ICollection<Booking> GetUsedBookings(DateTime startDate, DateTime endDate)
        {
            var bookings = _db.Bookings.Where(q => !(q.Date_From > endDate || q.Date_To < startDate))
               .ToList();
            return bookings;
        }
    }
}
