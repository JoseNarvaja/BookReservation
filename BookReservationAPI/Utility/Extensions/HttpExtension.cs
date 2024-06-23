using BookReservationAPI.Models.Pagination;
using System.Text.Json;

namespace BookReservationAPI.Utility.Extensions
{
    public static class HttpResponseExtension
    {
        public static void AddPaginationHeader(this HttpResponse httpResponse, PaginationHeader pagination)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            httpResponse.Headers.Add("Pagination", JsonSerializer.Serialize(pagination, jsonOptions));
            httpResponse.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
