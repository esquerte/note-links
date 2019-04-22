using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NoteLinks.Service.Logging.Service;
using Microsoft.Extensions.Logging;

namespace NoteLinks.Service.ExceptionHandling
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        private readonly IHostingEnvironment _environment;
        private readonly LoggingService _loggingService;

        public ErrorHandlingMiddleware(RequestDelegate next, IHostingEnvironment environment, IHostedService loggingService)
        {
            _environment = environment;
            _loggingService = loggingService as LoggingService;

            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                //_loggingService.Log(LogLevel.Warning, $"Application error. {exception.GetBaseException().Message}");
                await HandleExceptionAsync(context, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            ApiError apiError = null;
            string message = "";

            if (exception is ApiException)
            {
                var apiException = exception as ApiException;

                message = apiException.Message;

                apiError = new ApiError(message);

                context.Response.StatusCode = apiException.StatusCode;

                // Add data in a queue for logging
                _loggingService.Log(LogLevel.Warning, $"Application error. {message}");
            }
            else if (exception is UnauthorizedAccessException)
            {
                message = "Unauthorized access.";

                apiError = new ApiError(message);

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                // Add data in a queue for logging
                _loggingService.Log(LogLevel.Warning, $"Application error. {message}");
            }
            else
            {
                string stack;

                if (_environment.IsDevelopment())
                {
                    message = exception.GetBaseException().Message;
                    stack = exception.StackTrace;
                }
                else
                {
                    message = "Internal server error.";
                    stack = null;
                }

                apiError = new ApiError(message);
                apiError.Detail = stack;

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                // Add data in a queue for logging
                _loggingService.Log(LogLevel.Error, $"Application error. {message}");
            }

            var result = JsonConvert.SerializeObject(apiError);
            return context.Response.WriteAsync(result);
        }
    }
}
