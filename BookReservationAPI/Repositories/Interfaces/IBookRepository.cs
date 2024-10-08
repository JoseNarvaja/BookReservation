﻿using BookReservationAPI.Models;

namespace BookReservationAPI.Repository.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        void Update(Book book);
    }
}
