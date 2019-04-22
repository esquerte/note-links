//using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NoteLinks.Service.Logging.Service;
using System;
//using Hosting = Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;

namespace NoteLinks.Service.ExceptionHandling
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        IHostingEnvironment _environment;
        LoggingService _loggingService;

        public ApiExceptionFilter(IHostingEnvironment environment, IHostedService loggingService)
        {
            _environment = environment;
            _loggingService = loggingService as LoggingService;
        }

        public override void OnException(ExceptionContext context)
        {
            ApiError apiError = null;
            string message = "";

            if (context.Exception is ApiException)
            {         
                var exception = context.Exception as ApiException;
                context.Exception = null;

                message = exception.Message;

                apiError = new ApiError(context.ModelState);
                apiError.Message = message;

                context.HttpContext.Response.StatusCode = exception.StatusCode;

                // logging

                if (apiError.Errors != null)
                {
                    foreach (var entry in apiError.Errors)
                    {
                        message += $" {entry.Key}: {entry.Value}";
                    }
                }

                // Add data in a queue for logging
                _loggingService.Log(LogLevel.Warning, $"{context.ActionDescriptor.DisplayName}. {message}");
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                message = "Unauthorized access.";

                apiError = new ApiError(message);
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

                _loggingService.Log(LogLevel.Warning, $"{context.ActionDescriptor.DisplayName}. {message}");
            }
            else
            {
                string stack;

                if (_environment.IsDevelopment())
                {
                    message = context.Exception.GetBaseException().Message;
                    stack = context.Exception.StackTrace;
                }
                else
                {
                    message = "Internal server error.";
                    stack = null;
                }

                apiError = new ApiError(message);
                apiError.Detail = stack;

                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                // logging

                _loggingService.Log(LogLevel.Error, $"{context.ActionDescriptor.DisplayName}. {message}");
            }

            context.Result = new JsonResult(apiError);

            base.OnException(context);
        }
    }

}
