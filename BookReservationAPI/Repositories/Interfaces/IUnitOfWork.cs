namespace BookReservationAPI.Repository.Interfaces
{
    public interface IUnitOfWork
    {
        public IBookRepository Books { get; }
        public ICategoryRepository Categories { get; }
        public IReservationRepository Reservations { get; }
        public ILocalUserRepository LocalUsers { get; }
        Task Save();
    }
}
