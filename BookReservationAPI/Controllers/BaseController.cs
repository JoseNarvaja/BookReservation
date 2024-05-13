using BookReservationAPI.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookReservationAPI.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected ActionResult HandleException(Exception e)
        {
            HttpStatusCode statusCode;
            string message;

            switch (e)
            {
                case ArgumentException argumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = argumentException.Message;
                    break;
                case KeyNotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = notFoundException.Message;
                    break;
                default:
                    throw e;
            }

            throw new BusinessException(statusCode, message);
        }

    }
}
