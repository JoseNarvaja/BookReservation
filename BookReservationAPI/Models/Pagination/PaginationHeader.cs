namespace BookReservationAPI.Models.Pagination
{
    public class PaginationHeader
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public PaginationHeader(int pageNumber, int pageSize, int totalItems)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
            TotalItems = totalItems;
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        }
    }
}
