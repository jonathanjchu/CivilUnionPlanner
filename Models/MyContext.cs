using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Wedding> Weddings { get; set; }
        public DbSet<GuestWedding> GuestWeddings { get; set; }

        public List<Wedding> GetAllWeddings()
        {
            var weddings = Weddings.Include(w => w.GuestList).ThenInclude(gw => gw.User).ToList();

            weddings.RemoveAll(w => w.WeddingDate < DateTime.Now);

            SaveChanges();

            return weddings;
        }

        public bool InsertWedding(Wedding wedding, int userid)
        {
            var user = Users.FirstOrDefault(u => u.UserId == userid);

            if (user != null)
            {
                wedding.UserId = user.UserId;

                wedding.GuestList = new List<GuestWedding>();
                wedding.GuestList.Add(new GuestWedding()
                {
                    WeddingId = wedding.WeddingId,
                    UserId = userid
                });

                Add(wedding);
                SaveChanges();


                return true;
            }
            return false;
        }

        public bool JoinWeddingGuests(int weddingId, int userId)
        {

            var wedding = Weddings.Include(w => w.GuestList)
                                    .ThenInclude(gw => gw.User)
                                    .FirstOrDefault(w => w.WeddingId == weddingId);
            if (wedding != null)
            {
                wedding.GuestList.Add(new GuestWedding()
                {
                    UserId = userId,
                    WeddingId = weddingId
                });

                SaveChanges();

                return true;
            }

            return false;
        }

        public bool LeaveWedding(int weddingId, int userId)
        {
            var wedding = Weddings.Include(w => w.GuestList)
                                    .ThenInclude(gw => gw.User)
                                    .FirstOrDefault(w => w.WeddingId == weddingId);

            if (wedding != null)
            {
                wedding.GuestList.RemoveAll(gw => gw.WeddingId == weddingId && gw.UserId == userId);

                SaveChanges();

                return true;
            }

            return false;
        }

        public bool DeleteWedding(int weddingId, int userId)
        {
            var wedding = Weddings.Include(w => w.GuestList)
                                    .SingleOrDefault(w => w.WeddingId == weddingId && w.UserId == userId);


            if (wedding != null)
            {
                GuestWeddings.RemoveRange(wedding.GuestList);
                Weddings.Remove(wedding);

                SaveChanges();

                return true;
            }
            else
            {
                return false;
            }

        }

    }
}