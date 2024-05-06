using System.Net;

namespace BookReservationAPI.Exceptions
{
    public class BusinessException : Exception
    {
        public HttpStatusCode StatusCode { get;}
        public BusinessException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
