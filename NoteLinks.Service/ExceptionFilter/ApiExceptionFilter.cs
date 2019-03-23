using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Service.ExceptionFilter
{    
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        ILogger<ApiExceptionFilter> _logger;
        IHostingEnvironment _environment;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IHostingEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public override void OnException(ExceptionContext context)
        {
            ApiError apiError = null;

            if (context.Exception is ApiException)
            {
                var exception = context.Exception as ApiException;
                context.Exception = null;
                apiError = new ApiError(exception.Message);
                context.HttpContext.Response.StatusCode = exception.StatusCode;

                _logger.LogWarning($"{context.ActionDescriptor.DisplayName} {exception.Message}");
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

                _logger.LogError($"{context.ActionDescriptor.DisplayName} {context.Exception.GetBaseException().Message}");
            }

            context.Result = new JsonResult(apiError);

            base.OnException(context);
        }
    }

}
