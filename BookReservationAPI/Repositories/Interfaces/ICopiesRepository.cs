﻿using BookReservationAPI.Models;
using BookReservationAPI.Repository.Interfaces;

namespace BookReservationAPI.Repositories.Interfaces
{
    public interface ICopiesRepository : IRepository<Copy>
    {
        void Update(Copy copy);
        Task MarkAsAvailable(int id);
        Task MarkAsUnavailable(int id);
        bool IsCopyAvailable(int id);
        Task<int> GetAvailableCopiesCountByISBN(string isbn);
    }
}
