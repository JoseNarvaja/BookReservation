using BookReservationAPI.Models;
using BookReservationAPI.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookReservationAPI.Data.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<LocalUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        public DbInitializer(UserManager<LocalUser> userManager ,RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }


        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            if(!_roleManager.RoleExistsAsync(StaticData.RoleAdmin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(StaticData.RoleAdmin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticData.RoleCustomer)).GetAwaiter().GetResult();
            }

            _userManager.CreateAsync(new LocalUser
            {
                UserName = "admin",
                Name = "admin",
                Surname = "admin",
                Email = "admin@admin.com"
            }, "Admintest1#").GetAwaiter().GetResult();

            LocalUser admin = _context.Users.FirstOrDefault(u => u.UserName == "admin");
            _userManager.AddToRoleAsync(admin, StaticData.RoleAdmin).GetAwaiter().GetResult();
        }
    }
}
