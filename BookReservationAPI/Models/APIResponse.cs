using System.Net;

namespace BookReservationAPI.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
            Messages = new List<String>();
        }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> Messages { get; set; }
        public bool Success { get; set; }
        public object Result { get; set; }
    }
}
