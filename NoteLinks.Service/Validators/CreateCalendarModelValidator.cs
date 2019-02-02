using FluentValidation;
using NoteLinks.Service.ViewModels;
namespace NoteLinks.Service.Validators
{
    public class CreateCalendarModelValidator : AbstractValidator<CreateCalendarModel>
    {
        public CreateCalendarModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(128);
        }
    }
}
