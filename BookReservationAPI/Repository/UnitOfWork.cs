﻿using BookReservationAPI.Data;
using BookReservationAPI.Repository.Interfaces;

namespace BookReservationAPI.Repository
{
    public class UnitOfWork : IUnitOfWork
    { 
        private readonly AppDbContext _dbContext;
        public IBookRepository Books { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public IReservationRepository Reservations { get; private set; }
        public ILocalUserRepository LocalUsers { get; private set; }
        public UnitOfWork(AppDbContext context)
        {
            _dbContext = context;
            Books = new BookRepository(context);
            Categories = new CategoryRepository(context);
            Reservations = new ReservationRepository(context);
            LocalUsers = new LocalUserRepository(context);
        }

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
