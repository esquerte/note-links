using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoteLinks.Service.ExceptionFilter;
using NoteLinks.Service.ViewModels;
using System.Linq;

namespace NoteLinks.Service.Validators
{
    public class CreateNoteModelValidator : AbstractValidator<CreateNoteModel>
    {
        public CreateNoteModelValidator()
        {
            RuleFor(x => x.CalendarCode).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(128);
            RuleFor(x => x.FromDate).NotEmpty();
            RuleFor(x => x.ToDate).NotEmpty().GreaterThanOrEqualTo(x => x.FromDate).When(x => x.ToDate.HasValue);
        }
    }
}
