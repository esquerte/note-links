using FluentValidation;
using NoteLinks.Service.ViewModels;

namespace NoteLinks.Service.Validators
{
    public class CalendarModelValidator : AbstractValidator<CalendarModel>
    {
        public CalendarModelValidator()
        {
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(128);
        }
    }
}
