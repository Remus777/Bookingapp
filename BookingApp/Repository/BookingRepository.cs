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
    }
}
