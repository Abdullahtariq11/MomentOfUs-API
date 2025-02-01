using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.Json;
using MomentOfUs.Domain.Exceptions;
using MomentOfUs.Domain.Models;
using Newtonsoft.Json;
using Serilog;

namespace MomentOfUs.API.Extensions
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

        public GlobalErrorHandlingMiddleware( RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
        {

            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); //pass to next middleware
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occured");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            if (ex is NotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                _logger.LogWarning("NotFoundException: {Message}", ex.Message);
            }
            else if (ex is BadRequestException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogWarning("BadRequestException: {Message}", ex.Message);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                _logger.LogWarning(ex, "Internal Server Error");
            }

            var response= new ErrorDetails
            {
                StatusCode = context.Response.StatusCode,
                ErrorMessage=ex.Message,
            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}