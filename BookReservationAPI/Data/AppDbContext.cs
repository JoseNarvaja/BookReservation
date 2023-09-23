using Microsoft.EntityFrameworkCore;
using BookReservationAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BookReservationAPI.Data
{
    public class AppDbContext : IdentityDbContext<LocalUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<LocalUser> LocalUsers { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
    }
}
