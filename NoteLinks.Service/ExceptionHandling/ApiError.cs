using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Service.ExceptionHandling
{
    public class ApiError
    {
        public string Message { get; set; }
        public string Detail { get; set; }
        public Dictionary<string,string> Errors { get; set; }

        public ApiError(string message)
        {
            Message = message;
        }

        public ApiError(ModelStateDictionary modelState)
        {
            if (modelState != null && modelState.Any(m => m.Value.Errors.Count > 0))
            {
                Errors = modelState.Keys.SelectMany(k => modelState[k].Errors.Select(e => new {
                             property = k,
                             message = string.IsNullOrEmpty(e.ErrorMessage) ? "The input was not valid." : e.ErrorMessage
                         })).ToDictionary(m => m.property, m => m.message);
            }
        }

    }
}
