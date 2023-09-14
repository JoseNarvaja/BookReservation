namespace BookReservationAPI.Repository.Interfaces
{
    public interface IUnitOfWork
    {
        public IBookRepository Books { get; }
        public ICategoryRepository Categories { get; }
        Task Save();
    }
}
