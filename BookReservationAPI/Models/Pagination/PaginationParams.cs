namespace BookReservationAPI.Models.Pagination
{
    public class PaginationParams
    {
        private const int maxPageSize = 50;
        private int _pageSize = 10;
        private int _pageNumber = 1;
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value <= 0 ? 1 : value;
        }
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > maxPageSize ? maxPageSize : value;
        }

    }
}
