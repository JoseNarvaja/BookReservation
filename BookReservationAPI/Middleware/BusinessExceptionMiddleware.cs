using BookReservationAPI.Exceptions;
using BookReservationAPI.Models;
using System.Net;
using System.Text.Json;

namespace BookReservationAPI.Middleware
{
    public class BusinessExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<BusinessExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public BusinessExceptionMiddleware(RequestDelegate next, ILogger<BusinessExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _environment = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, "BusinessException occurred.");

                context.Response.StatusCode = (int)ex.StatusCode;

                var response = new APIResponse
                {
                    StatusCode = ex.StatusCode,
                    Messages = new List<string> { ex.Message },
                    Success = false
                };

                var responseJson = JsonSerializer.Serialize(response);

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(responseJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Messages = new List<string> { "An unexpected error occurred." },
                    Success = false
                };

                var responseJson = JsonSerializer.Serialize(response);

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(responseJson);
            }
        }
    }
}
