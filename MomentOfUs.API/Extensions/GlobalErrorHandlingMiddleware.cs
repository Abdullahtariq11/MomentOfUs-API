using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MomentOfUs.Domain.Exceptions;
using MomentOfUs.Domain.Models;
using Newtonsoft.Json;

namespace MomentOfUs.API.Extensions
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

        public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Pass request to the next middleware
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

 private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            /// If the exception is of type NotFoundException, it returns a 404 Not Found status and logs a warning.
            if (ex is NotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                _logger.LogWarning("NotFoundException: {Message}", ex.Message);
            }

            /// For BadRequestException, it returns a 400 Bad Request.
            else if (ex is BadRequestException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogWarning("BadRequestException: {Message}", ex.Message);
            }

            /// For all other exceptions, it returns a 500 Internal Server Error and logs the error.
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                _logger.LogWarning(ex, "Internal Server Error");
            }
            var response = new ErrorDetails
            {
                StatusCode = context.Response.StatusCode,
                ErrorMessage = ex.Message
            };

            /// The error details are serialized into JSON format using JsonConvert.SerializeObject() and sent back to the client.
            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));

        }
    }
}