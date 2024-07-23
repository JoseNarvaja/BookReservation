using BookReservationAPI.Models;
using BookReservationAPI.Repository.Interfaces;

namespace BookReservationAPI.Repositories.Interfaces
{
    public interface ICopyRepository : IRepository<Copy>
    {
        void Update(Copy copy);
    }
}
