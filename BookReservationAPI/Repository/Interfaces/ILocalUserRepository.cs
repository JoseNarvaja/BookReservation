using BookReservationAPI.Models;

namespace BookReservationAPI.Repository.Interfaces
{
    public interface ILocalUserRepository : IRepository<LocalUser>
    {
        void Update(LocalUser user);
    }
}
