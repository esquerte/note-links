using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NoteLinks.Service.Logging.Service;
using Hosting = Microsoft.Extensions.Hosting;

namespace NoteLinks.Service.ExceptionFilter
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        IHostingEnvironment _environment;

        LoggingService _loggingService;

        public ApiExceptionFilter(Hosting.IHostedService loggingService, IHostingEnvironment environment)
        {
            _environment = environment;
            _loggingService = loggingService as LoggingService;
        }

        public override void OnException(ExceptionContext context)
        {
            ApiError apiError = null;

            if (context.Exception is ApiException)
            {         
                var exception = context.Exception as ApiException;
                context.Exception = null;

                apiError = new ApiError(context.ModelState);
                apiError.Message = exception.Message;

                context.HttpContext.Response.StatusCode = exception.StatusCode;

                // logging

                string message = exception.Message;

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
            else
            {
                string message, stack;

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

                context.HttpContext.Response.StatusCode = 500;

                // logging

                _loggingService.Log(LogLevel.Error, $"{context.ActionDescriptor.DisplayName}. {message}");
            }

            context.Result = new JsonResult(apiError);

            base.OnException(context);
        }
    }

}
